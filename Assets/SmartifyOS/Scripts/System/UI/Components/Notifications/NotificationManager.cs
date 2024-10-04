using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using SmartifyOS.SaveSystem;
using UnityEngine;

namespace SmartifyOS.Notifications
{
    public class NotificationManager : MonoBehaviour
    {
        private static NotificationManager Instance;

        [SerializeField] private PushNotification pushNotificationPrefab;
        [SerializeField] private UpdateNotification updateNotificationPrefab;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Sends a new on screen notification
        /// </summary>
        /// <param name="notificationType">Changes the color, sound and icon of the notification</param>
        /// <param name="text">Text of the notification</param>
        /// <param name="mute">If the notification should be muted</param>
        /// <param name="showTime">Time in seconds to show the notification</param>
        public static void SendNotification(NotificationType notificationType, string text, bool mute = false, float showTime = 5f)
        {
            Instance.NewNotification(notificationType, text, mute, showTime);
        }

        private void NewNotification(NotificationType notificationType, string text, bool mute, float showTime)
        {
            PushNotification notification = Instantiate(pushNotificationPrefab, transform);
            notification.gameObject.SetActive(true);
            notification.Init(notificationType, text, showTime);

            if (mute)
                return;

            switch (notificationType)
            {
                case NotificationType.Info:
                    AudioManager.Instance.PlaySound(AudioManager.audioConfig_SO.notificationSounds.info);
                    break;
                case NotificationType.Warning:
                    //AudioManager.Instance.PlayWarningNotification();
                    AudioManager.Instance.PlaySound(AudioManager.audioConfig_SO.notificationSounds.warning);
                    break;
                case NotificationType.Error:
                    //AudioManager.Instance.PlayErrorNotification();
                    AudioManager.Instance.PlaySound(AudioManager.audioConfig_SO.notificationSounds.error);
                    break;
            }
        }

        public static void SendUpdateNotification(string usbName)
        {
            Instance.NewUpdateNotification(usbName);
        }

        private void NewUpdateNotification(string usbName)
        {
            UpdateNotification notification = Instantiate(updateNotificationPrefab, transform);
            notification.gameObject.SetActive(true);
            notification.Init($"Found new update on \"{usbName}\"", 20f);
        }

        //REMOVE THIS LATER
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SendNotification(NotificationType.Info, "Test");
            }
        }

        void Start()
        {
            Application.logMessageReceived += HandleLogMessage;
            SystemManager.OnUpdateAvailable += SendUpdateNotification;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= HandleLogMessage;
        }

        void HandleLogMessage(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                if (SaveManager.Load().notifications.ignoreErrors)
                {
                    return;
                }

                SendNotification(NotificationType.Error, logString);
            }
        }
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}


