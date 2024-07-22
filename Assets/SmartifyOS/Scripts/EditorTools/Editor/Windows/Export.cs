using SmartifyOS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using SmartifyOS.Editor.Styles;
using System;
using SmartifyOS;


public class Export : EditorWindow
{
    [MenuItem("Window/SmartifyOS/SmartifyOS Export")]
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

        GUI.enabled = usbDrives.Count > 0;

        if (GUILayout.Button("Export Installer", Style.Button, GUILayout.Height(30)))
        {

        }

        if (GUILayout.Button("Export Update", Style.Button, GUILayout.Height(30)))
        {

        }

        GUI.enabled = true;

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

        Debug.Log("Found " + allDrives.Length + " drives");


        foreach (DriveInfo drive in allDrives)
        {
            Debug.Log(drive.Name + " " + drive.DriveType + " " + drive.TotalSize + " " + drive.TotalFreeSpace + " " + drive.IsReady + " " + drive.VolumeLabel + " " + drive.DriveFormat);
            if (drive.IsReady && drive.DriveType == DriveType.Removable)
            {
                usbDrives.Add(new DirectoryInfo(drive.Name));
                usbDriveTexts.Add($"{drive.Name} ({drive.TotalSize.SizeConvert()}, {(drive.TotalFreeSpace * 100) / drive.TotalSize}% free)");
            }
        }
    }
}
