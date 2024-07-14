using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI.Components;
using UnityEngine;

public class AppListOpen : MonoBehaviour
{
    private IconButton button;

    private void Awake()
    {
        button = GetComponent<IconButton>();
        button.onClick += () => { UIManager.Instance.ShowUIWindow<AppListUIWindow>(); };
    }
}
