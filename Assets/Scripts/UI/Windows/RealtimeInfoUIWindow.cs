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
}
