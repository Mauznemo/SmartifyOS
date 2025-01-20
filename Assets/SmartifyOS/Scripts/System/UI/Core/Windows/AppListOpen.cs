using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI.Components;
using Unity.VisualScripting;
using UnityEngine;

public class AppListOpen : MonoBehaviour
{
    private IconButton button;

    private void Awake()
    {
        button = GetComponent<IconButton>();
        button.onClick += () =>
        {
            if (UIManager.Instance.IsWindowOpened<AppListUIWindow>())
                UIManager.Instance.HideUIWindow<AppListUIWindow>();
            else
                UIManager.Instance.ShowUIWindow<AppListUIWindow>();
        };
    }
}
