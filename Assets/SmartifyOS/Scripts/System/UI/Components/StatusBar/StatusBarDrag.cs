using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SmartifyOS.StatusBar
{
    public class StatusBarDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private float upperSnap;
        private float lowerSnap;

        private Vector2 lastPos;
        private Vector2 offset;
        private RectTransform rectTransform;

        private bool movedUp = true;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            upperSnap = rectTransform.anchoredPosition.y;
            lowerSnap = rectTransform.sizeDelta.y * -0.5f;

            lastPos = rectTransform.position;

            movedUp = true;
        }

        public void MoveUp()
        {
            LeanTween.moveY(rectTransform, upperSnap, 0.23f).setEaseInOutCubic();
            movedUp = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = eventData.position - (Vector2)rectTransform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.position = new Vector2(rectTransform.position.x, eventData.position.y - offset.y);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(Mathf.Abs((lastPos.y + offset.y) - eventData.position.y) > 50 && movedUp)
            {
                //rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, lowerSnap);
                lastPos = rectTransform.position;
                LeanTween.moveY(rectTransform, lowerSnap, 0.23f).setEaseInOutCubic();
                movedUp = false;
            }
            else if(Mathf.Abs((lastPos.y + offset.y) - eventData.position.y) > 50 && !movedUp)
            {
                //rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, upperSnap);
                lastPos = rectTransform.position;
                LeanTween.moveY(rectTransform, upperSnap, 0.23f).setEaseInOutCubic();
                movedUp = true;
            }
            else
            {
                rectTransform.position = lastPos;
            }
        }
    }

}
