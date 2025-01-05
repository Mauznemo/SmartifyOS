using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SmartifyOS.LinuxBluetooth
{
    public class BluetoothParser
    {
        public static BluetoothDeviceInfo ParseDeviceInfo(string input)
        {
            BluetoothDeviceInfo deviceInfo = new BluetoothDeviceInfo();

            string macPattern = @"Device\s([0-9A-Fa-f:]{17})";
            string namePattern = @"Name:\s*(.+)";
            string pairedPattern = @"Paired:\s*(yes|no)";
            string trustedPattern = @"Trusted:\s*(yes|no)";
            string bondedPattern = @"Bonded:\s*(yes|no)";
            string connectedPattern = @"Connected:\s*(yes|no)";
            string batteryPattern = @"Battery Percentage:\s*0x[\da-fA-F]{2}\s*\((\d+)\)";

            deviceInfo.macAddress = Regex.Match(input, macPattern).Groups[1].Value;
            deviceInfo.name = Regex.Match(input, namePattern).Groups[1].Value;
            deviceInfo.paired = Regex.Match(input, pairedPattern).Groups[1].Value == "yes";
            deviceInfo.trusted = Regex.Match(input, trustedPattern).Groups[1].Value == "yes";
            deviceInfo.bonded = Regex.Match(input, bondedPattern).Groups[1].Value == "yes";
            deviceInfo.connected = Regex.Match(input, connectedPattern).Groups[1].Value == "yes";
            try
            {
                deviceInfo.batteryPercentage = float.Parse(Regex.Match(input, batteryPattern).Groups[1].Value);
            }
            catch (Exception e)
            {
                deviceInfo.batteryPercentage = -1;
            }

            return deviceInfo;
        }

        public static List<BluetoothDevice> ParseDevices(string input)
        {
            var devices = new List<BluetoothDevice>();

            var deviceMatches = Regex.Matches(input, @"Device\s([0-9A-Fa-f:]{17})\s(.+)");
            foreach (Match match in deviceMatches)
            {
                var device = new BluetoothDevice();
                device.macAddress = match.Groups[1].Value;
                device.name = match.Groups[2].Value.Trim();
                devices.Add(device);
            }

            return devices;
        }

        public static (string macAddress, bool connected, bool success) ParseDeviceConnection(string input)
        {
            string macAddress = "";
            bool connected = false;
            bool success = true;

            var macAddressMatch = Regex.Match(input, @"Device\s([0-9A-Fa-f:]{17})");
            if (macAddressMatch.Success)
            {
                macAddress = macAddressMatch.Groups[1].Value;
            }
            else
            {
                success = false;
            }

            var connectedMatch = Regex.Match(input, @"Connected:\s(yes|no)");
            if (connectedMatch.Success)
            {
                connected = connectedMatch.Groups[1].Value == "yes";
            }
            else
            {
                success = false;
            }

            return (macAddress, connected, success);
        }

        public static (string macAddress, string deviceName) ExtractDeviceInfo(string input)
        {
            string namePattern = @"([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})\s+(.*)";
            string macPattern = @"([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})";

            Match nameMatch = Regex.Match(input, namePattern);
            Match macMatch = Regex.Match(input, macPattern);

            string macAddress = macMatch.Value;
            string deviceName = nameMatch.Groups[3].Value.Trim();

            return (macAddress, deviceName);
        }

        public static string ExtractDeviceMac(string input)
        {
            string macPattern = @"([0-9A-Fa-f]{2}[:]){5}([0-9A-Fa-f]{2})";

            Match macMatch = Regex.Match(input, macPattern);

            string macAddress = macMatch.Value;


            return macAddress;
        }

        public static string ExtractPasskey(string input)
        {
            string pattern = @"Confirm passkey (\d{6})";

            Match match = Regex.Match(input, pattern);

            string passkey = match.Groups[1].Value;

            return passkey;
        }

        public static string ParseSongTitle(string input)
        {
            string titlePattern = @"Title:\s(.+)";

            Match titleMatch = Regex.Match(input, titlePattern);

            if (titleMatch.Success)
            {
                return titleMatch.Groups[1].Value.Trim();
            }

            return "Parsing error";
        }

        public static string ParseSongArtist(string input)
        {
            string artistPattern = @"Artist:\s(.+)";

            Match artistMatch = Regex.Match(input, artistPattern);

            if (artistMatch.Success)
            {
                return artistMatch.Groups[1].Value.Trim();
            }

            return "Parsing error";
        }

        [Obsolete("Use ParseSongTitle() and ParseSongArtist() instead")]
        public static (string title, string artist) ParseArtistAndSongTitle(string input)
        {
            string title = "";
            string artist = "";

            string titlePattern = @"Track.Title:\s(.+)";
            string artistPattern = @"Track.Artist:\s(.+)";

            Match titleMatch = Regex.Match(input, titlePattern);
            Match artistMatch = Regex.Match(input, artistPattern);

            if (titleMatch.Success)
            {
                title = titleMatch.Groups[1].Value.Trim();

            }
            if (artistMatch.Success)
            {
                artist = artistMatch.Groups[1].Value.Trim();

            }

            return (title, artist);
        }

        public static EventType ParseEventType(string input)
        {
            if (input.Contains("DEL"))
            {
                if (input.Contains("Device"))
                {
                    return EventType.DEL_Device;
                }
                else if (input.Contains("Transport"))
                {
                    return EventType.DEL_Transport;
                }
            }
            else if (input.Contains("NEW") && input.Contains("Device"))
            {
                if (input.Contains("Device"))
                {
                    return EventType.NEW_Device;
                }
                else if (input.Contains("Transport"))
                {
                    return EventType.NEW_Transport;
                }
            }
            else if (input.Contains("CHG"))
            {
                if (input.Contains("Player"))
                {
                    return EventType.CHG_Player;
                }
                else if (input.Contains("Device"))
                {
                    return EventType.CHG_Device;
                }
                else if (input.Contains("Transport"))
                {
                    return EventType.CHG_Transport;
                }
            }
            else if (input.Trim().StartsWith("Status:") || input.Trim().StartsWith("Track.Title:") || input.Trim().StartsWith("Track.Artist:"))
            {
                return EventType.CHG_Player;
            }

            return EventType.Default;
        }

        public static bool IsSoftBlocked(string input)
        {
            bool softBlocked = false;

            var softBlockedMatch = Regex.Match(input, @"\d+:\s+hci\d+:\s+Bluetooth\s+Soft\sblocked:\s(yes|no)");
            if (softBlockedMatch.Success)
            {
                softBlocked = softBlockedMatch.Groups[1].Value == "yes";
            }

            return softBlocked;
        }
    }

    [Serializable]
    public struct BluetoothDevice
    {
        public string name;
        public string macAddress;
    }

    public struct BluetoothDeviceInfo
    {
        public string name;
        public string macAddress;
        public bool paired;
        public bool trusted;
        public bool bonded;
        public bool connected;
        public float batteryPercentage;
    }

    public enum EventType
    {
        Default,
        NEW_Device,
        NEW_Transport,
        DEL_Transport,
        DEL_Player,
        DEL_Endpoint,
        DEL_Device,
        CHG_Transport,
        CHG_Device,
        CHG_Player,
    }
}