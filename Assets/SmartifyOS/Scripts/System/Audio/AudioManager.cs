using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioConfig_SO audioConfig_SO;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            
        }

        public async Task SetSystemVolume(float volume)
        {
            await LinuxCommand.RunAsync($"amixer -D pulse sset Master {Mathf.Round(volume)}%");
            SaveManager.Load().system.audioVolume = volume;
        }
    }

}

