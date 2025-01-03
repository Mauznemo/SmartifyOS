using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartifyOS.Editor.Styles;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Updater : EditorWindow
    {
        private static Type type;
        private static bool isChecking = false;
        private bool isUpdating = false;
        private static bool isUnityUpgrade;

        private static List<Git.Diff> diffs = new List<Git.Diff>();
        private static List<string> logs = new List<string>();

        private Vector2 filesScrollPosition;
        private Vector2 logScrollPosition;

        // Add a menu item to show the Git Update Checker window
        [MenuItem("SmartifyOS/Check for Updates", false, 1)]
        public static void ShowWindow()
        {
            var window = GetWindow<Updater>("Updater (Experimental)");
            window.minSize = new Vector2(500, 500);
        }

        private async void OnEnable()
        {
            diffs.Clear();
            logs.Clear();
            isUnityUpgrade = false;
            await CheckType();
        }

        private void OnGUI()
        {
            GUILayout.Label($"Check for update (Repository type: {type})", EditorStyles.boldLabel);
            GUI.color = Color.gray;
            GUILayout.Label($"This updater is still in an experimental stage. Please be careful and report any bugs", EditorStyles.wordWrappedLabel);
            GUI.color = Color.white;

            GUILayout.Space(20);

            if (logs.Count > 0)
            {
                GUILayout.Label("Change Log", EditorStyles.boldLabel);
                ShowChangelog();
            }

            if (diffs.Count > 0)
            {
                GUILayout.Label("Changed Files", EditorStyles.boldLabel);
                GUILayout.Label($"These files are different or new (if you edited any of these files without committing it will cause an error. If a file one the remote changed but you also changed it it will cause a merge conflict):", EditorStyles.wordWrappedLabel);
                ShowDiff();
            }

            GUILayout.FlexibleSpace();

            if (isUnityUpgrade)
            {
                GUILayout.Label("This update contains a Unity version upgrade!", EditorStyles.boldLabel);
                GUILayout.Label("You can't upgrade the Unity version while Unity is open.", EditorStyles.wordWrappedLabel);

                GUILayout.Space(20);

                if (GUILayout.Button("How to upgrade Unity version", Style.Button, GUILayout.Height(30)))
                {
                    Application.OpenURL("https://docs.smartify-os.com/docs/updating#how-to-upgrade-unity-version");
                }
                return;
            }

            if (diffs.Count < 1)
            {
                if (!isChecking && GUILayout.Button("Check for Update", Style.Button, GUILayout.Height(30)))
                {
                    CheckForUpdates();
                }
            }
            else
            {
                if (!isUpdating && GUILayout.Button("Sync Repository", Style.Button, GUILayout.Height(30)))
                {
                    SyncRepository();
                }
            }
        }

        public static async void CheckForUpdatesInBackground()
        {
            isChecking = true;

            await CheckType();

            // Fetch the latest changes from the remote repository
            await Git.Command($"fetch {GetRemote()}");

            int ahead = 0;
            int behind = 0;

            if (type == Type.Fork)
            {
                ahead = await Git.GetAhead("upstream/main");
                behind = await Git.GetBehind("upstream/main");
            }
            else if (type == Type.Clone)
            {
                ahead = await Git.GetAhead("origin/main");
                behind = await Git.GetBehind("origin/main");
            }

            if (behind > 0)
            {
                ShowWindow();
                bool update = EditorUtility.DisplayDialog("Update", $"You are {behind} commits behind {GetRemote()}. Would you like to update?", "Yes", "No");

                if (update)
                {
                    diffs = await Git.GetDiffs($"{GetRemote()}/main");
                    logs = await Git.GetChangelog($"{GetRemote()}/main");

                    isUnityUpgrade = IsUnityUpgrade();
                }
            }
            else
            {
                Debug.Log($"Your repository is up to date \n({ahead} commits ahead and {behind} commits behind {GetRemote()}).");
            }
            isChecking = false;
        }

        // Check for updates from the remote repository
        private async void CheckForUpdates()
        {
            isChecking = true;

            await CheckType();

            // Fetch the latest changes from the remote repository
            await Git.Command($"fetch {GetRemote()}");

            int ahead = 0;
            int behind = 0;

            if (type == Type.Fork)
            {
                ahead = await Git.GetAhead("upstream/main");
                behind = await Git.GetBehind("upstream/main");
            }
            else if (type == Type.Clone)
            {
                ahead = await Git.GetAhead("origin/main");
                behind = await Git.GetBehind("origin/main");
            }

            if (behind > 0)
            {
                bool update = EditorUtility.DisplayDialog("Update", $"You are {behind} commits behind {GetRemote()}. Would you like to update?", "Yes", "No");

                if (update)
                {
                    diffs = await Git.GetDiffs($"{GetRemote()}/main");
                    logs = await Git.GetChangelog($"{GetRemote()}/main");

                    isUnityUpgrade = IsUnityUpgrade();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Update", $"Your repository is up to date \n\n({ahead} commits ahead and {behind} commits behind {GetRemote()}).", "Ok");
            }
            isChecking = false;
            Repaint();
        }

        private void ShowChangelog()
        {
            logScrollPosition = EditorGUILayout.BeginScrollView(logScrollPosition);

            foreach (string log in logs)
            {
                GUILayout.Label($"- {log}");
            }

            EditorGUILayout.EndScrollView();
        }

        private void ShowDiff()
        {
            filesScrollPosition = EditorGUILayout.BeginScrollView(filesScrollPosition);

            foreach (Git.Diff diff in diffs)
            {
                string diffType = "";
                switch (diff.diffType)
                {
                    case Git.DiffType.Added:
                        GUI.color = Color.green;
                        diffType = "A";
                        break;
                    case Git.DiffType.Modified:
                        GUI.color = Color.yellow;
                        diffType = "M";
                        break;
                    case Git.DiffType.Deleted:
                        GUI.color = Color.red;
                        diffType = "D";
                        break;
                }

                GUILayout.Label($"({diffType}) {diff.name}");

                GUI.color = Color.white;
            }

            EditorGUILayout.EndScrollView();
        }

        private static async Task CheckType()
        {
            if (await Git.HasUpstream())
            {
                type = Type.Fork;
            }
            else
            {
                type = Type.Clone;
            }
        }

        private static string GetRemote()
        {
            return type == Type.Fork ? "upstream" : "origin";
        }

        private static bool IsUnityUpgrade()
        {
            foreach (var log in logs)
            {
                if (log.Contains("unity upgrade"))
                {
                    return true;
                }
            }

            return false;
        }

        // Sync the local repository with remote changes
        private async void SyncRepository()
        {
            isUpdating = true;

            // Pull the latest changes
            (string output, string error) = await Git.Command($"pull {GetRemote()} main");

            if (!string.IsNullOrEmpty(error) && (error.ToLower().Contains("error") || error.ToLower().Contains("conflict")))
            {
                ShowErrorDialog(error);
            }
            else if (!string.IsNullOrEmpty(output.Trim()))
            {
                EditorUtility.DisplayDialog("Update", output, "Ok");
            }
            else if (!string.IsNullOrEmpty(error))
            {
                ShowErrorDialog(error);
            }

            diffs.Clear();
            logs.Clear();

            isUpdating = false;
        }

        private void ShowErrorDialog(string error)
        {
            if (EditorUtility.DisplayDialog("Update ERROR!", error, "How to resolve?", "Ok"))
            {
                Application.OpenURL("https://docs.smartify-os.com/docs/updating#errors-while-updating");
            }
        }

        private enum Type
        {
            Clone,
            Fork
        }
    }
}

