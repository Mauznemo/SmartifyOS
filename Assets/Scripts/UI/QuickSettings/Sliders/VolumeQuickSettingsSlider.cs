using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using SmartifyOS.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class VolumeQuickSettingsSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(async (value) =>
        {
            await AudioManager.Instance.SetSystemVolume(value * 100);
        });
    }

    private void Start()
    {
        slider.value = SaveManager.Load().system.audioVolume / 100;
    }
}
