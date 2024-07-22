using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class PopupsSettingsPage : BaseSettingsPage
    {
        [SerializeField] private ToggleButton autoCloseOnPowerOffToggle;
        [SerializeField] private ToggleButton allowModifyingWhileOnToggle;


        private void Awake()
        {
            autoCloseOnPowerOffToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().popups.autoCloseOnPowerOff = value;
            };

            allowModifyingWhileOnToggle.onValueChanged += (value) =>    
            {
                SaveManager.Load().popups.allowModifyingWhileOn = value;    
            };
        }

        private void Start()
        {
            autoCloseOnPowerOffToggle.SetToggle(SaveManager.Load().popups.autoCloseOnPowerOff);
            allowModifyingWhileOnToggle.SetToggle(SaveManager.Load().popups.allowModifyingWhileOn);
        }
    }

}