using System;
using SmartifyOS.UI;
using TMPro;
using UnityEngine;

public class RealtimeInfoUIWindow : BaseUIWindow
{
    [SerializeField] private RealtimeInfoGraphs realtimeInfoGraphs;
    [SerializeField] private TMP_Text steeringAngleText;
    [SerializeField] private TMP_Text batteryText;



    private void Start()
    {
        Init();

        InvokeRepeating(nameof(UpdateOtherData), 0.1f, 0.2f);

        MainController.OnNewBatteryVoltage += MainController_OnNewBatteryVoltage;
    }

    private void MainController_OnNewBatteryVoltage(float voltage)
    {
        float minVoltage = 11.5f;  // Voltage corresponding to 0% battery
        float maxVoltage = 12.6f;  // Voltage corresponding to 100% battery

        float percentage = (voltage - minVoltage) / (maxVoltage - minVoltage) * 100;

        bool charging = voltage > maxVoltage + 0.4f;

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
        if (!isOpen)
        {
            return;
        }

        steeringAngleText.text = $"Steering Angle: {LiveDataController.steeringWheelAngle}°";
    }

    protected override void HandleWindowOpened(BaseUIWindow window)
    {
        //Add all windows that should hide this window when they open
        if (window.IsWindowOfType(typeof(AppListUIWindow)))
        {
            Hide(true);
        }
    }
}
