using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class DateAndTimeSettingsPage : BaseSettingsPage
    {
        [SerializeField] private InputField hourOffsetInputField;
        [SerializeField] private InputField minuteOffsetInputField;

        private void Awake()
        {
            hourOffsetInputField.onValueChanged += (value) => 
            {
                SaveManager.Load().timezone.hourOffset = int.Parse(value);
            };
            minuteOffsetInputField.onValueChanged += (value) => 
            {
                SaveManager.Load().timezone.minuteOffset = int.Parse(value);
            };
        }

        private void Start()
        {
            hourOffsetInputField.SetText(SaveManager.Load().timezone.hourOffset.ToString("0"));
            minuteOffsetInputField.SetText(SaveManager.Load().timezone.minuteOffset.ToString("0"));
        }
    }

}