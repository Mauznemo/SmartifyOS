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

        private bool timeSet;

        private void Start()
        {
            LiveDataController.OnDateAndTime += LiveDataController_OnDateAndTime;

            InvokeRepeating(nameof(UpdateTime), 0f, 5f);
        }

        private void LiveDataController_OnDateAndTime(string jsonString)
        {
            if (timeSet)
                return;

            try
            {
                var timeAndDateData = JsonUtility.FromJson<TimeAndDateData>(jsonString);

                SetTime(ConvertToDateTime(timeAndDateData));
            }
            catch (Exception)
            { }
        }

        private async void SetTime(DateTime dateTime)
        {
            int hourOffset = SaveManager.Load().timezone.hourOffset;
            int minuteOffset = SaveManager.Load().timezone.minuteOffset;

            dateTime = dateTime.AddOffset(hourOffset, minuteOffset);

            string setDateCommand = $"sudo timedatectl set-time \"{dateTime.Year}-{dateTime.Month}-{dateTime.Day}\"";
            string setTimeCommand = $"sudo timedatectl set-time \"{dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}\"";
            await LinuxCommand.RunAsync(setDateCommand);
            await LinuxCommand.RunAsync(setTimeCommand);

            timeSet = true;
        }

        private void UpdateTime()
        {
            if (!timeSet) { return; }

            DateTime currentTime = DateTime.Now;

            string formattedTime = currentTime.ToString("HH:mm");

            timeText.text = formattedTime;
        }

        public static DateTime ConvertToDateTime(TimeAndDateData timeAndDateData)
        {
            if (timeAndDateData == null || timeAndDateData.time == null || timeAndDateData.date == null)
            {
                throw new ArgumentNullException("TimeAndDateData or its components cannot be null.");
            }

            return new DateTime(
                timeAndDateData.date.y,
                timeAndDateData.date.m,
                timeAndDateData.date.d,
                timeAndDateData.time.h,
                timeAndDateData.time.m,
                timeAndDateData.time.s
            );
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


