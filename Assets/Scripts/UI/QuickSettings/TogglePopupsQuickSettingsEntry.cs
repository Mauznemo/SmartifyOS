using UnityEngine;
using SmartifyOS.QuickSettings;
using System;

public class TogglePopupsQuickSettingsEntry : BaseQuickSettingsEntry
{
    private void Start()
    {
        Init();
        LightController.Instance.OnLeftLightStateChanged += LightController_OnLeftLightStateChanged;
    }

    private void LightController_OnLeftLightStateChanged(LightController.LightState state)
    {
        SetToggle(state == LightController.LightState.Up);
    }

    protected override void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
            LightController.Instance.Up();
        else
            LightController.Instance.Down();
    }
}