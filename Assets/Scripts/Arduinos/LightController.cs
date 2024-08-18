using UnityEngine;
using SmartifyOS.SerialCommunication;
using System;
using TMPro;
using SmartifyOS.SaveSystem;
using SmartifyOS.Notifications;
using System.Collections;

public class LightController : BaseSerialCommunication
{
    public static LightController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public event Action<LightState> OnLeftLightStateChanged;
    public event Action<LightState> OnRightLightStateChanged;

    public event Action<bool> OnWavingStateChanged;


    private LightState leftLightState;
    private LightState rightLightState;

    private bool waving;

    private void Start()
    {
        portName = SaveManager.Load().lightController.arduinoPort;
        Debug.Log("Light controller port: " + portName);
        Init();
        Debug.Log("Light controller connected: " + IsConnected());

        StartCoroutine(RequestData());
    }

    private IEnumerator RequestData()
    {
        yield return new WaitForSeconds(1f);

        if (!IsConnected())
            yield break;

        Send("ss");
    }

    private void Update()
    {
        ReadMessage();
    }

    private bool canTriggerLightError = true;
    private float lightErrorCooldown = 0.2f;

    private void OnLightError()
    {
        if (!canTriggerLightError)
        {
            return;
        }

        if (SaveManager.Load().popups.allowModifyingWhileOn)
        {
            NotificationManager.SendNotification(NotificationType.Warning, "The light is still on!");
        }
        else
        {
            NotificationManager.SendNotification(NotificationType.Warning, "You can't modify the light while it's on!");
        }

        StartCoroutine(StartLightErrorCooldown());
    }

    private IEnumerator StartLightErrorCooldown()
    {
        canTriggerLightError = false;
        yield return new WaitForSeconds(lightErrorCooldown);
        canTriggerLightError = true;
    }

    public override void Received(string message)
    {
        switch (message)
        {
            case "ru":
                rightLightState = LightState.Up;
                OnRightLightStateChanged?.Invoke(rightLightState);
                break;
            case "rd":
                rightLightState = LightState.Down;
                OnRightLightStateChanged?.Invoke(rightLightState);
                break;
            case "lu":
                leftLightState = LightState.Up;
                OnLeftLightStateChanged?.Invoke(leftLightState);
                break;
            case "ld":
                leftLightState = LightState.Down;
                OnLeftLightStateChanged?.Invoke(leftLightState);
                break;

            case "ws": //waving started
                waving = true;
                OnWavingStateChanged?.Invoke(waving);
                break;
            case "we": // waving ended
                waving = false;
                OnWavingStateChanged?.Invoke(waving);
                break;

            case "el": //error light (when trying to change light motor state while light is on)
                OnLightError();
                break;
        }
    }

    public void Toggle()
    {
        Send("tg");
    }

    public void ToggleLeft()
    {
        Send("tl");
    }

    public void ToggleRight()
    {
        Send("tr");
    }

    public void WinkLeft()
    {
        Send("wl");
    }

    public void WinkRight()
    {
        Send("wr");
    }

    public void ToggleWave()
    {
        Send("tw");
    }

    public void Down()
    {
        Send("rs");
    }

    public void Up()
    {
        Send("up");
    }

    public void AllowModifyingLightsWhileOn(bool allow)
    {
        if (allow)
        {
            Send("am");
        }
        else
        {
            Send("dm");
        }
    }

    public enum LightState
    {
        Down,
        Up,
    }
}
