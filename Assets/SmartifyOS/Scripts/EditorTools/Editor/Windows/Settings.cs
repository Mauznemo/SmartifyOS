using System;
using SmartifyOS.Editor.Styles;
using SmartifyOS.SerialCommunication;
using SmartifyOS.StatusBar;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Settings : EditorWindow
    {
        private Tab currentTab = Tab.Project;

        private GameObject vehicleParent;
        private BaseSerialCommunication[] serialScripts;
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
            Communication,
            Help
        }

        private void OnEnable()
        {
            vehicleParent = GameObject.Find("VehicleParent");
            serialScripts = GameObject.FindObjectsByType<BaseSerialCommunication>(FindObjectsSortMode.None);
            systemManager = FindFirstObjectByType<SystemManager>();
        }

        private void OnGUI()
        {
            DrawTabBar();
            GUILayout.Space(10);
            DrawActivePage();
        }

        private void DrawActivePage()
        {
            switch (currentTab)
            {
                case Tab.Project:
                    DrawProjectPage();
                    break;
                case Tab.Communication:
                    DrawCommunicationPage();
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

        private bool ToggleButton(bool value, bool setSceneDirty = false)
        {
            if (GUILayout.Button(value ? "Enabled" : "Disabled", GUILayout.MaxWidth(100)))
            {
                if (setSceneDirty)
                {
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
                return !value;
            }
            return value;
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
