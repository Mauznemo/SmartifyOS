using System;
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

        /// <summary>Called when the volume is changed with <see cref="SetSystemVolumeWithOverlay"/> method</summary>
        public static event Action<float> OnVolumeChangedOverlay;

        /// <summary> Returns the <see cref="AudioConfig_SO"/></summary>
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

            LinuxCommand.Run($"pactl set-sink-mute 0 0");
        }

        /// <summary>
        /// Plays the <see cref="audioClip"/>
        /// </summary>
        /// <param name="audioClip">Audio clip to play</param>
        public void PlaySound(AudioClip audioClip)
        {
            soundAudioSource.PlayOneShot(audioClip);
        }

        /// <summary>
        /// Starts the <see cref="AudioConfig_SO.notificationSounds.warningLoop"/>
        /// </summary>
        public void StartWarningSound()
        {
            playingWarningSound = true;

            warningAudioSource.Play();
        }

        /// <summary>
        /// Stops the <see cref="AudioConfig_SO.notificationSounds.warningLoop"/>
        /// </summary>
        public void StopWarningSound()
        {
            playingWarningSound = false;

            warningAudioSource.Stop();
        }

        /// <summary>
        /// Sets the system output volume with an overlay (useful for external audio control)
        /// </summary>
        /// <param name="volume">Volume in percentage</param>
        public async Task SetSystemVolumeWithOverlay(float volume)
        {
            await SetSystemVolume(volume);
            OnVolumeChangedOverlay?.Invoke(volume);
        }

        /// <summary>
        /// Sets the system output volume
        /// </summary>
        /// <param name="volume">Volume in percentage</param>
        public async Task SetSystemVolume(float volume)
        {

            float maxClamp = SaveManager.Load().system.allowOverAmplification ? 150 : 100;
            volume = Mathf.Clamp(volume, 0, maxClamp);

            float normalizedVolume = volume / maxClamp; // System volume as a 0-1 range
            float warningVolume = 1 - normalizedVolume; // Inverse relation
            warningAudioSource.volume = Mathf.Clamp(warningVolume, 0.1f, 0.8f);
            soundAudioSource.volume = Mathf.Clamp(warningVolume, 0.1f, 0.8f);

            await LinuxCommand.RunAsync($"pactl set-sink-volume 0 {Mathf.Round(volume)}%");
            SaveManager.Load().system.audioVolume = volume;
        }

        /// <summary>
        /// Gets the system output volume
        /// </summary>
        /// <returns>System output volume in percentage</returns>
        public float GetSystemVolume()
        {
            return SaveManager.Load().system.audioVolume;
        }
    }

}

