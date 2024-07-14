using SmartifyOS.LinuxBluetooth;
using SmartifyOS.UI.Components;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class BluetoothDeviceEntry : MonoBehaviour
    {
        [SerializeField] private BluetoothDevice device;

        [SerializeField] private UI.Components.Button connectButton;
        [SerializeField] private UI.Components.Button pairButton;
        [SerializeField] private UI.Components.Button trustButton;
        [SerializeField] private TMP_Text deviceNameText;
        [SerializeField] private TMP_Text deviceMacAddressText;

        [SerializeField] private bool connected;
        [SerializeField] private bool paired;
        [SerializeField] private bool trusted;

        private void Awake()
        {
            connectButton.onClick += () =>
            {
                Debug.Log("Click");
                if (connected)
                    BluetoothManager.Instance.DisconnectToDevice(device.macAddress);
                else
                    BluetoothManager.Instance.ConnectToDevice(device.macAddress);
            };
            pairButton.onClick += () =>
            {
                if (paired)
                    BluetoothManager.Instance.RemoveDevice(device.macAddress);
                else
                    BluetoothManager.Instance.PairDevice(device.macAddress);
            };

            trustButton.onClick += () =>
            {
                if (trusted)
                    BluetoothManager.Instance.UntrustDevice(device.macAddress);
                else
                    BluetoothManager.Instance.TrustDevice(device.macAddress);
            };
        }

        public void Init(BluetoothDevice newDevice)
        {
            device = newDevice;

            deviceNameText.text = device.name;
            deviceMacAddressText.text = device.macAddress;
            UpdateStats();

            BluetoothManager.OnUpdateUI += UpdateStats;
            BluetoothManager.OnRemoveDevice += OnRemoveDevice;
        }

        private void OnDestroy()
        {
            BluetoothManager.OnUpdateUI -= UpdateStats;
            BluetoothManager.OnRemoveDevice -= OnRemoveDevice;
        }

        private void OnRemoveDevice(string obj)
        {
            if (obj == device.macAddress)
            {
                BluetoothManager.Instance.RemoveDeviceFromList(obj);
                Debug.Log("Removed: " + obj);
                Destroy(gameObject);
            }
        }

        public void UpdateStats()
        {
            BluetoothDeviceInfo info = BluetoothManager.Instance.GetDeviceInfo(device.macAddress);
            paired = info.paired;
            connected = info.connected;
            trusted = info.trusted;

            connectButton.interactable = paired;

            connectButton.text = connected ? "Disconnect" : "Connect";
            pairButton.text = paired ? "Remove" : "Pair";
            trustButton.text = trusted ? "Untrust" : "Trust";
        }
    }

}


