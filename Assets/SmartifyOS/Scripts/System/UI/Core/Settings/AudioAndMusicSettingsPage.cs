using SmartifyOS.Audio;
using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class AudioAndMusicSettingsPage : BaseSettingsPage
    {
        [SerializeField] private ToggleButton autoPlayOnConnectToggle;
        [SerializeField] private ToggleButton allowOverAmplificationToggle;

        private void Awake()
        {
            autoPlayOnConnectToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().system.autoplayOnConnect = value;
            };

            allowOverAmplificationToggle.onValueChanged += async (value) =>
            {
                SaveManager.Load().system.allowOverAmplification = value;
                if (!value && SaveManager.Load().system.audioVolume > 100)
                {
                    await AudioManager.Instance.SetSystemVolume(100);
                }
            };
        }

        private void Start()
        {
            autoPlayOnConnectToggle.SetToggle(SaveManager.Load().system.autoplayOnConnect);
            allowOverAmplificationToggle.SetToggle(SaveManager.Load().system.allowOverAmplification);
        }
    }

}