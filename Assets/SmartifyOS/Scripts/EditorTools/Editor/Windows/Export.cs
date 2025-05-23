using SmartifyOS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using SmartifyOS.Editor.Styles;
using System;
using System.Linq;
using UnityEditor.ShortcutManagement;

namespace SmartifyOS.Editor
{
    public class Export : EditorWindow
    {
        [MenuItem("SmartifyOS/Export", false, 2)]
        [Shortcut("SmartifyOS/Open Export", KeyCode.E, ShortcutModifiers.Action)]
        public static void ShowWindow()
        {
            var window = GetWindow<Export>("SmartifyOS Export");
            //window.SetSize(700, 400);
            window.Show();
            window.minSize = new Vector2(300, 300);

        }

        private List<DirectoryInfo> usbDrives = new List<DirectoryInfo>();
        private List<string> usbDriveTexts = new List<string>();

        private readonly string[] exportTargets = new string[] { "Linux x86", "Linux Arm (Experimental)" };

        private int selectedDrive = 0;
        private int selectedTarget = 0;
        private bool exporting = false;

        private void OnEnable()
        {
            GetUSBDrives();
        }

        private void OnGUI()
        {
            Title();

            GUILayout.Space(30);

            if (usbDrives.Count < selectedDrive)
            {
                selectedDrive = 0;
            }

            EditorGUILayout.LabelField("Select Target USB Drive");
            if (usbDrives.Count > 0)
            {
                GUILayout.BeginHorizontal();
                {


                    selectedDrive = EditorGUILayout.Popup(selectedDrive, usbDriveTexts.ToArray(), Style.Popup, GUILayout.Height(30));
                    if (GUILayout.Button("Refresh", GUILayout.MaxWidth(60), GUILayout.Height(30)))
                    {
                        GetUSBDrives();
                    }
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("No USB Drives Found");
                if (GUILayout.Button("Search for USB Drives"))
                {
                    GetUSBDrives();
                }
            }


            GUILayout.Space(30);

            EditorGUILayout.LabelField("Export Target");
            selectedTarget = EditorGUILayout.Popup(selectedTarget, exportTargets, Style.Popup, GUILayout.Height(30));

            GUILayout.Space(20);

            if (selectedTarget == 1)
            {
                EditorGUILayout.LabelField("Linux Arm is not yet supported");
            }

            GUI.enabled = usbDrives.Count > 0 && selectedTarget != 1 && !exporting;


            if (GUILayout.Button("Export Installer", Style.Button, GUILayout.Height(30)))
            {
                ExportInstaller();
            }

            if (GUILayout.Button("Export Update", Style.Button, GUILayout.Height(30)))
            {
                ExportUpdate();
            }

            GUI.enabled = true;

            if (exporting)
            {
                EditorGUILayout.LabelField("Exporting...");
            }
        }

        private async void ExportInstaller()
        {
            exporting = true;
            string usbPath = usbDrives[selectedDrive].FullName;

            if (!Directory.Exists(usbPath))
                return;

            bool cloneSuccess = await Git.CloneRepository("https://github.com/Mauznemo/SmartifyOS-Installer.git", $"{usbPath}/SmartifyOS-Installer/");

            if (!cloneSuccess)
            {
                EditorUtility.DisplayDialog("Error", "Failed to clone repository", "Okay");
                exporting = false;
                return;
            }

            UnityEngine.Debug.Log("Cloned repository");

            if (!Directory.Exists($"{usbPath}/SmartifyOS-Installer/"))
            {
                UnityEngine.Debug.LogError("Couldn't find SmartifyOS-Installer folder");
                return;
            }

            if (!Directory.Exists($"{usbPath}/SmartifyOS-Installer/SmartifyOS/GUI/"))
            {
                Directory.CreateDirectory($"{usbPath}/SmartifyOS-Installer/SmartifyOS/GUI/");
            }

            BuildTo($"{usbPath}/SmartifyOS-Installer/SmartifyOS/GUI/");

            exporting = false;
        }

        private void ExportUpdate()
        {
            exporting = true;
            string usbPath = usbDrives[selectedDrive].FullName;

            if (!Directory.Exists(usbPath))
                return;

            string buildPath = usbPath + "/smartify_os_update/";

            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            BuildTo(buildPath);
        }

        private void BuildTo(string path)
        {
            if (!Directory.Exists(path))
                return;
            DeleteSubFolders(path);

            PlayerSettings.bundleVersion = GetBuildVersion();

            BuildOptions buildOptions = BuildOptions.None;
            string[] scenePaths = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => scene.path)
                    .ToArray();
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenePaths,
                locationPathName = $"{path}SmartifyOS.x86_64",
                target = BuildTarget.StandaloneLinux64,
                options = buildOptions,
                //subtarget = (int)buildSubtarget

            };

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            if (buildReport.summary.totalErrors == 0)
            {
                DeleteFoldersWithDoNotShip(path);

                EditorUtility.DisplayDialog("Build Successful", "Build Successful", "Okay");
            }
            else
            {
                EditorUtility.DisplayDialog($"{buildReport.summary.totalErrors} Errors", $"{buildReport.summary.totalErrors} Errors during Build", "Okay");
            }

            exporting = false;
        }

        private void DeleteFoldersWithDoNotShip(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            if (!directory.Exists)
            {
                return;
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                if (subDirectory.Name.ToLower().Contains("donotship"))
                {
                    subDirectory.Delete(true);
                }
                else
                {
                    DeleteFoldersWithDoNotShip(subDirectory.FullName);
                }
            }
        }

        private void DeleteSubFolders(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            if (!directory.Exists)
            {
                return;
            }

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                DeleteSubFolders(subDirectory.FullName);

                foreach (FileInfo file in subDirectory.GetFiles())
                {
                    file.Delete();
                }

                subDirectory.Delete(true);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
        }

        private void Title()
        {
            GUILayout.Space(30);
            EditorGUILayout.LabelField("Export Application", Style.LargeHeading);

            var descriptionStyle = new GUIStyle(EditorStyles.label);
            descriptionStyle.padding = new RectOffset(5, 0, 0, 0);
            descriptionStyle.wordWrap = true;

            EditorGUILayout.LabelField("Export your application as an installer or update.", descriptionStyle);
        }

        private void GetDrivesLinux()
        {
            usbDrives.Clear();
            usbDriveTexts.Clear();

            var drivesString = LinuxCommand.Run("df -h | grep -E '^/dev/' | awk '{print $NF}'");
            string[] drives = drivesString.Split('\n');


            foreach (var drive in drives)
            {
                if (drive == "/boot/efi" || drive == "/" || string.IsNullOrEmpty(drive))
                {
                    continue;
                }
                var driveInfo = new DirectoryInfo(drive);
                usbDrives.Add(driveInfo);
                usbDriveTexts.Add(driveInfo.Name);
            }

        }

        private void GetUSBDrives()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                GetDrivesLinux();

                return;
            }

            usbDrives.Clear();
            usbDriveTexts.Clear();

            DriveInfo[] allDrives = DriveInfo.GetDrives();


            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady && drive.DriveType == DriveType.Removable)
                {
                    usbDrives.Add(new DirectoryInfo(drive.Name));
                    usbDriveTexts.Add($"{drive.Name} ({drive.TotalSize.SizeConvert()}, {(drive.TotalFreeSpace * 100) / drive.TotalSize}% free)");
                }
            }
        }

        private string GetBuildVersion()
        {
            DateTime now = DateTime.Now;
            string version = $"{now.Year}.{now.Month:D2}.{now.Day:D2}.{now.Hour:D2}{now.Minute:D2}";
            return version;
        }
    }
}
