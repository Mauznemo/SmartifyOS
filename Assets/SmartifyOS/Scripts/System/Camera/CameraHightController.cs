using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI;
using SmartifyOS.UI.MediaPlayer;
using UnityEngine;

public class CameraHightController : MonoBehaviour
{
    [SerializeField] private float height = 5f;
    [SerializeField] private FreeLookInput freeLookInput;

    private void Start()
    {
        UIManager.OnWindowOpened += UIManager_OnWindowOpened;
        UIManager.OnWindowClosed += UIManager_OnWindowClosed;
    }

    private void UIManager_OnWindowOpened(BaseUIWindow window)
    {
        if (window.IsWindowOfType(typeof(FilePlayer), typeof(BluetoothPlayer)))
        {
            LeanTween.moveY(transform.gameObject, height, 0.5f);
            freeLookInput.SetOrbitRadius(10f, 1);
        }
    }

    private void UIManager_OnWindowClosed(BaseUIWindow window)
    {
        if (window.IsWindowOfType(typeof(FilePlayer), typeof(BluetoothPlayer)))
        {
            if (UIManager.Instance.IsWindowOpened<FilePlayer>() || UIManager.Instance.IsWindowOpened<BluetoothPlayer>())
                return;

            LeanTween.moveY(transform.gameObject, 0, 0.5f);
            freeLookInput.SetOrbitRadius(7.5f, 1);
        }
    }
}
