using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
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

        volumeSlider.value = AudioManager.Instance.GetSystemVolume() / 100f;
    }

    private void OnVolumeChanged(float volume)
    {
        CancelInvoke(nameof(HideOverlay));
        targetVolume = volume / 100f;
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
