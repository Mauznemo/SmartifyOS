using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SmartifyOS.Linux;
using SmartifyOS.Notifications;
using SmartifyOS.SaveSystem;
using SmartifyOS.SystemEvents;
using UnityEngine;

namespace SmartifyOS
{
    public class SystemManager : MonoBehaviour
    {
        public static SystemManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public static Action<string> OnUpdateAvailable;

        [SerializeField] private bool saveButton;
        [SerializeField] private bool scanButton;

        private bool locked = false;
        private string updatePath;

        private List<string> notifiedDirectories = new List<string>();

        private void Start()
        {
            SystemEventManager.SubscribeToEvent("SmartifyOS/Events/OnUsbDeviceConnected", OnUsbDeviceConnected);
        }

        private void OnUsbDeviceConnected(string content)
        {
            SearchForUpdate();
        }

        public void SearchForUpdate(bool clearHistory = false)
        {
            if (locked)
            {
                return;
            }
            locked = true;

            if (clearHistory)
            {
                notifiedDirectories.Clear();
            }

            StartCoroutine(SearchForDirectory());
        }

        private IEnumerator SearchForDirectory()
        {
            yield return new WaitForSeconds(3f);

            updatePath = RunLinuxShellScript.Run("~/SmartifyOS/Scripts/FindUpdatePath.sh");
            updatePath = updatePath.Trim();

            if (string.IsNullOrEmpty(updatePath) || updatePath == "not_found")
            {
                yield break;
            }

            if (!Directory.Exists(updatePath))
            {
                yield break;
            }

            if (notifiedDirectories.Contains(updatePath))
            {
                locked = false;
                yield break;
            }
            notifiedDirectories.Add(updatePath);

            OnUpdateAvailable?.Invoke(new DirectoryInfo(updatePath).Parent.Name);

            locked = false;
        }

        public void InstallUpdate()
        {
            if (!Directory.Exists(updatePath))
            {
                NotificationManager.SendNotification(NotificationType.Error, "USB device no longer connected");
                return;
            }

            RunLinuxShellScript.RunWithWindow("~/SmartifyOS/Scripts/AutoUpdate.sh", updatePath);
            Application.Quit();
        }


        private void OnApplicationQuit()
        {
            SaveManager.Save();
        }

        private void Update()
        {
            if (saveButton)
            {
                saveButton = false;
                SaveManager.Save();
            }

            if (scanButton)
            {
                scanButton = false;
                StartCoroutine(SearchForDirectory());
            }
        }
    }
}


