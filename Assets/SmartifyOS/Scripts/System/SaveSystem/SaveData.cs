using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.SaveSystem
{
    public partial class SaveData
    {
        public System system = new System();
        public Timezone timezone;
        public Notifications notifications = new Notifications();
    }

    public partial class System
    {
        public float audioVolume = 50;
        public bool allowOverAmplification = false;
        public bool autoplayOnConnect;
        public float brightness = 1;
    }

    public struct Timezone
    {
        public int hourOffset;
        public int minuteOffset;
    }

    public partial class Notifications
    {
        public bool ignoreErrors;
    }
}


