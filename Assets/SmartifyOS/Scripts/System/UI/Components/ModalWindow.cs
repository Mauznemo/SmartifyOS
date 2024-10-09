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

        /// <summary>
        /// Creates a new <see cref="ModalWindow"/>
        /// </summary>
        /// <returns>The created <see cref="ModalWindow"/></returns>
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

        /// <summary>
        /// Initializes the <see cref="ModalWindow"/>
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="content">Text of the window</param>
        /// <param name="modalType">What type of modal this is</param>
        /// <param name="confirm">Callback when the confirm button is pressed</param>
        /// <param name="cancel">Callback when the cancel button is pressed</param>
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

        /// <summary>
        /// Initializes the <see cref="ModalWindow"/> with custom button texts
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="content">Text of the window</param>
        /// <param name="confirmText">Confirm button text</param>
        /// <param name="cancelText">Cancel button text</param>
        /// <param name="confirm">Callback when the confirm button is pressed</param>
        /// <param name="cancel">Callback when the cancel button is pressed</param>
        public void Init(string title, string content, string confirmText, string cancelText, Action confirm, Action cancel)
        {
            titleText.text = title;
            contentText.text = content;
            CreateButton(confirmText, () =>
            {
                Close();
                confirm();
            });

            CreateButton(cancelText, () =>
            {
                Close();
                cancel();
            });

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Updates the content of the <see cref="ModalWindow"/>
        /// </summary>
        /// <param name="content">New content string</param>
        public void UpdateContent(string content)
        {
            contentText.text = content;
        }

        /// <summary>
        /// Closes the <see cref="ModalWindow"/>
        /// </summary>
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

