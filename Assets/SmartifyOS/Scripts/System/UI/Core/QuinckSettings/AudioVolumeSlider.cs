using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using SmartifyOS.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSlider : MonoBehaviour
{
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
    }
}
