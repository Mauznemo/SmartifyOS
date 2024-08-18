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

        StartCoroutine(RequestData());
    }

    private IEnumerator RequestData()
    {
        yield return new WaitForSeconds(1f);

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
            float voltage = float.Parse(message.Substring(2), System.Globalization.CultureInfo.InvariantCulture);
            OnNewBatteryVoltage?.Invoke(voltage);

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
                    ShutdownSystem();
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
        }

    }

    #region Shutdown Logic

    public void AcceptShutdown()
    {
        cancelShutdown = true;
        ShutdownSystem();
    }

    public void CancelShutdown()
    {
        cancelShutdown = true;
    }

    private IEnumerator WaitForPowerRestore()
    {
        yield return new WaitForSeconds(3);

        if (!systemPower)
        {
            warningModalWindow = ModalWindow.Create();
            warningModalWindow.Init("System Shutdown", "The system had no power for 3 seconds. And will shutdown in 5 seconds", "Shutdown", "Cancel", AcceptShutdown, CancelShutdown);
            StartCoroutine(OpenShutdownWarning());
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

        ShutdownSystem();
    }

    private void ShutdownSystem()
    {
        if (SaveManager.Load().popups.autoCloseOnPowerOff)
        {
            LightController.Instance.Down();
        }
        SaveManager.Save();
        Send("off");
        //LeanTween.alphaCanvas(canvasGroup, 1, 0.5f);

        Invoke(nameof(Shutdown), 0.6f);
    }

    private void Shutdown()
    {
        string s = LinuxCommand.Run("sudo sleep 5s; sudo shutdown -h now");
        Application.Quit();
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
