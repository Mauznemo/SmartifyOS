using System.Collections;
using System.Collections.Generic;
using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS
{
    public class SystemManager : MonoBehaviour
    {
        [SerializeField] private bool saveButton;

        private void OnApplicationQuit()
        {
            SaveManager.Save();
        }

        private void Update()
        {
            if (saveButton)
            {
                saveButton = false;
                SaveManager.Save();
            }
        }
    }
}


