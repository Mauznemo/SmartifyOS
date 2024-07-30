using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
namespace SmartifyOS.Notifications
{

    public class UpdateNotification : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        [SerializeField] private Button installButton;
        [SerializeField] private TMP_Text text;

        private CanvasGroup canvasGroup;

        private Vector2 startPos;
        private Vector2 offset;
        private Vector2 moveDir;

        private void Awake()
        {
            installButton.onClick += () =>
            {
                CancelInvoke(nameof(Delete));
                Delete();
                SystemManager.Instance.InstallUpdate();
            };
        }

        public void Init(string text, float showTime)
        {
            startPos = transform.position;

            canvasGroup = GetComponent<CanvasGroup>();

            this.text.text = text;
            LeanTween.alphaCanvas(canvasGroup, 1, 0.2f).setEaseInOutSine();


            Invoke(nameof(Delete), showTime);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = eventData.position - (Vector2)transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position - offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            CancelInvoke();
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInOutSine();
            Delete();

        }

        private void Delete()
        {
            LeanTween.alphaCanvas(canvasGroup, 0, 0.2f).setEaseInOutSine();
            Destroy(gameObject, 0.2f);
        }
    }
}