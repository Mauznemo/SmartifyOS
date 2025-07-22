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
        [SerializeField] private ShowAction showAction;

        private IconButton button;

        private void Awake()
        {
            button = GetComponent<IconButton>();
            button.onClick += () =>
            {
                UIManager.Instance.GetWindowInstance<AppListUIWindow>().Hide();
                UIManager.Instance.ShowWindow(windowToOpen, showAction);
            };
        }
    }
}


