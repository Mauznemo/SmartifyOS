using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using SmartifyOS.SaveSystem;
using SmartifyOS.StatusBar;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSlider : MonoBehaviour
{
    private StatusBarDrag statusBarDrag;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        slider.onValueChanged.AddListener(async (volume) =>
        {
            slider.interactable = false;
            float multiplier = SaveManager.Load().system.allowOverAmplification ? 150 : 100;
            await AudioManager.Instance.SetSystemVolume(volume * multiplier);
            slider.interactable = true;
        });
    }

    private void Start()
    {
        float multiplier = SaveManager.Load().system.allowOverAmplification ? 150 : 100;
        slider.value = SaveManager.Load().system.audioVolume / multiplier;

        statusBarDrag = GetComponentInParent<StatusBarDrag>();
        statusBarDrag.OnQuickSettingsOpened += OnQuickSettingsOpened;
    }

    private void OnQuickSettingsOpened()
    {
        float multiplier = SaveManager.Load().system.allowOverAmplification ? 150 : 100;
        slider.value = AudioManager.Instance.GetSystemVolume() / multiplier;
    }
}
