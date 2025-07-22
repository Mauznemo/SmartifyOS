using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI;
using SmartifyOS.UI.MediaPlayer;
using UnityEngine;

public class CameraHightController : MonoBehaviour
{
    [SerializeField] private float height = 5f;

    private void Start()
    {
        UIManager.OnWindowShown += UIManager_OnWindowOpened;
        UIManager.OnWindowHidden += UIManager_OnWindowClosed;
    }

    private void UIManager_OnWindowOpened(BaseUIWindow window)
    {
        if (window.IsWindowOfType(typeof(FilePlayer), typeof(BluetoothPlayer)))
        {
            LeanTween.moveY(transform.gameObject, height, 0.5f);
        }
    }

    private void UIManager_OnWindowClosed(BaseUIWindow window)
    {
        if (window.IsWindowOfType(typeof(FilePlayer), typeof(BluetoothPlayer)))
        {
            if (UIManager.Instance.IsWindowVisible<FilePlayer>() || UIManager.Instance.IsWindowVisible<BluetoothPlayer>())
                return;

            LeanTween.moveY(transform.gameObject, 0, 0.5f);
        }
    }
}
