using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SmartifyOS.Audio;
using Unity.VisualScripting;
using UnityEngine;

public class ControlwheelManager : MonoBehaviour
{
    public static event Action<int> OnChanged;
    public static event Action OnButtonPressed;

    private static Mode mode = Mode.Audio;

    private bool buttonCooldown = false;

    private void Start()
    {
        MainController.OnControlwheelChanged += OnControlwheelChanged;
        MainController.OnControlwheelButton += OnControlwheelButton;
    }

    private void OnControlwheelButton(bool pressed)
    {
        if (buttonCooldown)
        {
            return;
        }

        if (pressed)
        {
            OnButtonPress();
        }

        StartCoroutine(StartButtonCooldown());
    }

    private IEnumerator StartButtonCooldown()
    {
        buttonCooldown = true;

        yield return new WaitForSeconds(0.3f);

        buttonCooldown = false;
    }

    private void OnControlwheelChanged(int dir)
    {
        switch (dir)
        {
            case 1:
                OnIncreased();
                break;
            case -1:
                OnDecreased();
                break;
        }
    }

    public static Mode GetMode()
    {
        return mode;
    }

    public static void SetDefaultMode()
    {
        SetMode(Mode.Audio);
    }

    public static void SetMode(Mode mode)
    {
        Debug.Log("Mode Changed: " + mode);
        ControlwheelManager.mode = mode;
    }

    private void OnButtonPress()
    {
        switch (mode)
        {
            case Mode.Audio:
                if (UIManager.Instance.IsWindowOpened<AppListUIWindow>())
                {
                    UIManager.Instance.HideUIWindow<AppListUIWindow>();
                }
                else
                {
                    UIManager.Instance.ShowUIWindow<AppListUIWindow>();
                }
                return;
        }

        OnButtonPressed?.Invoke();
    }

    private bool settingVolume = false;

    private async void OnIncreased()
    {
        OnChanged?.Invoke(1);
        switch (mode)
        {
            case Mode.Audio:
                if (settingVolume)
                    return;

                settingVolume = true;
                await AudioManager.Instance.SetSystemVolumeWithOverlay(GetVolume() + 5f);
                settingVolume = false;
                break;
        }
    }

    private async void OnDecreased()
    {
        OnChanged?.Invoke(-1);
        switch (mode)
        {
            case Mode.Audio:
                if (settingVolume)
                    return;

                settingVolume = true;
                await AudioManager.Instance.SetSystemVolumeWithOverlay(GetVolume() - 5f);
                settingVolume = false;
                break;
        }
    }

    private float GetVolume()
    {
        return AudioManager.Instance.GetSystemVolume();
    }

    public enum Mode
    {
        Audio,
        Ambient,
        AppList
    }
}
