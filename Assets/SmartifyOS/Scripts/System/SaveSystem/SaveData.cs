using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.SaveSystem
{
    public partial class SaveData
    {
        public System system = new System();
    }

    public partial class System
    {
        public float audioVolume = 50;
        public bool autoplayOnConnect;
        public float brightness = 1;
    }
}


