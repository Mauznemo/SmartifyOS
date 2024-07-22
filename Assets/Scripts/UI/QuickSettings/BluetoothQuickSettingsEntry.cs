using UnityEngine;
using SmartifyOS.QuickSettings;
using SmartifyOS.LinuxBluetooth;
using System;

public class BluetoothQuickSettingsEntry : BaseQuickSettingsEntry
{
    private void Start()
    {
        Init();
        SetToggle(!BluetoothManager.Instance.IsSoftBlocked());

        BluetoothManager.OnSoftBlockedChanged += BluetoothManager_OnSoftBlockedChanged;
    }

    private void BluetoothManager_OnSoftBlockedChanged(bool value)
    {
        SetToggle(!value);
    }


    //Executed if this script is on a gameobject together with QuickSettings.ToggleButton component
    protected override void OnToggleValueChanged(bool isOn)
    {
        BluetoothManager.Instance.SetBluetoothBlock(!isOn);
    }
}