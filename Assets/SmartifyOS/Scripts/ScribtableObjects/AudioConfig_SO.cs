using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Audio
{
    [CreateAssetMenu(fileName = "AudioConfigSO", menuName = "SmartifyOS/ScriptableObjects/AudioConfigSo", order = 1)]
    public partial class AudioConfig_SO : ScriptableObject
    {
        public NotificationSounds notificationSounds;
        public BluetoothSounds bluetoothSounds;
    }

    [System.Serializable]
    public struct NotificationSounds
    {
        public AudioClip info;
        public AudioClip warning;
        public AudioClip error;
    }

    [System.Serializable]
    public struct BluetoothSounds
    {
        public AudioClip connect;
        public AudioClip disconnect;
    }
}
