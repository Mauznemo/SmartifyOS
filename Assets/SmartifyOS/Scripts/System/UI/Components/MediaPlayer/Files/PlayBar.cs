using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SmartifyOS.UI.MediaPlayer
{
    public class PlayBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<float> OnValueChanged;

        private bool isDragging = false;

        private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
        }

        public void SetValue(float value)
        {
            if (isDragging)
                return;

            slider.value = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnValueChanged?.Invoke(slider.value);
            isDragging = false;
        }
    }
}