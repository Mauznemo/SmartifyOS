using UnityEngine;
using SmartifyOS.QuickSettings;
using System;

public class ToggleWaveQuickSettingsEntry : BaseQuickSettingsEntry
{
    private void Start()
    {
        Init();

        LightController.Instance.OnWavingStateChanged += LightController_OnWavingStateChanged;
    }

    private void LightController_OnWavingStateChanged(bool obj)
    {
        SetToggle(obj);
    }

    //Executed if this script is on a gameobject together with QuickSettings.ToggleButton component
    protected override void OnToggleValueChanged(bool isOn)
    {
        LightController.Instance.ToggleWave();
    }
}