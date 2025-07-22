using System;
using SmartifyOS.StatusBar;
using SmartifyOS.UI;
using TMPro;
using UnityEngine;

public class RealtimeInfoUIWindow : BaseUIWindow
{
    [SerializeField] private RealtimeInfoGraphs realtimeInfoGraphs;
    [SerializeField] private TMP_Text steeringAngleText;
    [SerializeField] private TMP_Text batteryText;

    [SerializeField] private Sprite chargingSprite;
    [SerializeField] private Sprite lowBatterySprite;

    private StatusBar.StatusEntry chargingEntry;
    private StatusBar.StatusEntry lowBatteryEntry;

    private void Start()
    {
        Init();

        InvokeRepeating(nameof(UpdateOtherData), 0.1f, 0.2f);

        MainController.OnNewBatteryVoltage += MainController_OnNewBatteryVoltage;

        chargingEntry = StatusBar.AddStatus(chargingSprite);
        chargingEntry.Hide();
        lowBatteryEntry = StatusBar.AddStatus(lowBatterySprite);
        lowBatteryEntry.Hide();
    }

    private void MainController_OnNewBatteryVoltage(float voltage)
    {
        float minVoltage = 11.5f;  // Voltage corresponding to 0% battery
        float maxVoltage = 12.6f;  // Voltage corresponding to 100% battery

        float percentage = (voltage - minVoltage) / (maxVoltage - minVoltage) * 100;
        percentage = Mathf.Clamp(percentage, 0, 100);

        bool charging = voltage > maxVoltage + 0.1f;

        if (charging)
            chargingEntry.Show();
        else
            chargingEntry.Hide();

        if (percentage < 20)
            lowBatteryEntry.Show();
        else
            lowBatteryEntry.Hide();

        string percentageText = charging ? "charging" : $"{Mathf.Round(percentage)}%";
        batteryText.text = $"Battery: {voltage.ToString("0.00")}V ({percentageText})";
    }

    protected override void OnShow()
    {
        realtimeInfoGraphs.updateGraphs = true;
    }

    protected override void OnHide()
    {
        realtimeInfoGraphs.updateGraphs = false;
    }

    private void UpdateOtherData()
    {
        if (!isVisible)
        {
            return;
        }

        steeringAngleText.text = $"Steering Angle: {LiveDataController.steeringWheelAngle}°";
    }
}
