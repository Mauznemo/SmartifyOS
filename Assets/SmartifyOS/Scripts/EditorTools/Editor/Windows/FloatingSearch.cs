using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartifyOS.Editor;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class SearchItemAttribute : Attribute
{
    public string Title { get; }

    public SearchItemAttribute(string title)
    {
        Title = title;
    }
}


public class FloatingSearch : EditorWindow
{
    private string searchText = "";
    private List<MethodInfo> searchResults = new List<MethodInfo>();
    private int selectedIndex = 0;
    private static Dictionary<string, MethodInfo> searchItems;

    private bool focusRequested = false;

    private static FloatingSearch window;

    [MenuItem("Window/Search Window")]
    [Shortcut("SmartifyOS/Search", KeyCode.Space, ShortcutModifiers.Action)]
    public static void ShowWindow()
    {
        //window = GetWindow<FloatingSearch>("Search Window");
        window = CreateInstance<FloatingSearch>();
        window.ShowPopup();
        // Get screen dimensions
        int screenWidth = Screen.currentResolution.width;
        int screenHeight = Screen.currentResolution.height;
        window.position = new Rect((screenWidth - 600 - 200) / 2, (screenHeight - 78 - 500) / 2, 600, 78);
        PopulateSearchItems();
    }

    private static void PopulateSearchItems()
    {
        searchItems = new Dictionary<string, MethodInfo>();
        var methods = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(method => method.GetCustomAttribute<SearchItemAttribute>() != null);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<SearchItemAttribute>();
            if (!searchItems.ContainsKey(attribute.Title))
            {
                searchItems.Add(attribute.Title, method);
            }
        }
    }

    private void OnLostFocus()
    {
        Close();
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Search", EditorStyles.boldLabel);

        GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
        textFieldStyle.fontStyle = FontStyle.Bold;
        textFieldStyle.fontSize = 13;
        textFieldStyle.padding = new RectOffset(10, 5, 7, 5);

        GUI.SetNextControlName("SearchTextField");
        string newSearchText = EditorGUILayout.TextField(searchText, textFieldStyle, GUILayout.Height(30));

        if (!focusRequested)
        {
            EditorGUI.FocusTextInControl("SearchTextField");
            focusRequested = true; // Ensure this only runs once
        }

        if (newSearchText != searchText)
        {
            searchText = newSearchText;
            UpdateSearchResults();
            selectedIndex = -1;
        }

        DrawSearchResults();
        HandleInput();
    }

    private void UpdateSearchResults()
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            searchResults.Clear();
        }
        else
        {
            searchResults = searchItems
                .Where(item => item.Key.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(item => item.Value)
                .ToList();
        }
    }

    private void DrawSearchResults()
    {
        window.SetSize(600, 78 + Mathf.Clamp(searchResults.Count - 1, 0, 10) * 16);

        if (searchResults.Count == 0)
        {
            EditorGUILayout.LabelField("No results found.");
            return;
        }


        for (int i = 0; i < searchResults.Count; i++)
        {
            var method = searchResults[i];
            var attribute = method.GetCustomAttribute<SearchItemAttribute>();

            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.richText = true;
            if (i == selectedIndex)
            {
                style.normal.textColor = Color.white;
                style.normal.background = Texture2D.grayTexture;
            }

            string displayText = attribute.Title;
            int matchIndex = displayText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);

            if (matchIndex >= 0)
            {
                string before = displayText.Substring(0, matchIndex);
                string match = displayText.Substring(matchIndex, searchText.Length);
                string after = displayText.Substring(matchIndex + searchText.Length);

                GUILayout.BeginHorizontal();
                GUILayout.Label($"{before}<b>{match}</b>{after}", style);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label(displayText, style);
            }
        }
    }

    private void HandleInput()
    {
        if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.DownArrow)
            {
                selectedIndex = Mathf.Min(selectedIndex + 1, searchResults.Count - 1);
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.UpArrow)
            {
                selectedIndex = Mathf.Max(selectedIndex - 1, 0);
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Return && selectedIndex >= 0)
            {
                ExecuteSelectedItem();
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Escape)
            {
                Close();
                Event.current.Use();
            }
        }
    }

    private void ExecuteSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < searchResults.Count)
        {
            var method = searchResults[selectedIndex];
            method.Invoke(null, null);
        }
    }

    [SearchItem("Settings: Project")]
    public static void SettingsProject()
    {
        Settings.ShowWindowAt(Settings.Tab.Project);
    }

    [SearchItem("Settings: Editor")]
    public static void SettingsEditor()
    {
        Settings.ShowWindowAt(Settings.Tab.Editor);
    }

    [SearchItem("Settings: Communication")]
    public static void SettingsCommunication()
    {
        Settings.ShowWindowAt(Settings.Tab.Communication);
    }

    [SearchItem("Settings: Apps")]
    public static void SettingsApps()
    {
        Settings.ShowWindowAt(Settings.Tab.Apps);
    }
}
