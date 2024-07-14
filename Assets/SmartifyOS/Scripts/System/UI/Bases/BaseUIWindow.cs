using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.UI
{
    public abstract class BaseUIWindow : MonoBehaviour
    {
        protected bool wasOpen;

        protected void Init()
        {
            UIManager.OnWindowOpened += UIManager_OnWindowOpened;
            UIManager.OnWindowClosed += UIManager_OnWindowClosed;

            transform.localScale = Vector3.zero;
        }

        private void UIManager_OnWindowClosed(BaseUIWindow obj)
        {
            if (obj == this)
                return;

            HandleWindowClosed(obj);
        }

        private void UIManager_OnWindowOpened(BaseUIWindow obj)
        {
            if (obj == this)
                return;

            HandleWindowOpened(obj);
        }

        protected virtual void HandleWindowOpened(BaseUIWindow window) { }

        protected virtual void HandleWindowClosed(BaseUIWindow window) { }

        protected virtual void OnShow() { }

        protected virtual void OnHide() { }


        public void Show()
        {
            if(!Application.isPlaying)
            {
                transform.localScale = Vector3.one;
                return;
            }

            UIManager.Instance.AddOpenWindow(this);

            wasOpen = true;

            transform.localScale = Vector3.one;

            OnShow();
        }

        public void Hide(bool internalUpdate = false)
        {
            if(!Application.isPlaying)
            {
                transform.localScale = Vector3.zero;
                return;
            }

            UIManager.Instance.RemoveOpenWindow(this);

            if (!internalUpdate)
                wasOpen = false;

            transform.localScale = Vector3.zero;

            OnHide();
        }
    }
}


