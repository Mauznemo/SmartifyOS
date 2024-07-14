using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private ModalWindow modalWindowPrefab;
        [SerializeField] private List<BaseUIWindow> openWindows = new List<BaseUIWindow>();

        public static event Action<BaseUIWindow> OnWindowOpened;
        public static event Action<BaseUIWindow> OnWindowClosed;

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

