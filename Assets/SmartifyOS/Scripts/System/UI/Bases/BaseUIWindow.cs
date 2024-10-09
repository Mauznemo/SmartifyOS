using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.UI
{
    public abstract class BaseUIWindow : MonoBehaviour
    {
        /// <summary>Was the window open before it was closed by another window</summary>
        protected bool wasOpen;
        /// <summary>Is the window currently open</summary>
        protected bool isOpen;

        protected void Init()
        {
            UIManager.Instance.RegisterUIWindow(this);

            BaseUIManager.OnWindowOpened += UIManager_OnWindowOpened;
            BaseUIManager.OnWindowClosed += UIManager_OnWindowClosed;

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

        /// <summary>
        /// Called when any other window is opened
        /// </summary>
        /// <param name="window">The window that was opened</param>
        protected virtual void HandleWindowOpened(BaseUIWindow window) { }


        /// <summary>
        /// Called when any other window is closed
        /// </summary>
        /// <param name="window">The window that was closed</param>
        protected virtual void HandleWindowClosed(BaseUIWindow window) { }

        /// <summary>
        /// Called when the window is shown (opened)
        /// </summary>
        protected virtual void OnShow() { }

        /// <summary>
        /// Called when the window is hidden (closed)
        /// </summary>
        protected virtual void OnHide() { }


        /// <summary>
        /// Shows the window
        /// </summary>
        public void Show()
        {
            if (!Application.isPlaying)
            {
                transform.localScale = Vector3.one;
                return;
            }

            UIManager.Instance.AddOpenWindow(this);

            wasOpen = true;
            isOpen = true;

            //transform.localScale = Vector3.one;
            LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutCubic();

            OnShow();
        }

        /// <summary>
        /// Hides the window
        /// </summary>
        /// <param name="internalUpdate"><see cref="wasOpen"/> will not be reset if <see langword="true"/></param>
        public void Hide(bool internalUpdate = false)
        {
            if (!Application.isPlaying)
            {
                transform.localScale = Vector3.zero;
                return;
            }

            UIManager.Instance.RemoveOpenWindow(this);

            if (!internalUpdate)
                wasOpen = false;

            isOpen = false;

            //transform.localScale = Vector3.zero;
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInOutCubic();

            OnHide();
        }
    }
}


