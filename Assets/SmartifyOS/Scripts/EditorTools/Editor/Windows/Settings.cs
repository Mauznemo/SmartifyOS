using System;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Settings : EditorWindow
    {
        private Tab currentTab = Tab.General;

        [MenuItem("SmartifyOS/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<Settings>("Settings");
            window.minSize = new Vector2(500, 300);
        }

        private enum Tab
        {
            General,
            Test,
            Help
        }

        private void OnGUI()
        {
            DrawTabBar();
            DrawActivePage();
        }

        private void DrawActivePage()
        {
            switch (currentTab)
            {
                case Tab.General:
                    DrawGeneralPage();
                    break;
                case Tab.Test:
                    DrawTestPage();
                    break;
                case Tab.Help:
                    DrawHelpPage();
                    break;
            }
        }

        #region Pages
        private void DrawGeneralPage()
        {
            GUILayout.Label("General Page");
        }

        private void DrawTestPage()
        {
            GUILayout.Label("Test Page");
        }

        private void DrawHelpPage()
        {
            GUILayout.Label("Help Page");
        }
        #endregion

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
