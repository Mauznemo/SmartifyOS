using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class InteriorLightingSettingsPage : BaseSettingsPage
    {
        [SerializeField] private ToggleButton activateLightOnDoorOpenToggle;
        [SerializeField] private ToggleButton activateLedStripOnDoorOpenToggle;

        private void Awake()
        {
            activateLightOnDoorOpenToggle.onValueChanged += (value) => 
            {
                SaveManager.Load().interiorLighting.activateLightOnDoorOpen = value;
            };

            activateLedStripOnDoorOpenToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().interiorLighting.activateLedStripOnDoorOpen = value;
            };
        }

        private void Start()
        {
            activateLightOnDoorOpenToggle.SetToggle(SaveManager.Load().interiorLighting.activateLightOnDoorOpen);
            activateLedStripOnDoorOpenToggle.SetToggle(SaveManager.Load().interiorLighting.activateLedStripOnDoorOpen);
        }
    }

}