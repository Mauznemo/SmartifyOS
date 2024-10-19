using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using SmartifyOS.SaveSystem;
using UnityEngine;
using UnityEngine.UI;

public class VolumeOverlay : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float sliderMultiplier = 1f;

    private CanvasGroup canvasGroup;
    private bool isActive = false;
    private float targetVolume;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        AudioManager.OnVolumeChangedOverlay += OnVolumeChanged;

        float multiplier = SaveManager.Load().system.allowOverAmplification ? 150 : 100;
        volumeSlider.value = AudioManager.Instance.GetSystemVolume() / multiplier;
    }

    private void OnVolumeChanged(float volume)
    {
        CancelInvoke(nameof(HideOverlay));
        float multiplier = SaveManager.Load().system.allowOverAmplification ? 150 : 100;
        targetVolume = volume / multiplier;
        ShowOverlay();
        Invoke(nameof(HideOverlay), 5f);
    }

    private void Update()
    {
        if (isActive && targetVolume != volumeSlider.value)
        {
            float volume = Mathf.Lerp(volumeSlider.value, targetVolume, Time.deltaTime * sliderMultiplier);
            volumeSlider.value = volume;
        }
    }

    private void ShowOverlay()
    {
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutSine();
        LeanTween.alphaCanvas(canvasGroup, 1, 0.2f).setEaseInOutSine();

        isActive = true;
    }

    private void HideOverlay()
    {
        LeanTween.alphaCanvas(canvasGroup, 0, 0.2f).setEaseInOutSine();
        LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInOutSine();

        isActive = false;
    }
}
