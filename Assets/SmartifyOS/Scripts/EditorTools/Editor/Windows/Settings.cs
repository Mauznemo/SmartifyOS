using System;
using SmartifyOS.Editor.Styles;
using SmartifyOS.SerialCommunication;
using SmartifyOS.StatusBar;
using SmartifyOS.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Settings : EditorWindow
    {
        private Tab currentTab = Tab.Project;
        private Vector2 scrollPos;

        private bool autoCheckForUpdates = false;

        private GameObject vehicleParent;
        private BaseSerialCommunication[] serialScripts;
        private BaseUIWindow[] windows;
        private SystemManager systemManager;

        [MenuItem("SmartifyOS/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<Settings>("Settings");
            window.minSize = new Vector2(500, 300);
        }

        private enum Tab
        {
            Project,
            Editor,
            Communication,
            Apps,
            Help
        }

        private void OnEnable()
        {
            vehicleParent = GameObject.Find("VehicleParent");
            serialScripts = GameObject.FindObjectsByType<BaseSerialCommunication>(FindObjectsSortMode.None);
            windows = GameObject.FindObjectsByType<BaseUIWindow>(FindObjectsSortMode.None);
            systemManager = FindFirstObjectByType<SystemManager>();

            autoCheckForUpdates = EditorPrefs.GetBool("AutoCheckForUpdates", false);
        }

        private void OnGUI()
        {
            DrawTabBar();
            GUILayout.Space(10);
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            DrawActivePage();
            GUILayout.EndScrollView();
        }

        private void DrawActivePage()
        {
            switch (currentTab)
            {
                case Tab.Project:
                    DrawProjectPage();
                    break;
                case Tab.Editor:
                    DrawEditorPage();
                    break;
                case Tab.Communication:
                    DrawCommunicationPage();
                    break;
                case Tab.Apps:
                    DrawAppsPage();
                    break;
                case Tab.Help:
                    DrawHelpPage();
                    break;
            }
        }

        #region Pages
        private void DrawProjectPage()
        {
            GUILayout.Label("Scene", EditorStyles.boldLabel);
            string currentVehicle = "None";
            if (vehicleParent.transform.childCount > 0)
            {
                currentVehicle = vehicleParent.transform.GetChild(0).name;
            }
            BeginColorBox();
            GUILayout.Label("Current Vehicle: " + currentVehicle);
            GUI.enabled = vehicleParent.transform.childCount > 0;
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(100)))
            {
                Undo.DestroyObjectImmediate(vehicleParent.transform.GetChild(0).gameObject);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            GUI.enabled = true;
            EndColorBox();

            GUILayout.Label("Boot Screen", EditorStyles.boldLabel);
            BeginColorBox();
            GUILayout.Label("Show SmartifyOS Logo on Startup");
            systemManager.showLogoOnPowerOn = ToggleButton(systemManager.showLogoOnPowerOn, true);
            EndColorBox();

            BeginColorBox();
            GUILayout.Label("Show SmartifyOS Logo on Power Off");
            systemManager.showLogoOnPowerOff = ToggleButton(systemManager.showLogoOnPowerOff, true);
            EndColorBox();

        }

        private void DrawEditorPage()
        {
            GUILayout.Label("Editor Settings", EditorStyles.boldLabel);
            BeginColorBox();
            GUILayout.Label("Check for Updates on Startup (Experimental)");
            autoCheckForUpdates = ToggleButton(autoCheckForUpdates, false, (value) =>
            {
                EditorPrefs.SetBool("AutoCheckForUpdates", value);
            });
            EndColorBox();
        }

        private void DrawCommunicationPage()
        {
            GUILayout.Label("Serial Communication Scripts", EditorStyles.boldLabel);

            if (serialScripts.Length == 0)
            {
                GUILayout.Label("No serial communication scripts found.");
                GUILayout.Space(5);
            }

            foreach (var item in serialScripts)
            {
                BeginColorBox();
                GUILayout.Label(item.name);

                if (GUILayout.Button("Select", GUILayout.MaxWidth(100)))
                {
                    Selection.activeGameObject = item.gameObject;
                }
                EndColorBox();
            }

            GUILayout.Label("To communicate with serial deices like Arduinos right click in the project browser in the place where you want to create the new script. Then click on Create > SmartifyOS > Serial Communication Script.", EditorStyles.wordWrappedLabel);
            if (LinkLabel("How to create new serial communication scripts"))
            {
                Application.OpenURL("https://docs.smartify-os.com/docs/category/serial-communication");
            }
        }

        private void DrawAppsPage()
        {
            GUILayout.Label("App Windows", EditorStyles.boldLabel);
            foreach (var item in windows)
            {
                BeginColorBox();
                GUILayout.Label(item.name);

                if (GUILayout.Button("Show and Select", GUILayout.MaxWidth(150)))
                {
                    foreach (var window in windows)
                    {
                        window.Hide();
                    }
                    item.Show();
                    Selection.activeGameObject = item.gameObject;
                }
                EndColorBox();
            }

            if (LinkLabel("How to create new windows"))
            {
                Application.OpenURL("https://docs.smartify-os.com/docs/category/windows");
            }
        }

        private void DrawHelpPage()
        {
            GUILayout.Label("Finding Objects", EditorStyles.boldLabel);
            BeginColorBox();
            GUILayout.Label("Settings pages parent");

            if (GUILayout.Button("Select", GUILayout.MaxWidth(100)))
            {
                Selection.activeGameObject = GameObject.Find("Settings").transform.Find("PagesScrollView/Viewport/Pages").gameObject;
            }
            EndColorBox();

            BeginColorBox();
            GUILayout.Label("Quick settings buttons parent");

            if (GUILayout.Button("Select", GUILayout.MaxWidth(100)))
            {
                Selection.activeGameObject = FindFirstObjectByType<StatusBarDrag>().transform.Find("QuickSettings/Buttons").gameObject;
            }
            EndColorBox();

            GUILayout.Label("Links", EditorStyles.boldLabel);

            BeginColorBox();
            GUILayout.Label("Documentation");

            if (GUILayout.Button("Open", GUILayout.MaxWidth(100)))
            {
                Application.OpenURL("https://docs.smartify-os.com/");
            }
            EndColorBox();

            BeginColorBox();
            GUILayout.Label("Report a bug");

            if (GUILayout.Button("Open", GUILayout.MaxWidth(100)))
            {
                Application.OpenURL("https://github.com/Mauznemo/SmartifyOS/issues");
            }
            EndColorBox();

            BeginColorBox();
            GUILayout.Label("Support the project");

            if (GUILayout.Button("PayPal", GUILayout.MaxWidth(100)))
            {
                Application.OpenURL("https://www.paypal.com/donate/?hosted_button_id=BSPF2HUZRP7AN");
            }
            if (GUILayout.Button("GitHub Sponsor", GUILayout.MaxWidth(100)))
            {
                Application.OpenURL("https://github.com/sponsors/Mauznemo");
            }
            EndColorBox();

            BeginColorBox();
            GUILayout.Label("Discord Server");

            if (GUILayout.Button("Join", GUILayout.MaxWidth(100)))
            {
                Application.OpenURL("https://discord.gg/dYf8zrVUHt");
            }
            EndColorBox();
        }
        #endregion

        private bool ToggleButton(bool value, bool setSceneDirty = false, Action<bool> onValueChanged = null)
        {
            if (GUILayout.Button(value ? "Enabled" : "Disabled", GUILayout.MaxWidth(100)))
            {
                if (setSceneDirty)
                {
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                onValueChanged?.Invoke(!value);
                return !value;
            }
            return value;
        }

        private bool LinkLabel(string label, params GUILayoutOption[] options)
        {
            GUIStyle LinkStyle = new GUIStyle(EditorStyles.label);
            LinkStyle.normal.textColor = Color.magenta;
            LinkStyle.stretchWidth = true;
            LinkStyle.alignment = TextAnchor.MiddleCenter;
            LinkStyle.margin = new RectOffset(5, 5, 5, 5);
            LinkStyle.richText = true;


            return GUILayout.Button($"<u>{label}</u>", LinkStyle);
        }

        private void BeginColorBox()
        {
            GUILayout.BeginHorizontal(Style.Box, GUILayout.ExpandWidth(true));
        }

        private void EndColorBox()
        {
            GUILayout.EndHorizontal();
        }

        private void DrawTabBar()
        {
            GUILayout.BeginHorizontal();
            System.Collections.IList tabs = Enum.GetValues(typeof(Tab));
            for (int i = 0; i < tabs.Count; i++)
            {
                Tab tab = (Tab)tabs[i];
                GUI.color = currentTab == tab ? Color.gray : Color.white;
                if (GUILayout.Button(tab.ToString()))
                {
                    currentTab = tab;
                }
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
        }
    }
}
