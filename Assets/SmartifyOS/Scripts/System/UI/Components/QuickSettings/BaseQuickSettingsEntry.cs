using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.QuickSettings
{
    public abstract class BaseQuickSettingsEntry : MonoBehaviour
    {
        private Button button;
        private ToggleButton toggleButton;
        protected EntryType entryType = EntryType.None;

        protected void Init()
        {
            if(gameObject.TryGetComponent(out button))
            {
                button.onClick += OnClick;
                entryType = EntryType.Button;
            }
            else if(gameObject.TryGetComponent(out toggleButton))
            {
                toggleButton.onValueChanged += OnToggleValueChanged;
                entryType = EntryType.Toggle;
            }

            if (entryType == EntryType.None)
            {
                Debug.LogError("Please Add a Quick Settings Toggle or Button to to this gameobject", gameObject);
            }
        }

        protected virtual void OnClick()
        { }

        protected virtual void OnToggleValueChanged(bool isOn)
        { }

        protected void SetToggle(bool isOn)
        {
            if(toggleButton == null)
            {
                throw new System.Exception("Toggle Button not found");
            }
            toggleButton.SetToggle(isOn);
        }

        protected enum EntryType
        {
            None,
            Button,
            Toggle
        }
    }
}


