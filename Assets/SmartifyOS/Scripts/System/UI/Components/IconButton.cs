using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace SmartifyOS.UI.Components
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class IconButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action onClick;

        public Sprite icon;

        [SerializeField] private bool _interactable = true;
        public bool interactable
        {
            get => _interactable;
            set => SetInteractable(value);
        }

        [SerializeField] private Color highTextColor = new Color(0.122f, 0.122f, 0.122f);

        [SerializeField] private Color highColor = new Color(1, 1, 1, 0.5f);

        [SerializeField] private Color offColor = new Color(1, 1, 1, 0.133f);

        [SerializeField] private Color disabledColor = new Color(0, 0, 0, 0.31f);

        private Image image;


        [SerializeField] private Image iconImage;

        private void OnEnable()
        {
            image = GetComponent<Image>();
        }

        public void SetIcon(Sprite icon)
        {
            this.icon = icon;
            iconImage.SetSpriteIfNotNull(icon);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                iconImage.SetSpriteIfNotNull(icon);
            }
        }

        private void OnValidate()
        {
            if (image == null) return;
            SetInteractable(_interactable);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable) return;
            onClick?.Invoke();
        }

        private void SetInteractable(bool interactable)
        {
            _interactable = interactable;
            if (!_interactable)
            {
                image.color = disabledColor;
                iconImage.SetColorIfNotNull(offColor);
            }
            else
            {
                UpdateUI(false);
            }
        }

        public void SetOffColor(Color color)
        {
            offColor = color;
        }

        private void UpdateUI(bool active)
        {
            if (active)
            {
                image.color = highColor;
                iconImage.SetColorIfNotNull(highTextColor);
            }
            else
            {
                image.color = offColor;
                iconImage.SetColorIfNotNull(Color.white);
            }
        }

        /// <summary>
        /// Triggers the onClick event without the button actually being pressed
        /// </summary>
        public void TriggerClick()
        {
            onClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable) return;
            UpdateUI(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable) return;
            UpdateUI(false);
        }
    }

}
