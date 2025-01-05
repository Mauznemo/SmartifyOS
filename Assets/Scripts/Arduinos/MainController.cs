using UnityEngine;
using SmartifyOS.SerialCommunication;
using System;
using SmartifyOS;
using System.Collections;
using SmartifyOS.SaveSystem;
using SmartifyOS.UI;
using SmartifyOS.StatusBar;

public class MainController : BaseSerialCommunication
{
    public static MainController Instance { get; private set; }

    public static event Action OnReverse;
    public static event Action OnForward;
    public static event Action<bool> OnActionButton1;
    public static event Action<bool> OnActionButton2;

    public static event Action<bool> OnControlwheelButton;
    public static event Action<int> OnControlwheelChanged;

    public static event Action<bool> OnLeftDoorOpened;
    public static event Action<bool> OnRightDoorOpened;
    public static event Action<bool> OnTrunkOpened;

    public static event Action<bool> OnInteriorLightChanged;

    public static event Action<float> OnNewBatteryVoltage;

    public static bool isInReverse { get; private set; }

    public static bool leftDoorOpen;
    public static bool rightDoorOpen;
    public static bool trunkOpen;


    public static bool systemPower = true;

    [SerializeField] private Sprite noPowerIconSprite;

    private bool cancelShutdown;
    private ModalWindow warningModalWindow;
    private StatusBar.StatusEntry noPowerStatusEntry;

    private bool ignoreControlWheel = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        portName = SaveManager.Load().mainController.arduinoPort;
        Debug.Log("Main controller port: " + portName);
        Init();
        Debug.Log("Main controller connected: " + IsConnected());

        noPowerStatusEntry = StatusBar.AddStatus(noPowerIconSprite);
        noPowerStatusEntry.Hide();

        SystemManager.OnPowerOff += SystemManager_OnPowerOff;

        StartCoroutine(RequestData());
    }

    private IEnumerator RequestData()
    {
        yield return new WaitForSeconds(1f);

        ignoreControlWheel = false;

        if (!IsConnected())
            yield break;

        Send("sd");
    }

    private void Update()
    {
        ReadMessage();
    }

    public override void Received(string message)
    {
        if (message.StartsWith("v_"))
        {
            if (float.TryParse(message.Substring(2), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float voltage))
            {
                OnNewBatteryVoltage?.Invoke(voltage + 0.5f); //According to my multimeter the battery reading was 0.5V to low
            }

            return;
        }

        switch (message)
        {
            case "po":
                systemPower = false;
                noPowerStatusEntry.Show();

                //check if there was rpm before if so shutdown instantly
                if (LiveDataController.highestRpm > 1)
                {
                    StartCoroutine(WaitForPowerRestore(3f, false));
                }
                else
                {
                    StartCoroutine(WaitForPowerRestore());
                }
                break;
            case "p":
                systemPower = true;
                noPowerStatusEntry.Hide();

                break;
            case "re": //in reverse
                OnReverse?.Invoke();
                isInReverse = true;
                break;
            case "rd": //not in reverse
                OnForward?.Invoke();
                isInReverse = false;
                break;
            case "a1u":
                OnActionButton1?.Invoke(false); //Down is true
                break;
            case "a1d":
                OnActionButton1?.Invoke(true);
                break;
            case "a2u":
                OnActionButton2?.Invoke(false);
                break;
            case "a2d":
                OnActionButton2?.Invoke(true);
                break;
            case "ldo":
                leftDoorOpen = true;
                OnLeftDoorOpened?.Invoke(true);
                break;
            case "ldc":
                leftDoorOpen = false;
                OnLeftDoorOpened?.Invoke(false);
                break;
            case "rdo":
                rightDoorOpen = true;
                OnRightDoorOpened?.Invoke(true);
                break;
            case "rdc":
                rightDoorOpen = false;
                OnRightDoorOpened?.Invoke(false);
                break;
            case "to":
                trunkOpen = true;
                OnTrunkOpened?.Invoke(true);
                break;
            case "tc":
                trunkOpen = false;
                OnTrunkOpened?.Invoke(false);
                break;
            case "cwu":
                if (ignoreControlWheel) return;
                OnControlwheelChanged?.Invoke(1);
                break;
            case "cwd":
                if (ignoreControlWheel) return;
                OnControlwheelChanged?.Invoke(-1);
                break;
            case "cwbd":
                if (ignoreControlWheel) return;
                OnControlwheelButton?.Invoke(true);
                break;
            case "cwbu":
                if (ignoreControlWheel) return;
                OnControlwheelButton?.Invoke(false);
                break;
        }

    }

    #region Shutdown Logic

    public void AcceptShutdown()
    {
        cancelShutdown = true;
        SystemManager.Instance.ShutdownSystem();
    }

    public void CancelShutdown()
    {
        cancelShutdown = true;
    }

    private IEnumerator WaitForPowerRestore(float time = 3, bool showDialog = true)
    {
        yield return new WaitForSeconds(time);

        if (showDialog)
        {
            if (!systemPower)
            {
                warningModalWindow = ModalWindow.Create();
                warningModalWindow.Init("System Shutdown", "The system had no power for 3 seconds. And will shutdown in 5 seconds", "Shutdown", "Cancel", AcceptShutdown, CancelShutdown);
                StartCoroutine(OpenShutdownWarning());
            }
        }
        else
        {
            if (!systemPower)
                SystemManager.Instance.ShutdownSystem();
        }

    }

    private IEnumerator OpenShutdownWarning()
    {
        float time = 5;

        while (time > 0)
        {
            warningModalWindow.UpdateContent($"The system had no power for 3 seconds. And will shutdown in {Mathf.RoundToInt(time)} seconds");
            if (cancelShutdown)
            {
                cancelShutdown = false;
                yield break;
            }
            time -= Time.deltaTime;
            yield return null;
        }

        SystemManager.Instance.ShutdownSystem();
    }

    private void SystemManager_OnPowerOff()
    {
        if (SaveManager.Load().popups.autoCloseOnPowerOff)
        {
            LightController.Instance.Down();
        }
        Send("off");
    }


    #endregion

    public void ActivateLight(bool on)
    {
        if (on)
        {
            Send("le");
        }
        else
        {
            Send("ld");
        }
        OnInteriorLightChanged?.Invoke(on);
    }

    public void SetLedColor(Color color, LedStrip ledStrip)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(color);

        switch (ledStrip)
        {
            case LedStrip.Left:
                Send("ll_" + hexColor);
                break;
            case LedStrip.Right:
                Send("rl_" + hexColor);
                break;
        }
    }
}
