using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class CameraSettingsPage : BaseSettingsPage
    {
        [SerializeField] private ToggleButton autoFullScreenToggle;
        [SerializeField] private InputField indexInputField;


        private void Awake()
        {
            autoFullScreenToggle.onValueChanged += (value) =>
            {
                SaveManager.Load().camera.autoFullscreen = value;
            };

            indexInputField.onValueChanged += (value) =>
            {
                SaveManager.Load().camera.currentCameraIndex = int.Parse(value);
            };
        }

        private void Start()
        {
            autoFullScreenToggle.SetToggle(SaveManager.Load().camera.autoFullscreen);
            indexInputField.SetText(SaveManager.Load().camera.currentCameraIndex.ToString());
        }
    }

}