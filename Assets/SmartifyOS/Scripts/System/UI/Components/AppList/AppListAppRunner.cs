using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using UnityEngine;

namespace SmartifyOS.UI
{
    public class AppListAppRunner : MonoBehaviour
    {
        [SerializeField] private BaseUIWindow windowToOpen;

        private IconButton button;

        private void Awake()
        {
            button = GetComponent<IconButton>();
            button.onClick += () =>
            {
                UIManager.Instance.GetUIWindowInstance<AppListUIWindow>().Hide();
                UIManager.Instance.ShowUIWindow(windowToOpen);
            };
        }
    }
}


