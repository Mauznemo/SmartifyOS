using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace SmartifyOS.StatusBar
{
    public class StatusBar : MonoBehaviour
    {
        public static StatusBar Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private GameObject statusEntryPrefab;
        [SerializeField] private Transform statusEntryParent;

        public static void SetTime(DateTime dateTime)
        {

        }

        public static StatusEntry AddStatus(Sprite sprite)
        {
            Debug.Log("AddStatus");
            StatusEntry statusEntry = new StatusEntry(sprite);
            return statusEntry;
        }

        private GameObject InstantiateStatusEntry(Sprite sprite)
        {
            var statusEntry = Instantiate(statusEntryPrefab, statusEntryParent);
            statusEntry.GetComponent<Image>().sprite = sprite;
            statusEntry.SetActive(true);
            return statusEntry;
        }

        public class StatusEntry
        {
            GameObject instance;

            public StatusEntry(Sprite sprite)
            {
                instance = Instance.InstantiateStatusEntry(sprite);
            }

            public void Show()
            {
                instance.SetActive(true);
            }

            public void Hide()
            {
                instance.SetActive(false);
            }

            public void Remove()
            {
                Destroy(instance);
            }
        }
    }

}


