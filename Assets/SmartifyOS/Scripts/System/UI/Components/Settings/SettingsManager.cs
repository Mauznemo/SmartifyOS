using SmartifyOS.UI.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace SmartifyOS.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [SerializeField] protected TMP_Text titleText;
        [SerializeField] private List<BaseSettingsPage> pageHistory = new List<BaseSettingsPage>();
        [SerializeField] private List<BaseSettingsPage> settingsPages = new List<BaseSettingsPage>();

        [SerializeField] private IconButton backButton;
        [SerializeField] private IconButton closeButton;

        private SettingsUIWindow settingsUIWindow;

        private void Awake()
        {
            Instance = this;

            backButton.onClick += Back;
            closeButton.onClick += Close;

            settingsUIWindow = GetComponent<SettingsUIWindow>();
        }


        private void Start()
        {
            var pages = GetComponentsInChildren<BaseSettingsPage>();
            settingsPages = pages.ToList();
            foreach (var page in pages)
            {
                if(!page.disableOnStart)
                    continue;

                page.gameObject.SetActive(false);
            }
        }

        public void Close()
        {
            settingsUIWindow.Hide();
        }

        public void Open()
        {
            settingsUIWindow.Show();
        }

        public void Back()
        {
            if (pageHistory.Count < 2)
                return;
            pageHistory[pageHistory.Count - 1].gameObject.SetActive(false);
            pageHistory[pageHistory.Count - 2].gameObject.SetActive(true);
            titleText.text = pageHistory[pageHistory.Count - 2].pageName;
            pageHistory.RemoveAt(pageHistory.Count - 1);
        }

        public void ShowSettingsPage<T>() where T : BaseSettingsPage
        {
            BaseSettingsPage page = settingsPages.OfType<T>().FirstOrDefault();

            if (page != null)
            {
                Open();
                page.Open();
            }
            else
            {
                Debug.LogError("Page has no instance");
            }
        }


        public void OpenPage(BaseSettingsPage page)
        {
            page.gameObject.SetActive(true);
            titleText.text = page.pageName;

            if (pageHistory.Count > 0)
            {
                pageHistory[pageHistory.Count - 1].gameObject.SetActive(false);
            }

            pageHistory.Add(page);
        }
    }
}


