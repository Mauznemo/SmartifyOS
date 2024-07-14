using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace SmartifyOS.Settings
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class LinkButton : MonoBehaviour, IPointerClickHandler
    {
        public string text = "Settings Link Button";

        [SerializeField] private bool _interactable = true;

        [SerializeField] private BaseSettingsPage pageToOpen;
        public bool interactable
        {
            get => _interactable;
            set => SetInteractable(value);
        }

      

        [SerializeField] private Color normalColor = new Color(1, 1, 1, 0.133f);

        [SerializeField] private Color disabledColor = new Color(0, 0, 0, 0.31f);

        private Image image;


        [SerializeField] private TMP_Text label;
        [SerializeField] private Image icon;

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
            pageToOpen.Open();
        }

        private void SetInteractable(bool interactable)
        {
            _interactable = interactable;
            if (!_interactable)
            {
                image.color = disabledColor;
                label.SetColorIfNotNull(normalColor);
                icon.SetColorIfNotNull(normalColor);
            }
            else
            {
                image.color = normalColor;
                label.SetColorIfNotNull(Color.white);
                icon.SetColorIfNotNull(Color.white);
            }
        }
    }

}
