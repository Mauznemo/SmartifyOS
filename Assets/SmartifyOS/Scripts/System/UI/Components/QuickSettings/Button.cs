using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace SmartifyOS.QuickSettings
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action onClick;

        public string title = "Quick Settings button";

        [SerializeField] private Sprite iconSource;

        [SerializeField] private Color highTextColor = new Color(0.122f, 0.122f, 0.122f);

        [SerializeField] private Color highColor = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Color offColor = new Color(1, 1, 1, 0.133f);

        private Image image;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text titleText;

        private void OnEnable()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                titleText.SetIfNotNull(title);
                iconImage.SetSpriteIfNotNull(iconSource);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        private void UpdateUI(bool active)
        {
            if (active)
            {
                image.color = highColor;
                titleText.SetColorIfNotNull(highTextColor);
                iconImage.SetColorIfNotNull(highTextColor);
            }
            else
            {
                image.color = offColor;
                titleText.SetColorIfNotNull(Color.white);
                iconImage.SetColorIfNotNull(Color.white);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateUI(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UpdateUI(false);
        }
    }

}
