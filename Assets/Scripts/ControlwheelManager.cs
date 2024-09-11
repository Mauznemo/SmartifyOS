using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using UnityEngine;

public class ControlwheelManager : MonoBehaviour
{
    private Mode mode = Mode.Audio;

    private void Start()
    {
        MainController.OnControlwheelChanged += OnControlwheelChanged;
        MainController.OnControlwheelButton += OnControlwheelButton;
    }

    private void OnControlwheelButton(bool pressed)
    {

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

    private async void OnIncreased()
    {
        switch (mode)
        {
            case Mode.Audio:
                await AudioManager.Instance.SetSystemVolumeWithOverlay(GetVolume() + 0.1f);
                break;
        }
    }

    private async void OnDecreased()
    {
        switch (mode)
        {
            case Mode.Audio:
                await AudioManager.Instance.SetSystemVolumeWithOverlay(GetVolume() - 0.1f);
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
    }
}
