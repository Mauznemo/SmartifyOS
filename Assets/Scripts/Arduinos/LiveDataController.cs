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

    public static float speedKmh { get; private set; }
    public static float rpm { get; private set; }
    public static float highestRpm { get; private set; }
    public static float steeringWheelAngle { get; private set; }
    public static float wheelAngle { get; private set; }

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

                float rpm = float.Parse(values[0], CultureInfo.InvariantCulture);
                float speedKmh = float.Parse(values[1], CultureInfo.InvariantCulture);
                float steeringWheelAngle = float.Parse(values[2], CultureInfo.InvariantCulture);

                if (speedKmh != -1)
                {
                    _speedKmh = speedKmh;
                    LiveDataController.speedKmh = GetSmoothedSpeed();

                    if (!hasGpsSignal)
                    {
                        hasGpsSignal = true;
                        OnGpsSignal?.Invoke(true);
                    }
                }

                _rpm = rpm;
                LiveDataController.rpm = GetSmoothedRpm();
                _steeringWheelAngle = -steeringWheelAngle;
                LiveDataController.steeringWheelAngle = GetSteeringWheelAngle();

                if (LiveDataController.rpm > highestRpm)
                {
                    highestRpm = LiveDataController.rpm;
                }

                infoDisplay.SetFirstText(LiveDataController.speedKmh, "km/h", "0");
                infoDisplay.SetFirstText(LiveDataController.rpm, "K RPM", "0.00");
            }
            catch (Exception)
            {
                // ignored
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
