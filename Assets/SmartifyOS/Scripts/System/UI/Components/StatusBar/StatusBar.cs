using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using SmartifyOS.SaveSystem;
using TMPro;
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

        [SerializeField] private TMP_Text timeText;

        public static bool isTimeSet { get; private set; }

        private void Start()
        {
            InvokeRepeating(nameof(UpdateTime), 0f, 5f);
        }

        /// <summary>
        /// Sets the system time and date
        /// </summary>
        /// <param name="dateTime">DateTime to set to</param>
        public static void SetSystemTime(DateTime dateTime)
        {
            Instance.SetTime(dateTime);
        }

        private async void SetTime(DateTime dateTime)
        {
            int hourOffset = SaveManager.Load().timezone.hourOffset;
            int minuteOffset = SaveManager.Load().timezone.minuteOffset;

            dateTime = dateTime.AddOffset(hourOffset, minuteOffset);

            string setDateAndTimeCommand = $"sudo date -s \\\"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}\\\"";
            await LinuxCommand.RunAsync(setDateAndTimeCommand);

            isTimeSet = true;
        }

        private void UpdateTime()
        {
            if (!isTimeSet) { return; }

            DateTime currentTime = DateTime.Now;

            string formattedTime = currentTime.ToString("HH:mm");

            timeText.text = formattedTime;
        }

        /// <summary>
        /// Create and add a new icon to the status bar
        /// </summary>
        /// <param name="sprite">Icon sprite</param>
        /// <returns>The created <see cref="StatusEntry"/></returns>
        public static StatusEntry AddStatus(Sprite sprite)
        {
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

            /// <summary>
            /// Shows the status entry
            /// </summary>
            public void Show()
            {
                instance.SetActive(true);
            }

            /// <summary>
            /// Hides the status entry
            /// </summary>
            public void Hide()
            {
                instance.SetActive(false);
            }

            /// <summary>
            /// Removes the status entry
            /// </summary>
            public void Remove()
            {
                Destroy(instance);
            }
        }
    }

    [System.Serializable]
    public class TimeAndDateData
    {
        public TimeData time;
        public DateData date;
    }

    [System.Serializable]
    public class TimeData
    {
        public int h;
        public int m;
        public int s;
    }

    [System.Serializable]
    public class DateData
    {
        public int d;
        public int m;
        public int y;
    }

    public static class DateTimeExtensions
    {
        public static DateTime AddOffset(this DateTime dateTime, int hourOffset, int minuteOffset)
        {
            dateTime = dateTime.AddHours(hourOffset);
            dateTime = dateTime.AddMinutes(minuteOffset);
            return dateTime;
        }
    }

}


