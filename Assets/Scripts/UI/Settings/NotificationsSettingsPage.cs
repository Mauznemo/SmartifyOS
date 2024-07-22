using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class NotificationsSettingsPage : BaseSettingsPage
    {
        [SerializeField] private ToggleButton doorWarningWhenDrivingToggle;
        [SerializeField] private ToggleButton trunkWarningWhenDrivingToggle;
        [SerializeField] private ToggleButton ignoreErrorsToggle;


        private void Awake()
        {
            doorWarningWhenDrivingToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().notifications.doorWarningWhenDriving = value;
            };

            trunkWarningWhenDrivingToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().notifications.trunkWarningWhenDriving = value;
            };

            ignoreErrorsToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().notifications.ignoreErrors = value;
            };
        }

        private void Start()
        {
            doorWarningWhenDrivingToggle.SetToggle(SaveManager.Load().notifications.doorWarningWhenDriving);
            trunkWarningWhenDrivingToggle.SetToggle(SaveManager.Load().notifications.trunkWarningWhenDriving);
            ignoreErrorsToggle.SetToggle(SaveManager.Load().notifications.ignoreErrors);
        }
    }

}