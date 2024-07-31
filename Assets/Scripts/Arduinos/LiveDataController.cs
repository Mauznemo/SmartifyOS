using UnityEngine;
using SmartifyOS.SerialCommunication;
using SmartifyOS.SaveSystem;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartifyOS.StatusBar;

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
    public static bool hasGpsSignal { get; private set; }

    private float _speedKmh;
    private float _rpm;
    private float _steeringWheelAngle;

    [SerializeField] private AnimationCurve speedDisplayRemap;
    [SerializeField] private InfoDisplay infoDisplay;

    [SerializeField] private int smoothingFactorSpeedKmh = 5;
    [SerializeField] private int smoothingFactorRpm = 5;
    [SerializeField] private int smoothingFactorSteeringWheelAngle = 5;

    [SerializeField] private Sprite noGpsSprite;

    private StatusBar.StatusEntry noGpsSignalStatusEntry;

    private float lastSpeedKmh;
    private float lastRpm;
    private float lastSteeringWheelAngle;

    private List<float> rpmReadings = new List<float>();
    private List<float> speedKmhReadings = new List<float>();
    private List<float> steeringWheelAngleReadings = new List<float>();


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
        noGpsSignalStatusEntry = StatusBar.AddStatus(noGpsSprite);

        portName = SaveManager.Load().liveController.arduinoPort;

        Debug.Log("Live controller port: " + portName);
        InitLive();

        Debug.Log("Live controller connected: " + IsConnected());
    }

    private void Update()
    {
        //REMOVE THIS LATER
        DetermineDrivingState();
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

                float rpmRaw = float.Parse(values[0], CultureInfo.InvariantCulture) * 1000f;
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
                        noGpsSignalStatusEntry.Hide();
                        OnGpsSignal?.Invoke(true);
                    }
                }

                _rpm = rpmRaw;
                rpm = GetSmoothedRpm();
                _steeringWheelAngle = -steeringWheelAngleRaw;
                steeringWheelAngle = GetSteeringWheelAngle();
                wheelAngle = steeringWheelAngle / 15f;

                if (rpm > highestRpm)
                {
                    highestRpm = rpm;
                }

                infoDisplay.SetFirstText(GetRemappedSpeed(speedKmh), "km/h", "0");
                infoDisplay.SetSecondText(rpm / 1000f, "K RPM", "0.00");
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
            if (speedKmh <= 10f)
            {
                drivingTriggerTimer = drivingTriggerTime;
                isDriving = false;
                OnStoppedDriving?.Invoke();
                Debug.Log("Stopped Driving");
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
                    Debug.Log("Started Driving");
                }
            }
        }
    }

    private float GetRemappedSpeed(float speed)
    {
        float remappedSpeed = speedDisplayRemap.Evaluate(speed);

        if (speed > GetMaxTimeOfCurve(speedDisplayRemap))
            return speed;
        else
            return remappedSpeed;
    }

    private float GetMaxTimeOfCurve(AnimationCurve curve)
    {
        float maxTime = 0f;

        foreach (Keyframe key in curve.keys)
        {
            if (key.time > maxTime)
            {
                maxTime = key.time;
            }
        }

        return maxTime;
    }

    private float GetSmoothedRpm()
    {
        if (rpm == _rpm)
            return rpm;

        rpmReadings.Add(_rpm);

        if (rpmReadings.Count > smoothingFactorRpm)
            rpmReadings.RemoveAt(0);

        return WeightedMovingAverage(rpmReadings);
    }

    private float GetSteeringWheelAngle()
    {
        if (steeringWheelAngle == _steeringWheelAngle)
            return _steeringWheelAngle;

        steeringWheelAngleReadings.Add(_steeringWheelAngle);

        if (steeringWheelAngleReadings.Count > smoothingFactorSteeringWheelAngle)
            steeringWheelAngleReadings.RemoveAt(0);

        return WeightedMovingAverage(steeringWheelAngleReadings);
    }

    private float GetSmoothedSpeed()
    {
        if (speedKmh == _speedKmh)
            return speedKmh;


        speedKmhReadings.Add(_speedKmh);

        if (speedKmhReadings.Count > smoothingFactorSpeedKmh)
            speedKmhReadings.RemoveAt(0);

        return WeightedMovingAverage(speedKmhReadings);
    }

    private float WeightedMovingAverage(List<float> readings)
    {
        int count = readings.Count;
        float weightedSum = 0;
        int weightTotal = 0;

        for (int i = 0; i < count; i++)
        {
            int weight = i + 1;
            weightedSum += readings[i] * weight;
            weightTotal += weight;
        }

        return weightedSum / weightTotal;
    }
}
