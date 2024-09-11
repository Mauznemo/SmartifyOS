using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using UnityEngine;
using UnityEngine.UI;

public class VolumeOverlay : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        AudioManager.OnVolumeChangedOverlay += OnVolumeChanged;
    }

    private void OnVolumeChanged(float volume)
    {
        CancelInvoke(nameof(HideOverlay));
        volumeSlider.value = volume;
        ShowOverlay();
        Invoke(nameof(HideOverlay), 5f);
    }

    private void ShowOverlay()
    {
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutSine();
        LeanTween.alphaCanvas(canvasGroup, 1, 0.2f).setEaseInOutSine();
    }

    private void HideOverlay()
    {
        LeanTween.alphaCanvas(canvasGroup, 0, 0.2f).setEaseInOutSine();
        LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInOutSine();
    }
}
