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

        public static bool playingWarningSound { get; private set; }

        public static AudioConfig_SO audioConfig_SO => Instance._audioConfig_SO;

        [SerializeField] private AudioConfig_SO _audioConfig_SO;

        [SerializeField] private AudioSource soundAudioSource;
        [SerializeField] private AudioSource warningAudioSource;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            warningAudioSource.loop = true;
            warningAudioSource.clip = audioConfig_SO.notificationSounds.warningLoop;
        }

        public void PlaySound(AudioClip audioClip)
        {
            soundAudioSource.PlayOneShot(audioClip);
        }

        public void StartWarningSound()
        {
            playingWarningSound = true;

            warningAudioSource.Play();
        }

        public void StopWarningSound()
        {
            playingWarningSound = false;

            warningAudioSource.Stop();
        }

        public async Task SetSystemVolume(float volume)
        {
            float maxClamp = SaveManager.Load().system.allowOverAmplification ? 100 : 150;
            volume = Mathf.Clamp(volume, 0, maxClamp);

            await LinuxCommand.RunAsync($"pactl set-sink-volume 0 {Mathf.Round(volume)}%");
            SaveManager.Load().system.audioVolume = volume;
        }
    }

}

