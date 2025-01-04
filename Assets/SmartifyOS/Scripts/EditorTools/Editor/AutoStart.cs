using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SmartifyOS.Editor
{
    [InitializeOnLoad]
    public class AutoStart
    {
        static AutoStart()
        {
            EditorApplication.update += Update;

        }

        public static void Update()
        {
            if (!SessionState.GetBool("FirstInitDone", false))
            {
                SessionState.SetBool("FirstInitDone", true);
                EditorApplication.update -= Update;
                OnStartup();
            }
        }

        public static void OnStartup()
        {
            bool isFirstLaunch = EditorPrefs.GetInt("SmartifyOSWelcome", 0) == 0;
            if (isFirstLaunch)
                Welcome.ShowWindow();

            if (EditorPrefs.GetBool("AutoCheckForUpdates", false))
            {
                Updater.CheckForUpdatesInBackground();
            }

            if (EditorPrefs.GetBool("EditorUpdateShowAtStartup", true))
            {
                EditorPrefs.SetBool("EditorUpdateShowAtStartup", false);
            }

            if (EditorPrefs.GetBool("AutoOpenMainScene", true))
            {
                EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
            }
        }
    }
}

