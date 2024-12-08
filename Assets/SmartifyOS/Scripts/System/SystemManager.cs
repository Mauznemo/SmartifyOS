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
        public static event Action OnPowerOff;

        public bool showLogoOnPowerOn;
        public bool showLogoOnPowerOff = true;

        [SerializeField] private bool saveButton;
        [SerializeField] private bool scanButton;

        [SerializeField] private LogoScreen logoScreen;

        private bool locked = false;
        private string updatePath;

        private void Start()
        {
            SystemEventManager.SubscribeToEvent("SmartifyOS/Events/OnUsbDeviceConnected", OnUsbDeviceConnected);

            if (showLogoOnPowerOn)
                logoScreen.ShowScreenFor(1f, false);
        }

        private void OnUsbDeviceConnected(string content)
        {
            UnityMainThreadDispatcher.GetInstance().Enqueue(() => SearchForUpdate());
        }

        public void SearchForUpdate()
        {
            StartCoroutine(SearchForDirectory());
        }

        private IEnumerator SearchForDirectory()
        {
            if (locked)
            {
                yield break;
            }
            locked = true;

            //NotificationManager.SendNotification(NotificationType.Info, "USB device connected, searching for update...");

            yield return new WaitForSeconds(5f);

            updatePath = RunLinuxShellScript.Run("~/SmartifyOS/Scripts/FindUpdatePath.sh");
            updatePath = updatePath.Trim();

            if (string.IsNullOrEmpty(updatePath) || updatePath == "not_found")
            {
                locked = false;
                yield break;
            }

            if (!Directory.Exists(updatePath))
            {
                locked = false;
                yield break;
            }

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

        public void ShutdownSystem()
        {
            SaveManager.Save();
            OnPowerOff?.Invoke();
            if (showLogoOnPowerOff)
                logoScreen.ShowScreen();

            Invoke(nameof(Shutdown), 2f);
        }

        private void Shutdown()
        {
            string s = LinuxCommand.Run("sudo sleep 1s; sudo shutdown -h now");
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


