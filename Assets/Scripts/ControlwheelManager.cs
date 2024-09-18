using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SmartifyOS.Audio;
using Unity.VisualScripting;
using UnityEngine;

public class ControlwheelManager : MonoBehaviour
{
    private Mode mode = Mode.Audio;
    private List<int> lastDirections = new List<int>();

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
        lastDirections.Add(dir);

        if (lastDirections.Count > 4)
        {
            lastDirections.RemoveAt(0);
        }

        switch (GetCurrentDirection())
        {
            case 1:
                OnIncreased();
                break;
            case -1:
                OnDecreased();
                break;
        }
    }

    //The Rotary Encoder sometimes glitches a bit and sends a wrong direction at the end or in between, so is to prevent that.
    private int GetCurrentDirection()
    {
        int countNegative = lastDirections.Count(x => x == -1);
        int countPositive = lastDirections.Count(x => x == 1);

        if (countNegative > countPositive)
        {
            return -1;
        }
        else if (countPositive > countNegative)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private async void OnIncreased()
    {
        switch (mode)
        {
            case Mode.Audio:
                await AudioManager.Instance.SetSystemVolumeWithOverlay(GetVolume() + 5f);
                break;
        }
    }

    private async void OnDecreased()
    {
        switch (mode)
        {
            case Mode.Audio:
                await AudioManager.Instance.SetSystemVolumeWithOverlay(GetVolume() - 5f);
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
