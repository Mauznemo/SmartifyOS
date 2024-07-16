using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
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
            await AudioManager.Instance.SetSystemVolume(volume * 100);
            slider.interactable = true;
        });
    }
}
