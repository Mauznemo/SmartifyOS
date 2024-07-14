using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartifyOS.UI.Components;
using TMPro;
using System;

namespace SmartifyOS.UI
{
    public class ModalWindow : MonoBehaviour
    {

        public static ModalWindow Create()
        {
            if (UIManager.Instance == null)
                throw new Exception("UIManager is not initialized.");

            return UIManager.Instance.CreateModal();
        }

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text contentText;

        [SerializeField] private Transform buttonParent;
        [SerializeField] private Button buttonPrefab;

        public void Init(string title, string content, ModalType modalType, Action confirm, Action cancel)
        {
            titleText.text = title;
            contentText.text = content;

            switch (modalType)
            {
                case ModalType.Okay:
                    CreateButton("Okay", () =>
                    {
                        Close();
                        confirm();
                    });
                    break;
                case ModalType.OkayCancel:
                    CreateButton("Okay", () =>
                    {
                        Close();
                        confirm();
                    });
                    CreateButton("Cancel", () =>
                    {
                        Close();
                        cancel();
                    });
                    break;
                case ModalType.YesNo:
                    CreateButton("Yes", () =>
                    {
                        Close();
                        confirm();
                    });
                    CreateButton("No", () =>
                    {
                        Close();
                        cancel();
                    });
                    break;
                case ModalType.YesCancel:
                    CreateButton("Yes", () =>
                    {
                        Close();
                        confirm();
                    });
                    CreateButton("Cancel", () =>
                    {
                        Close();
                        cancel();
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modalType), modalType, null);
            }

            gameObject.SetActive(true);
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        private Button CreateButton(string text, Action onClick)
        {
            Button button = Instantiate(buttonPrefab, buttonParent);
            button.gameObject.SetActive(true);
            button.onClick += onClick;
            button.text = text;
            return button;
        }

        public enum ModalType
        {
            Okay,
            OkayCancel,
            YesNo,
            YesCancel
        }
    }
}

