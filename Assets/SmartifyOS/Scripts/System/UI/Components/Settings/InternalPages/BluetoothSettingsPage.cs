using SmartifyOS.LinuxBluetooth;
using SmartifyOS.UI;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class BluetoothSettingsPage : BaseSettingsPage
    {
        [SerializeField] private UI.Components.Button scanEnableButton;

        [SerializeField] private BluetoothDeviceEntry deviceEntryPrefab;
        [SerializeField] private Transform deviceEntryParent;
        [SerializeField] private Transform transformPaired;
        [SerializeField] private Transform transformFound;

        private bool blocked;

        private void Awake()
        {
            scanEnableButton.onClick += () =>
            {
                if (blocked)
                {
                    blocked = !blocked;
                    BluetoothManager.Instance.SetBluetoothBlock(blocked);
                    scanEnableButton.text = "Scan for Devices";
                }
                else
                {
                    BluetoothManager.Instance.SetScan(true);
                    scanEnableButton.text = "Scanning...";
                }
            };
    

            BluetoothManager.OnDeviceFound += HandleDeviceFound;
            BluetoothManager.OnConfirmPasskey += HandleConfirmPasskey;

        }

        protected override void OnOpened()
        {
            Debug.Log("OnOpen");
            blocked = BluetoothManager.Instance.IsSoftBlocked();
            if(blocked)
                scanEnableButton.text = "Turn on Bluetooth";

            ShowPairedDevices();
        }

        private void HandleConfirmPasskey(string obj)
        {
            
            ModalWindow.Create().Init("Passkey", $"Confirm passkey: {obj}", ModalWindow.ModalType.YesNo, () =>
            {
                BluetoothManager.Instance.ConfirmPasskey();
            }, () => { });
        }

        private void HandleDeviceFound((string macAddress, string name, bool paired) device)
        {
            int index;
            if (device.paired)
            {
                index = transformPaired.GetSiblingIndex();
            }
            else
            {
                index = transformFound.GetSiblingIndex();
                scanEnableButton.gameObject.SetActive(false);
            }

            BluetoothDeviceEntry deviceEntry = Instantiate(deviceEntryPrefab, deviceEntryParent);
            deviceEntry.gameObject.SetActive(true);
            deviceEntry.transform.SetSiblingIndex(index + 1);
            deviceEntry.Init(new BluetoothDevice
            {
                name = device.name,
                macAddress = device.macAddress
            });
        }

        private void ShowPairedDevices()
        {
            var devices = BluetoothManager.Instance.ListPairedDevices();
            foreach (var device in devices)
            {
                BluetoothManager.Instance.HandleNewDevice(device.name, device.macAddress, true);
            }
        }
    }

}