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
        private Type type;
        private bool isChecking = false;
        private bool isUpdating = false;

        List<Git.Diff> diffs = new List<Git.Diff>();

        private Vector2 scrollPosition;

        // Add a menu item to show the Git Update Checker window
        [MenuItem("SmartifyOS/Check for Updates")]
        public static void ShowWindow()
        {
            var window = GetWindow<Updater>("Updater (Experimental)");
            window.minSize = new Vector2(500, 300);
        }

        private async void OnEnable()
        {
            diffs.Clear();
            await CheckType();
        }

        private void OnGUI()
        {
            GUILayout.Label($"Check for update (Repository type: {type})", EditorStyles.boldLabel);
            GUI.color = Color.gray;
            GUILayout.Label($"This updater is still in an experimental stage. Please be careful and report any bugs", EditorStyles.wordWrappedLabel);
            GUI.color = Color.white;

            GUILayout.Space(20);

            if (diffs.Count > 0)
            {
                GUILayout.Label($"These files are different or new (files will only be deleted if you haven't modified them):", EditorStyles.wordWrappedLabel);
                ShowDiff();
            }

            GUILayout.FlexibleSpace();

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
                if (behind > 0)
                {
                    bool update = EditorUtility.DisplayDialog("Update", $"You are {behind} commits behind {GetRemote()}. Would you like to update?", "Yes", "No");
                }
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
                    diffs = await Git.GetDiffs();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Update", $"Your repository is up to date \n\n({ahead} commits ahead and {behind} commits behind {GetRemote()}).", "Ok");
            }
            isChecking = false;
            Repaint();
        }

        private void ShowDiff()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

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

        private async Task CheckType()
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

        private string GetRemote()
        {
            return type == Type.Fork ? "upstream" : "origin";
        }

        // Sync the local repository with remote changes
        private async void SyncRepository()
        {
            isUpdating = true;

            // Pull the latest changes
            string output = await Git.Command($"pull {GetRemote()} main");

            EditorUtility.DisplayDialog("Update", output, "Ok");

            diffs.Clear();

            isUpdating = false;
        }

        private enum Type
        {
            Clone,
            Fork
        }
    }
}

