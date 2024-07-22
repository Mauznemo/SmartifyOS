using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class ArduinoPortsSettingsPage : BaseSettingsPage
    {
       [SerializeField] private InputField mainControllerInputField;
       [SerializeField] private InputField lightControllerInputField;
       [SerializeField] private InputField liveDataControllerInputField;
       [SerializeField] private InputField lockControllerInputField;


        private void Awake()
        {
            mainControllerInputField.onValueChanged += (value) => 
            {
                SaveManager.Load().mainController.arduinoPort = value;
            };

            lightControllerInputField.onValueChanged += (value) =>
            {
                SaveManager.Load().lightController.arduinoPort = value;
            };

            liveDataControllerInputField.onValueChanged += (value) =>
            {
                SaveManager.Load().liveController.arduinoPort = value;
            };

            lockControllerInputField.onValueChanged += (value) =>
            {
                SaveManager.Load().lockController.arduinoPort = value;
            };
            
        }

        private void Start()
        {
            mainControllerInputField.SetText(SaveManager.Load().mainController.arduinoPort);
            lightControllerInputField.SetText(SaveManager.Load().lightController.arduinoPort);
            liveDataControllerInputField.SetText(SaveManager.Load().liveController.arduinoPort);
            lockControllerInputField.SetText(SaveManager.Load().lockController.arduinoPort);
        }
    }

}