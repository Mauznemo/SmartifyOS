using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmartifyOS.UI
{
    public class BaseUIManager : MonoBehaviour
    {
        [SerializeField] protected ModalWindow modalWindowPrefab;
        [SerializeField] protected List<BaseUIWindow> visibleWindows = new List<BaseUIWindow>();
        [SerializeField] protected BottomBar bottomBar;
        [SerializeField] private Transform topLevelElement;

        [SerializeField] protected List<BaseUIWindow> windowInstances = new List<BaseUIWindow>();

        public static event Action<BaseUIWindow> OnWindowShown;
        public static event Action<BaseUIWindow> OnWindowHidden;

        public bool IsWindowOpened<T>() where T : BaseUIWindow
        {
            return visibleWindows.OfType<T>().Any();
        }

        public T GetWindowInstance<T>() where T : BaseUIWindow
        {
            T window = windowInstances.OfType<T>().FirstOrDefault();

            if (window != null)
            {
                return window;
            }
            else
            {
                Debug.LogError("Window has no instance");
                return null;
            }
        }

        /// <summary>
        /// Show a window
        /// </summary>
        /// <param name="window">Window instance to show</param>
        /// <param name="showAction">Show action to perform</param>
        public void ShowWindow(BaseUIWindow window, ShowAction showAction = ShowAction.OpenOnTop)
        {

            if (window != null)
            {
                if (showAction == ShowAction.OpenSingle)
                    HideAllWindows();
                window.Show(showAction);
            }
            else
            {
                Debug.LogError("Window has no instance");
            }
        }

        /// <summary>
        /// Show a window
        /// </summary>
        /// <typeparam name="T">Window type to show</typeparam>
        /// <param name="showAction">Show action to perform</param>
        public void ShowWindow<T>(ShowAction showAction = ShowAction.OpenOnTop) where T : BaseUIWindow
        {
            BaseUIWindow window = windowInstances.OfType<T>().FirstOrDefault();

            if (window != null)
            {
                if (showAction == ShowAction.OpenSingle)
                    HideAllWindows();
                window.Show(showAction);
            }
            else
            {
                Debug.LogError("Window has no instance");
            }
        }

        /// <summary>
        /// Hide a window that is currently visible
        /// </summary>
        /// <typeparam name="T">Window type to hide</typeparam>
        public void HideWindow<T>() where T : BaseUIWindow
        {
            BaseUIWindow window = windowInstances.OfType<T>().FirstOrDefault();

            if (window != null)
            {
                window.Hide();
            }
            else
            {
                Debug.LogError("Window has no instance");
            }
        }

        public int GetTopLevelSiblingIndex()
        {
            return topLevelElement.GetSiblingIndex() - 1;
        }

        /// <summary>
        /// Register the instance of a window
        /// </summary>
        public void RegisterWindowInstance(BaseUIWindow window)
        {
            if (windowInstances.Contains(window))
                return;

            windowInstances.Add(window);
        }

        /// <summary>
        /// Register the window as shown
        /// </summary>
        public void RegisterShownWindow(BaseUIWindow window)
        {
            if (visibleWindows.Contains(window))
            {
                Debug.Log("Window already open");
                return;
            }

            visibleWindows.Add(window);
            OnWindowShown?.Invoke(window);

            UpdateBottomBarVisibility();
        }

        /// <summary>
        /// Register the window as hidden
        /// </summary>
        public void RegisterHiddenWindow(BaseUIWindow window)
        {
            visibleWindows.Remove(window);
            OnWindowHidden?.Invoke(window);

            UpdateBottomBarVisibility();
        }

        /// <summary>
        /// Create a new modal window
        /// </summary>
        /// <returns>The created <see cref="ModalWindow"/> (unconfigured)</returns>
        public ModalWindow CreateModal()
        {
            ModalWindow modalWindow = Instantiate(modalWindowPrefab, transform);

            modalWindow.gameObject.SetActive(true);

            return modalWindow;
        }

        protected void HideAllWindows()
        {
            foreach (BaseUIWindow window in visibleWindows.ToList())
            {
                window.Hide();
            }
        }

        protected void UpdateBottomBarVisibility()
        {
            if (CheckBottomBarVisibility())
            {
                bottomBar.Show();
            }
            else
            {
                bottomBar.Hide();
            }
        }

        /// <summary>Checks if the bottom bar should be visible</summary>
        protected bool CheckBottomBarVisibility()
        {
            foreach (BaseUIWindow window in visibleWindows.ToList())
            {
                if (window.hidesBottomUI)
                    return false;
            }

            return true;
        }
    }
    /// <summary>
    /// Action to perform when showing a window
    /// </summary>
    public enum ShowAction
    {
        /// <summary>Show the window on top of all open windows</summary>
        OpenOnTop,
        /// <summary>Close all other windows and show the window</summary>
        OpenSingle,
        /// <summary>Show the window behind all other windows</summary>
        OpenInBackground,
        /// <summary>Show the window and don't change the z-index</summary>
        OpenInPlace
    }
}

