using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.SaveSystem
{
    public partial class SaveData
    {
        public System system;
    }

    public struct System
    {
        public float audioVolume;
        public bool autoplayOnConnect;
        public EventPaths eventPaths;
    }

    public partial struct EventPaths
    {
        public string onUsbDeviceConnected;
    }
}


