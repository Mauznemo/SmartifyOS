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
    public class ToggleButton : MonoBehaviour, IPointerClickHandler
    {
        public event Action<bool> onValueChanged;

        public string title = "Quick Settings Toggle";

        public string stateTextTrue = "ON";
        public string stateTextFalse = "OFF";

        [SerializeField] private Sprite iconSource;
        [SerializeField] private Sprite iconSourceFalse;

        [SerializeField] private bool isOn = false;

        [SerializeField] private Color onTextColor = new Color(0.122f, 0.122f, 0.122f);

        [SerializeField] private Color onColor = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Color offColor = new Color(1, 1, 1, 0.133f);

        private Image image;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text titleText;

        public void SetToggle(bool isOn)
        {
            this.isOn = isOn;
            UpdateUI();
        }

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

                UpdateUI();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            isOn = !isOn;

            onValueChanged?.Invoke(isOn);

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (isOn)
            {
                image.color = onColor;
                stateText.SetIfNotNull(stateTextTrue);
                stateText.SetColorIfNotNull(onTextColor);
                titleText.SetColorIfNotNull(onTextColor);
                iconImage.SetColorIfNotNull(onTextColor);
                iconImage.SetSpriteIfNotNull(iconSource);
            }
            else
            {
                image.color = offColor;
                stateText.SetIfNotNull(stateTextFalse);
                stateText.SetColorIfNotNull(Color.white);
                titleText.SetColorIfNotNull(Color.white);
                iconImage.SetColorIfNotNull(Color.white);
                iconImage.SetSpriteIfNotNull(iconSourceFalse);
            }
        }
    }

}
