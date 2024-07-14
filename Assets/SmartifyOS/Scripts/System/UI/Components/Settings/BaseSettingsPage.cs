using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public abstract class BaseSettingsPage : MonoBehaviour
    { 
        public bool disableOnStart = true;

        public string pageName = "New Page";


        public void Open()
        {
            SettingsManager.Instance.OpenPage(this);
            OnOpened();
        }

        protected virtual void OnOpened() 
        { }
    }
}


