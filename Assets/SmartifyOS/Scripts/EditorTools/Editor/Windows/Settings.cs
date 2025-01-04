using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common;
using SmartifyOS.Editor.Styles;
using SmartifyOS.Editor.Theming;
using SmartifyOS.SerialCommunication;
using SmartifyOS.StatusBar;
using SmartifyOS.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Settings : EditorWindow
    {
        public static bool DEV_MODE
        {
            get
            {
                return EditorPrefs.GetBool("DEV_MODE", false);
            }
            set
            {
                EditorPrefs.SetBool("DEV_MODE", value);
            }
        }

        private Tab currentTab = Tab.Project;
        private Vector2 scrollPos;

        private bool autoCheckForUpdates = false;
        private bool autoOpenMainScene = true;

        private GameObject vehicleParent;
        private List<BaseSerialCommunication> serialScripts;
        private List<BaseUIWindow> windows;
        private SystemManager systemManager;

        private ThemeData themeData;
        private Color newColorStyleColor = Color.white;
        private string newColorStyleName = "";
        private bool newColorStyleRemovable = true;
        private float newValueStyleValue = 0f;
        private string newValueStyleName = "";
        private bool newValueStyleRemovable = true;

        [MenuItem("SmartifyOS/Settings", false, 0)]
        [Shortcut("SmartifyOS/Open Settings", KeyCode.Period, ShortcutModifiers.Action)]
        public static void ShowWindow()
        {
            var window = GetWindow<Settings>("Settings");
            window.minSize = new Vector2(500, 300);
        }

        [MenuItem("Window/SOS Dev Tools/Activate Dev Mode")]
        public static void ActivateDevMode()
        {
            DEV_MODE = true;
        }

        [MenuItem("Window/SOS Dev Tools/Deactivate Dev Mode")]
        public static void DeactivateDevMode()
        {
            DEV_MODE = false;
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
            GetObjects();

            autoCheckForUpdates = EditorPrefs.GetBool("AutoCheckForUpdates", false);
            autoOpenMainScene = EditorPrefs.GetBool("AutoOpenMainScene", true);
            currentTab = (Tab)EditorPrefs.GetInt("SettingsTab", 0);

            themeData = ThemeData.GetThemeData();
        }

        private void GetObjects()
        {
            vehicleParent = GameObject.Find("VehicleParent");
            serialScripts = GameObject.FindObjectsByType<BaseSerialCommunication>(FindObjectsSortMode.None).ToList();
            windows = GameObject.FindObjectsByType<BaseUIWindow>(FindObjectsSortMode.None).ToList();
            systemManager = FindFirstObjectByType<SystemManager>();
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


            GUILayout.Label("Theming", EditorStyles.boldLabel);
            GUILayout.BeginVertical(Style.Box, GUILayout.ExpandWidth(true));
            GUILayout.Label("Colors:", EditorStyles.boldLabel);
            foreach (var colorStyle in themeData.colorStyles.styles)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(colorStyle.Key, GUILayout.Width(300));
                colorStyle.Value.color = EditorGUILayout.ColorField(colorStyle.Value.color);
                if (colorStyle.Value.canBeRemoved || DEV_MODE)
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(100)))
                    {
                        var colorThemers = GameObject.FindObjectsByType<ColorThemer>(FindObjectsSortMode.None);
                        bool inUse = false;
                        foreach (var colorThemer in colorThemers)
                        {
                            if (colorThemer.GetStyleName() == colorStyle.Key)
                            {
                                inUse = true;
                                break;
                            }
                        }

                        if (inUse)
                        {
                            bool delete = EditorUtility.DisplayDialog("In Use", $"The color style {colorStyle.Key} is in use. If you remove it every themer using it will default to the first style.", "Delete", "Cancel");

                            if (delete)
                            {
                                foreach (var colorThemer in colorThemers)
                                {
                                    if (colorThemer.GetStyleName() == colorStyle.Key)
                                    {
                                        colorThemer.styleName = themeData.colorStyles.GetFirstStyleName();
                                        colorThemer.UpdateColor(ThemeData.GetThemeData().GetColor(colorThemer.styleName));
                                        EditorUtility.SetDirty(colorThemer);
                                    }
                                }

                                ActiveEditorTracker.sharedTracker.ForceRebuild();
                                themeData.colorStyles.RemoveStyle(colorStyle.Key);
                                themeData.SaveAsset();
                                GUIUtility.ExitGUI();
                            }
                        }
                        else
                        {
                            themeData.colorStyles.RemoveStyle(colorStyle.Key);
                            themeData.SaveAsset();
                            GUIUtility.ExitGUI();
                        }
                    }
                }

                if (GUILayout.Button("Apply", GUILayout.Width(80)))
                {
                    var colorThemers = GameObject.FindObjectsByType<ColorThemer>(FindObjectsSortMode.None);
                    foreach (var colorThemer in colorThemers)
                    {
                        if (colorThemer.GetStyleName() == colorStyle.Key)
                        {
                            colorThemer.UpdateColor(colorStyle.Value.color);
                        }
                    }
                    EditorUtility.SetDirty(this);
                    SceneView.RepaintAll();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            newColorStyleName = EditorGUILayout.TextField(newColorStyleName);
            newColorStyleColor = EditorGUILayout.ColorField(newColorStyleColor);

            if (DEV_MODE)
            {
                newColorStyleRemovable = GUILayout.Toggle(newColorStyleRemovable, "Removable");
            }

            if (GUILayout.Button("Add Color"))
            {
                if (string.IsNullOrEmpty(newColorStyleName))
                {
                    EditorUtility.DisplayDialog("No name", "Please enter a name for your style.", "Ok");
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return;
                }

                if (themeData.colorStyles.styles.ContainsKey(newColorStyleName))
                {
                    EditorUtility.DisplayDialog("Already exists", "Style with name '" + newColorStyleName + "' already exists.", "Ok");
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return;
                }

                GUIUtility.keyboardControl = 0;
                themeData.colorStyles.AddStyle(newColorStyleName, newColorStyleColor, newColorStyleRemovable);
                newColorStyleName = "";
                newColorStyleColor = Color.white;
                newColorStyleRemovable = true;
                themeData.SaveAsset();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


            GUILayout.BeginVertical(Style.Box, GUILayout.ExpandWidth(true));
            GUILayout.Label("Values:", EditorStyles.boldLabel);
            foreach (var valueStyle in themeData.valueStyles.styles)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(valueStyle.Key, GUILayout.Width(300));
                valueStyle.Value.value = EditorGUILayout.FloatField(valueStyle.Value.value);
                if (valueStyle.Value.canBeRemoved || DEV_MODE)
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(100)))
                    {
                        var valueThemers = GameObject.FindObjectsByType<ValueThemer>(FindObjectsSortMode.None);
                        bool inUse = false;
                        foreach (var valueThemer in valueThemers)
                        {
                            if (valueThemer.GetStyleName() == valueStyle.Key)
                            {
                                inUse = true;
                                break;
                            }
                        }

                        if (inUse)
                        {
                            bool delete = EditorUtility.DisplayDialog("In Use", $"The value style {valueStyle.Key} is in use. If you remove it every themer using it will default to the first style.", "Delete", "Cancel");

                            if (delete)
                            {
                                foreach (var valueThemer in valueThemers)
                                {
                                    if (valueThemer.GetStyleName() == valueStyle.Key)
                                    {
                                        valueThemer.styleName = themeData.valueStyles.GetFirstStyleName();
                                        valueThemer.UpdateValue(ThemeData.GetThemeData().GetValue(valueThemer.styleName));
                                        EditorUtility.SetDirty(valueThemer);
                                    }
                                }

                                ActiveEditorTracker.sharedTracker.ForceRebuild();
                                themeData.valueStyles.RemoveStyle(valueStyle.Key);
                                themeData.SaveAsset();
                                GUIUtility.ExitGUI();
                            }
                        }
                        else
                        {
                            themeData.valueStyles.RemoveStyle(valueStyle.Key);
                            themeData.SaveAsset();
                            GUIUtility.ExitGUI();
                        }
                    }
                }

                if (GUILayout.Button("Apply", GUILayout.Width(80)))
                {
                    var valueThemers = GameObject.FindObjectsByType<ValueThemer>(FindObjectsSortMode.None);
                    foreach (var valueThemer in valueThemers)
                    {
                        if (valueThemer.GetStyleName() == valueStyle.Key)
                        {
                            valueThemer.UpdateValue(valueStyle.Value.value);
                        }
                    }
                    EditorUtility.SetDirty(this);
                    SceneView.RepaintAll();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            newValueStyleName = EditorGUILayout.TextField(newValueStyleName);
            newValueStyleValue = EditorGUILayout.FloatField(newValueStyleValue);

            if (DEV_MODE)
            {
                newValueStyleRemovable = GUILayout.Toggle(newValueStyleRemovable, "Removable");
            }

            if (GUILayout.Button("Add Value"))
            {
                if (string.IsNullOrEmpty(newValueStyleName))
                {
                    EditorUtility.DisplayDialog("No name", "Please enter a name for your style.", "Ok");
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return;
                }

                if (themeData.valueStyles.styles.ContainsKey(newValueStyleName))
                {
                    EditorUtility.DisplayDialog("Already exists", "Style with name '" + newValueStyleName + "' already exists.", "Ok");
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return;
                }

                GUIUtility.keyboardControl = 0;
                themeData.valueStyles.AddStyle(newValueStyleName, newValueStyleValue, newValueStyleRemovable);
                newValueStyleName = "";
                newValueStyleValue = 0f;
                newValueStyleRemovable = true;
                themeData.SaveAsset();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
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

            BeginColorBox();
            GUILayout.Label("Auto open Main Scene on Startup");
            autoOpenMainScene = ToggleButton(autoOpenMainScene, false, (value) =>
            {
                EditorPrefs.SetBool("AutoOpenMainScene", value);
            });
            EndColorBox();
        }

        private void DrawCommunicationPage()
        {
            GUILayout.Label("Serial Communication Scripts", EditorStyles.boldLabel);

            if (serialScripts.Count == 0)
            {
                GUILayout.Label("No serial communication scripts found.");
                GUILayout.Space(5);
            }

            foreach (var item in serialScripts)
            {
                if (item == null)
                    continue;

                BeginColorBox();
                GUILayout.Label(item.name);

                if (GUILayout.Button("Select", GUILayout.MaxWidth(100)))
                {
                    Selection.activeGameObject = item.gameObject;
                }

                if (GUILayout.Button(new GUIContent("Remove", "Remove serial communication and delete its script file"), GUILayout.Width(60)))
                {
                    serialScripts.Remove(item);
                    item.RemoveAndDeleteFile();
                    GetObjects();

                    EndColorBox();
                    break;
                }
                EndColorBox();
            }

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
                if (item == null)
                    continue;

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

                if (GUILayout.Button(new GUIContent("Remove", "Remove window and delete its script file"), GUILayout.Width(60)))
                {
                    windows.Remove(item);
                    item.RemoveAndDeleteFile();
                    GetObjects();

                    EndColorBox();
                    break;
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
                    EditorPrefs.SetInt("SettingsTab", (int)tab);
                }
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
        }
    }
}
