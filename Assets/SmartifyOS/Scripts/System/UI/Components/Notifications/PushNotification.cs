using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SmartifyOS.Notifications
{
    public class PushNotification : MonoBehaviour
    {
        [SerializeField] private Sprite infoIcon;
        [SerializeField] private Sprite warningIcon;
        [SerializeField] private Sprite errorIcon;

        [SerializeField] private Color infoColor;
        [SerializeField] private Color warningColor;
        [SerializeField] private Color errorColor;

        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;

        private UnityEngine.UI.Outline outline;
        private CanvasGroup canvasGroup;
        public void Init(NotificationType type, string text, float showTime)
        {
            outline = GetComponent<UnityEngine.UI.Outline>();
            canvasGroup = GetComponent<CanvasGroup>();

            this.text.text = text;
            LeanTween.alphaCanvas(canvasGroup, 1, 0.2f).setEaseInOutSine();

            switch (type)
            {
                case NotificationType.Info:
                    outline.effectColor = infoColor;
                    icon.sprite = infoIcon;
                    icon.color = infoColor;
                    break;
                case NotificationType.Warning:
                    icon.sprite = warningIcon;
                    icon.color = warningColor;
                    outline.effectColor = warningColor;
                    break;
                case NotificationType.Error:
                    icon.sprite = errorIcon;
                    icon.color = errorColor;
                    outline.effectColor = errorColor;
                    break;
            }

            Invoke(nameof(Delete), showTime);
        }

        private void Delete()
        {
            LeanTween.alphaCanvas(canvasGroup, 0, 0.2f).setEaseInOutSine();
            Destroy(gameObject, 0.2f);
        }
    }
}

