using SmartifyOS.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using SmartifyOS.Editor.Styles;


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

    private List<DriveInfo> usbDrives = new List<DriveInfo>();
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

        if(usbDrives.Count < selectedDrive)
        {
            selectedDrive = 0;
        }

        EditorGUILayout.LabelField("Select Target USB Drive");
        if(usbDrives.Count > 0)
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

    private void GetUSBDrives()
    {
        usbDrives.Clear();
        usbDriveTexts.Clear();

        DriveInfo[] allDrives = DriveInfo.GetDrives();

        foreach (DriveInfo drive in allDrives)
        {
            if (drive.IsReady && drive.DriveType == DriveType.Removable)
            {
                usbDrives.Add(drive);
                usbDriveTexts.Add($"{drive.Name} ({drive.TotalSize.SizeConvert()}, {(drive.TotalFreeSpace * 100) / drive.TotalSize}% free)");
            }
        }
    }
}
