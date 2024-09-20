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
    private List<int> lastDirections = new List<int>();

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

            StartCoroutine(StartButtonCooldown());
        }
    }

    private IEnumerator StartButtonCooldown()
    {
        buttonCooldown = true;

        yield return new WaitForSeconds(0.1f);

        buttonCooldown = false;
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

    public static Mode GetMode()
    {
        return mode;
    }

    public static void SetDefaultMode()
    {
        ControlwheelManager.mode = Mode.Audio;
    }

    public static void SetMode(Mode mode)
    {
        ControlwheelManager.mode = mode;
    }

    private void OnButtonPress()
    {
        OnButtonPressed?.Invoke();
    }

    private async void OnIncreased()
    {
        OnChanged?.Invoke(1);
        switch (mode)
        {
            case Mode.Audio:
                await AudioManager.Instance.SetSystemVolumeWithOverlay(GetVolume() + 5f);
                break;
        }
    }

    private async void OnDecreased()
    {
        OnChanged?.Invoke(-1);
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
        Ambient
    }
}
