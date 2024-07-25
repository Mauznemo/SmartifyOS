using UnityEngine;
using SmartifyOS.QuickSettings;
using System;

public class ToggleInteriorLightQuickSettingsEntry : BaseQuickSettingsEntry
{
    private void Start()
    {
        Init();

        MainController.OnInteriorLightChanged += MainController_OnInteriorLightChanged;
    }

    private void MainController_OnInteriorLightChanged(bool obj)
    {
        SetToggle(obj);
    }

    //Executed if this script is on a gameobject together with QuickSettings.ToggleButton component
    protected override void OnToggleValueChanged(bool isOn)
    {
        //MainController.Instance.ActivateLight(isOn);
        LedManager.Instance.SetInteriorLight(isOn);
    }
}