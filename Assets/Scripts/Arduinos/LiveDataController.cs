using UnityEngine;
using SmartifyOS.SerialCommunication;
using SmartifyOS.SaveSystem;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

public class LiveDataController : BaseLiveSerialCommunication
{
    public static LiveDataController Instance { get; private set; }

    public static event Action<string> OnDateAndTime;
    public static event Action<bool> OnGpsSignal;

    public static event Action OnStartedDriving;
    public static event Action OnStoppedDriving;

    public static float speedKmh { get; private set; }
    public static float rpm { get; private set; }
    public static float highestRpm { get; private set; }
    public static float steeringWheelAngle { get; private set; }
    public static float wheelAngle { get; private set; }

    public static bool isDriving { get; private set; }

    private float _speedKmh;
    private float _rpm;
    private float _steeringWheelAngle;

    [SerializeField] private InfoDisplay infoDisplay;

    [SerializeField] private int smoothingFactorSpeedKmh = 5;
    [SerializeField] private int smoothingFactorRpm = 5;
    [SerializeField] private int smoothingFactorSteeringWheelAngle = 5;

    private float lastSpeedKmh;
    private float lastRpm;
    private float lastSteeringWheelAngle;

    private List<float> rpmReadings = new List<float>();
    private List<float> speedKmhReadings = new List<float>();
    private List<float> steeringWheelAngleReadings = new List<float>();

    private bool hasGpsSignal = false;

    private float drivingTriggerTime = 3f;
    private float drivingTriggerTimer = 3f;

    private int messagesToIgnore = 10;
    private int messagesReceived;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(portName))
            portName = SaveManager.Load().liveController.arduinoPort;

        InitLive();
    }

    private void OnDestroy()
    {
        StopSerialThread();
    }

    public override void Received(string message)
    {
        if (messagesReceived < messagesToIgnore)
        {
            messagesReceived++;
            return;
        }

        if (!string.IsNullOrEmpty(message))
        {
            if (message.Contains("time"))
            {
                OnDateAndTime?.Invoke(message);
                return;
            }

            try
            {
                string[] values = message.Split('_');

                float rpmRaw = float.Parse(values[0], CultureInfo.InvariantCulture);
                float speedKmhRaw = float.Parse(values[1], CultureInfo.InvariantCulture);
                float steeringWheelAngleRaw = float.Parse(values[2], CultureInfo.InvariantCulture);

                if (speedKmhRaw != -1)
                {
                    _speedKmh = speedKmhRaw;
                    speedKmh = GetSmoothedSpeed();

                    DetermineDrivingState();

                    if (!hasGpsSignal)
                    {
                        hasGpsSignal = true;
                        OnGpsSignal?.Invoke(true);
                    }
                }

                _rpm = rpmRaw;
                rpm = GetSmoothedRpm();
                _steeringWheelAngle = -steeringWheelAngleRaw;
                steeringWheelAngle = GetSteeringWheelAngle();

                if (rpm > highestRpm)
                {
                    highestRpm = rpm;
                }

                infoDisplay.SetFirstText(speedKmh, "km/h", "0");
                infoDisplay.SetFirstText(rpm, "K RPM", "0.00");
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }

    private void DetermineDrivingState()
    {
        if (isDriving)
        {
            if (speedKmh <= 2f)
            {
                drivingTriggerTimer = drivingTriggerTime;
                isDriving = false;
                OnStoppedDriving?.Invoke();
            }
        }
        else
        {
            if (speedKmh > 10f)
            {
                drivingTriggerTimer -= Time.deltaTime;
                if (drivingTriggerTimer <= 0)
                {
                    isDriving = true;
                    OnStartedDriving?.Invoke();
                }
            }
        }
    }

    private float GetSmoothedRpm()
    {
        if (_rpm == lastRpm)
            return rpm;

        lastRpm = _rpm;

        rpmReadings.Add(_rpm);

        if (rpmReadings.Count > smoothingFactorRpm)
            rpmReadings.RemoveAt(0);

        return rpmReadings.Average();
    }

    private float GetSteeringWheelAngle()
    {
        if (_steeringWheelAngle == lastSteeringWheelAngle)
            return _steeringWheelAngle;

        lastSteeringWheelAngle = _steeringWheelAngle;

        steeringWheelAngleReadings.Add(_steeringWheelAngle);

        if (steeringWheelAngleReadings.Count > smoothingFactorSteeringWheelAngle)
            steeringWheelAngleReadings.RemoveAt(0);

        return steeringWheelAngleReadings.Average();
    }

    private float GetSmoothedSpeed()
    {
        if (_speedKmh == lastSpeedKmh)
            return _speedKmh;

        lastSpeedKmh = _speedKmh;

        speedKmhReadings.Add(_speedKmh);

        if (speedKmhReadings.Count > smoothingFactorSpeedKmh)
            speedKmhReadings.RemoveAt(0);

        return speedKmhReadings.Average();
    }
}
