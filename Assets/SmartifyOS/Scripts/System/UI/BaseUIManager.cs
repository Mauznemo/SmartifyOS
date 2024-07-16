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
        [SerializeField] protected List<BaseUIWindow> openWindows = new List<BaseUIWindow>();

        [SerializeField] protected List<BaseUIWindow> windowInstances = new List<BaseUIWindow>();

        public static event Action<BaseUIWindow> OnWindowOpened;
        public static event Action<BaseUIWindow> OnWindowClosed;

        public T GetUIWindowInstance<T>() where T : BaseUIWindow
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

        public void ShowUIWindow(BaseUIWindow window)
        {
            if (window != null)
            {
                window.Show();
            }
            else
            {
                Debug.LogError("Window has no instance");
            }
        }

        public void ShowUIWindow<T>() where T : BaseUIWindow
        {
            BaseUIWindow window = windowInstances.OfType<T>().FirstOrDefault();

            if (window != null)
            {
                window.Show();
            }
            else
            {
                Debug.LogError("Window has no instance");
            }
        }

        public void RegisterUIWindow(BaseUIWindow window)
        {
            if (windowInstances.Contains(window))
                return;

            windowInstances.Add(window);
        }

        public void AddOpenWindow(BaseUIWindow window)
        {
            if (openWindows.Contains(window))
            {
                Debug.Log("Window already open");
                return;
            }

            openWindows.Add(window);
            OnWindowOpened?.Invoke(window);
        }

        public void RemoveOpenWindow(BaseUIWindow window)
        {
            openWindows.Remove(window);
            OnWindowClosed?.Invoke(window);
        }

        public ModalWindow CreateModal()
        {
            ModalWindow modalWindow = Instantiate(modalWindowPrefab, transform);

            modalWindow.gameObject.SetActive(true);

            return modalWindow;
        }
    }
}

