using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI.Components;
using UnityEngine;

namespace SmartifyOS.StatusBar
{
    public class BottomButtons : MonoBehaviour
    {
        [SerializeField] private IconButton settingsButton;
        [SerializeField] private IconButton powerButton;

        [SerializeField]
        private StatusBarDrag statusBarDrag;

        private void Awake()
        {
            settingsButton.onClick += () => { UIManager.Instance.ShowWindow<SettingsUIWindow>(); statusBarDrag.MoveUp(); };
            powerButton.onClick += () => { UIManager.Instance.ShowWindow<PowerOffUIWindow>(UI.ShowAction.OpenInPlace); };
        }
    }
}


