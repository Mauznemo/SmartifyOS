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
    public class Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action onClick;

        public string text = "Button";

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


        [SerializeField] private TMP_Text label;

        private void OnEnable()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                label.SetIfNotNull(text);
            }
        }

        private void OnValidate()
        {
            if (image == null) return;
            SetInteractable(_interactable);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!_interactable) return;
            onClick?.Invoke();
        }

        private void SetInteractable(bool interactable)
        {
            _interactable = interactable;
            if (!_interactable)
            {
                image.color = disabledColor;
                label.SetColorIfNotNull(offColor);
            }
            else
            {
                UpdateUI(false);
            }
        }

        private void UpdateUI(bool active)
        {
            if (active)
            {
                image.color = highColor;
                label.SetColorIfNotNull(highTextColor);
            }
            else
            {
                image.color = offColor;
                label.SetColorIfNotNull(Color.white);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!_interactable) return;
            UpdateUI(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable) return;
            UpdateUI(false);
        }
    }

}
