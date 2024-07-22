using UnityEngine;
using SmartifyOS.QuickSettings;
using System;

public class LockToggleQuickSettingsEntry : BaseQuickSettingsEntry
{
    private void Start()
    {
        Init();
        LockController.OnDoorsLocked += LockController_OnDoorsLocked;
        LockController.OnDoorsUnlocked += LockController_OnDoorsUnlocked;
    }

    private void LockController_OnDoorsUnlocked()
    {
        SetToggle(false);
    }

    private void LockController_OnDoorsLocked()
    {
        SetToggle(true);
    }

    //Executed if this script is on a gameobject together with QuickSettings.ToggleButton component
    protected override void OnToggleValueChanged(bool isOn)
    {
        LockController.Instance.LockDoors(isOn);
    }
}