using System.Collections;
using System.Collections.Generic;
using SmartifyOS.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessQuickSettingsSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private CanvasGroup darkOverlay;

    private void Awake()
    {
        slider.onValueChanged.AddListener((value) =>
        {
            darkOverlay.alpha = 1 - value;
        });
    }

    private void Start()
    {
        slider.value = SaveManager.Load().system.brightness;
        darkOverlay.alpha = 1 - slider.value;
    }
}
