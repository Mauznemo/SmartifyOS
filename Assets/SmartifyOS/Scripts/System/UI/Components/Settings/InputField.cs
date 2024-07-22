using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SmartifyOS.Settings
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class InputField : MonoBehaviour
    {
        public event Action<string> onValueChanged;

        [SerializeField] private string label = "Settings InputFiled";
        [SerializeField] private string placeholder = "Enter Text...";
        [SerializeField] private string text = "";

        [SerializeField] private bool _interactable = true;
        public bool interactable
        {
            get => _interactable;
            set => SetInteractable(value);
        }



        [SerializeField] private Color normalColor = new Color(1, 1, 1, 0.133f);

        [SerializeField] private Color disabledColor = new Color(0, 0, 0, 0.31f);

        private Image image;


        [SerializeField] private TMP_Text labelText;
        [SerializeField] private TMP_Text placeholderText;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_InputField inputField;

        private void Awake()
        {
            if (inputField != null)
                inputField.onValueChanged.AddListener((value) => onValueChanged?.Invoke(value));
        }

        private void OnEnable()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                labelText.SetIfNotNull(label);
                placeholderText.SetIfNotNull(placeholder);

                inputField.text = text;

                if (inputField != null)
                {
                    inputField.interactable = _interactable;
                }
            }
        }

        public void SetText(string text)
        {
            this.text = text;
            if (inputField != null)
            {
                inputField.text = text;
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
            if (inputField != null)
            {
                inputField.Select();
            }
        }

        private void SetInteractable(bool interactable)
        {
            _interactable = interactable;
            if (inputField != null)
            {
                inputField.interactable = _interactable;
            }
            if (!_interactable)
            {
                image.color = disabledColor;
                labelText.SetColorIfNotNull(normalColor);
                icon.SetColorIfNotNull(normalColor);
            }
            else
            {
                image.color = normalColor;
                labelText.SetColorIfNotNull(Color.white);
                icon.SetColorIfNotNull(Color.white);
            }
        }
    }
}


