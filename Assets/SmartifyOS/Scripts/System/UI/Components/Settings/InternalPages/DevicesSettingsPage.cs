using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class DevicesSettingsPage : BaseSettingsPage
    {
        [SerializeField] private ToggleButton autoPlayOnConnectToggle;


        private void Awake()
        {
            autoPlayOnConnectToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().system.autoplayOnConnect = value;
            };
        }

        private void Start()
        {
            autoPlayOnConnectToggle.SetToggle(SaveManager.Load().system.autoplayOnConnect);
        }
    }

}