using SmartifyOS.LinuxBluetooth;
using SmartifyOS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace SmartifyOS.LinuxBluetooth
{
    public class BluetoothManager : MonoBehaviour
    {
        public static BluetoothManager Instance { get; private set; }

        //Player
        public static event Action OnPlayerPlaying;
        public static event Action OnPlayerPaused;
        public static event Action<string> OnPlayerTitleChanged;
        public static event Action<string> OnPlayerArtistChanged;
        public static event Action OnPlayerStopped;

        //UI
        public static event Action OnUpdateUI;
        public static event Action<string> OnRemoveDevice;
        //public static event Action<string, bool> OnDeviceFound;
        public static event Action<(string macAddress, string name, bool paired)> OnDeviceFound;

        //Connection
        public static event Action<string> OnDeviceConnected;
        public static event Action OnFailedToConnected;
        public static event Action<string> OnDeviceDisconnected;
        public static event Action<string> OnDeviceNotAvailable;
        public static event Action<string> OnConfirmPasskey;

        public static event Action<bool> OnSoftBlockedChanged;

        private void Awake()
        {
            Instance = this;
        }

        private Process process;
        private StreamWriter processInputWriter;

        [SerializeField] private Sprite bluetoothIconSprite;

        [SerializeField] private List<BluetoothDevice> bluetoothDevices = new List<BluetoothDevice>();

        private void Start()
        {
            StartBluetoothCtl();

            var devices = ListConnectedDevices();

            if (devices.Count > 0)
            {
                OnDeviceConnected?.Invoke(devices[0].macAddress);
                UnityEngine.Debug.Log($"<b><color=green>Connected to: {devices[0].macAddress}</color></b>");
                statusEntry = StatusBar.StatusBar.AddStatus(bluetoothIconSprite);
            }
        }

        public bool IsSoftBlocked()
        {
            string input = LinuxCommand.Run("rfkill list");
            bool softLock = BluetoothParser.IsSoftBlocked(input);
            return softLock;
        }

        public void SetBluetoothBlock(bool blocked)
        {
            OnSoftBlockedChanged?.Invoke(blocked);
            string command = blocked ? "rfkill block bluetooth" : "rfkill unblock bluetooth";
            LinuxCommand.Run(command);
        }

        public void SetScan(bool on)
        {
            string command = on ? "scan on" : "scan off";
            SendCommand(command);
        }

        public void SetPower(bool on)
        {
            string command = on ? "power on" : "power off";
            SendCommand(command);
        }

        public void SetDiscoverable(bool on)
        {
            string command = on ? "discoverable on" : "discoverable off";
            SendCommand(command);
        }

        public void SetDiscoverableTimeout(int seconds)
        {
            string command = $"discoverable-timeout {seconds}";
            SendCommand(command);
        }

        public void SetPairable(bool on)
        {
            string command = on ? "pairable on" : "pairable off";
            SendCommand(command);
        }

        public void SetAlias(string alias)
        {
            SendCommand($"set-alias {alias}");
        }

        public void ConnectToDevice(string deviceAddress)
        {
            SendCommand($"connect {deviceAddress}");
        }

        public void DisconnectToDevice(string deviceAddress)
        {
            SendCommand($"disconnect {deviceAddress}");
        }

        public void PairDevice(string deviceAddress)
        {
            SendCommand($"pair {deviceAddress}");
        }

        public void RemoveDevice(string deviceAddress)
        {
            SendCommand($"remove {deviceAddress}");
        }

        public void TrustDevice(string deviceAddress)
        {
            SendCommand($"trust {deviceAddress}");
        }

        public void UntrustDevice(string deviceAddress)
        {
            SendCommand($"untrust {deviceAddress}");
        }

        public void ConfirmPasskey()
        {
            UnityEngine.Debug.Log("Confirmed Passkey");
            SendCommand("yes");
        }

        public void DenyPasskey()
        {
            UnityEngine.Debug.Log("Denied Passkey");
            SendCommand("no");
        }

        public List<BluetoothDevice> ListPairedDevices()
        {
            string devices = LinuxCommand.Run("bluetoothctl devices Paired");
            return BluetoothParser.ParseDevices(devices);
        }

        public List<BluetoothDevice> ListBondedDevices()
        {
            string devices = LinuxCommand.Run("bluetoothctl devices Bonded");
            return BluetoothParser.ParseDevices(devices);
        }

        public List<BluetoothDevice> ListTrustedDevices()
        {
            string devices = LinuxCommand.Run("bluetoothctl devices Trusted");
            return BluetoothParser.ParseDevices(devices);
        }

        public List<BluetoothDevice> ListConnectedDevices()
        {
            string devices = LinuxCommand.Run("bluetoothctl devices Connected");
            return BluetoothParser.ParseDevices(devices);
        }

        public BluetoothDeviceInfo GetDeviceInfo(string deviceAddress)
        {
            string info = LinuxCommand.Run($"bluetoothctl info {deviceAddress}");
            return BluetoothParser.ParseDeviceInfo(info);
        }

        public void RemoveDeviceFromList(string deviceAddress)
        {
            bluetoothDevices.RemoveAll(device => device.macAddress == deviceAddress);
        }

        public void PlayerPlay()
        {
            SendPlayerCommand("play");
        }

        public void PlayerPause()
        {
            SendPlayerCommand("pause");
        }

        public void PlayerNext()
        {
            SendPlayerCommand("next");
        }

        public void PlayerPrevious()
        {
            SendPlayerCommand("previous");
        }

        public void StartBluetoothCtl()
        {
            // Check if the system is running on Linux
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                UnityEngine.Debug.Log("Unsupported platform: This function is intended for Linux systems only.");
                return;
            }

            // Create process start info
            ProcessStartInfo psi = new ProcessStartInfo("bluetoothctl");
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            // Start the process
            process = new Process();
            process.StartInfo = psi;
            process.Start();

            // Read the output
            process.OutputDataReceived += OutputDataReceived;
            process.ErrorDataReceived += ErrorDataReceived;

            processInputWriter = process.StandardInput;

            process.BeginOutputReadLine();
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            UnityMainThreadDispatcher.GetInstance().Enqueue(() => OutputDataReceivedMainThread(e.Data));
        }

        private void OutputDataReceivedMainThread(string input)
        {
            EventType eventType = BluetoothParser.ParseEventType(input);
            switch (eventType)
            {
                case EventType.NEW_Device:
                    var (macAddress, deviceName) = BluetoothParser.ExtractDeviceInfo(input);
                    UnityEngine.Debug.Log($"New device: {deviceName}");
                    if (macAddress != null)
                    {
                        HandleNewDevice(deviceName, macAddress, false);
                    }
                    break;
                case EventType.NEW_Transport:
                    UnityEngine.Debug.Log("New Transport");
                    break;
                case EventType.DEL_Device:
                    UnityEngine.Debug.Log("Device deleted");
                    OnRemoveDevice?.Invoke(BluetoothParser.ExtractDeviceInfo(input).macAddress);
                    break;
                case EventType.DEL_Transport:
                    UnityEngine.Debug.Log("Transport deleted");
                    break;
                case EventType.CHG_Device:
                    UnityEngine.Debug.Log("Device changed");
                    HandleDeviceChange(input);
                    break;
                case EventType.CHG_Transport:
                    UnityEngine.Debug.Log("Transport changed");
                    break;
                case EventType.CHG_Player:
                    HandlePlayerChange(input);
                    break;
                default:
                    break;
            }


            if (input.Contains("Confirm passkey"))
            {
                //SendCommand("yes");
                OnConfirmPasskey?.Invoke(BluetoothParser.ExtractPasskey(input));
                return;
            }
            else if (input.Contains("Failed to connect"))
            {
                OnFailedToConnected?.Invoke();
                return;
            }
            else if (input.Contains("not available"))
            {
                OnDeviceNotAvailable?.Invoke(BluetoothParser.ExtractDeviceMac(input));
                return;
            }
        }

        private StatusBar.StatusBar.StatusEntry statusEntry;

        private void HandleDeviceChange(string input)
        {
            OnUpdateUI?.Invoke();

            var (macAddress, connected, success) = BluetoothParser.ParseDeviceConnection(input);

            if (!success)
                return;

            if (connected)
            {
                OnDeviceConnected?.Invoke(macAddress);
                UnityEngine.Debug.Log($"<b><color=green>Connected to: {macAddress}</color></b>");
                statusEntry = StatusBar.StatusBar.AddStatus(bluetoothIconSprite);
            }
            else
            {
                OnDeviceDisconnected?.Invoke(macAddress);
                UnityEngine.Debug.Log($"<b><color=red>Disconnected from: {macAddress}</color></b>");
                statusEntry.Remove();
            }
        }

        private void HandlePlayerChange(string input)
        {
            if (input.Contains("paused"))
            {
                OnPlayerPaused?.Invoke();

                UnityEngine.Debug.Log("Player paused");
                return;
            }
            else if (input.Contains("playing"))
            {
                OnPlayerPlaying?.Invoke();
                UnityEngine.Debug.Log("Player playing");
                return;
            }
            else if (input.Contains("stopped"))
            {
                OnPlayerStopped?.Invoke();
                UnityEngine.Debug.Log("Player Stopped (HIDE)");
                return;
            }

            var (title, artist) = BluetoothParser.ParseArtistAndSongTitle(input);

            if (!string.IsNullOrEmpty(title))
            {
                UnityEngine.Debug.Log($"Player title changed to: {title}");
                OnPlayerTitleChanged?.Invoke(title);
                return;
            }
            else if (!string.IsNullOrEmpty(artist))
            {
                UnityEngine.Debug.Log($"Player artist changed to: {artist}");
                OnPlayerArtistChanged?.Invoke(artist);
                return;
            }
        }

        public void HandleNewDevice(string name, string macAddress, bool paired)
        {
            if (string.IsNullOrEmpty(macAddress)) { return; }

            if (!bluetoothDevices.Exists(device => device.macAddress == macAddress))
            {
                bluetoothDevices.Add(new BluetoothDevice
                {
                    name = name,
                    macAddress = macAddress
                });

                OnDeviceFound?.Invoke((macAddress, name, paired));
                UnityEngine.Debug.Log($"<b>Found: {name}</b>");
            }
            else
            {
                OnUpdateUI?.Invoke();
            }
        }

        public void SendPlayerCommand(string command)
        {
            SendCommand("menu player");
            SendCommand(command);
            SendCommand("back");
        }

        public void SendCommand(string command)
        {
            if (process != null && !process.HasExited && processInputWriter != null)
            {
                processInputWriter.WriteLine(command);
                processInputWriter.Flush();  // Ensure the command is sent immediately
            }
        }

        void OnDestroy()
        {
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }

            if (processInputWriter != null)
            {
                processInputWriter.Close();
                processInputWriter = null;
            }
        }
        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            UnityEngine.Debug.LogError(e.Data);
        }
    }

}
