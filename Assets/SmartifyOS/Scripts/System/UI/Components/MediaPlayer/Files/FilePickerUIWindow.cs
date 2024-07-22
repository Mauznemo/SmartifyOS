using SmartifyOS.UI.Components;
using SmartifyOS.UI;
using UnityEngine;
using SmartifyOS;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SmartifyOS.UI.MediaPlayer;
using TMPro;
using System;

public class FilePickerUIWindow : BaseUIWindow
{
    [SerializeField] private FilePlayer filePlayer;
    [SerializeField] private TMP_Text searchingText;

    [SerializeField] private List<FileDir> dirList;

    [SerializeField] private List<DirectoryInfo> drivesList = new List<DirectoryInfo>();

    [SerializeField] private GameObject selectDriveScreen;
    [SerializeField] private GameObject selectFileScreen;

    [SerializeField] private Button driveButtonPrefab;
    [SerializeField] private Transform driveParent;

    [SerializeField] private Button folderButtonPrefab;
    [SerializeField] private Button fileButtonPrefab;
    [SerializeField] private Transform parent;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    protected override void OnShow()
    {
        GetDrivesLinux();
    }

    private void GetDrivesLinux()
    {
        DestroySpawnedObjects();

        selectDriveScreen.SetActive(true);
        selectFileScreen.SetActive(false);

        drivesList.Clear();

        var drivesString = LinuxCommand.Run("df -h | grep -E '^/dev/' | awk '{print $NF}'");
        string[] drives = drivesString.Split('\n');


        foreach (var drive in drives)
        {
            if (drive == "/boot/efi" || string.IsNullOrEmpty(drive))
            {
                continue;
            }
            var driveInfo = new DirectoryInfo(drive);
            drivesList.Add(driveInfo);
        }


        foreach (var drive in drivesList)
        {
            Button driveButton = Instantiate(driveButtonPrefab, driveParent);
            driveButton.gameObject.SetActive(true);
            driveButton.text = drive.Name;
            driveButton.onClick += () => SelectDrive(drive.FullName);
            spawnedObjects.Add(driveButton.gameObject);
        }

    }

    private void SelectDrive(string fullName)
    {
        selectDriveScreen.SetActive(false);
        selectFileScreen.SetActive(true);
        SearchAudioFiles(fullName);
    }

    private void SearchAudioFiles(string searchDir)
    {
        searchingText.gameObject.SetActive(true);
        
        DestroySpawnedObjects();

        string files = LinuxCommand.Run($"find {searchDir} -type f \\( -iname \"*.mp3\" -o -iname \"*.flac\" -o -iname \"*.wav\" -o -iname \"*.aac\" -o -iname \"*.ogg\" -o -iname \"*.m4a\" \\)");

        string[] fileArray = files.Split('\n');
        Debug.Log("FileArray Length: " + fileArray.Length);
        dirList = GroupFilesByDirectory(fileArray);

        foreach (var dir in dirList)
        {
            Button folderButton = Instantiate(folderButtonPrefab, parent);
            folderButton.gameObject.SetActive(true);
            folderButton.text = new FileInfo(dir.path).Name;
            spawnedObjects.Add(folderButton.gameObject);

            folderButton.onClick += () => OpenFolder(dir.path);
        }

        searchingText.gameObject.SetActive(false);
    }

    private void OpenFolder(string path)
    {
        FileDir? directory = GetDirectory(path, dirList);
        if (directory == null) { return; }

        foreach (var button in spawnedObjects)
        {
            Destroy(button);
        }

        spawnedObjects.Clear();

        for (int i = 0; i < directory?.files.Count; i++)
        {
            Button button = Instantiate(fileButtonPrefab, parent);
            button.gameObject.SetActive(true);
            button.text = directory?.files[i];
            spawnedObjects.Add(button.gameObject);

            var filePath = directory?.path + "/" + directory?.files[i];

            button.onClick += () =>
            {
                filePlayer.SelectAndPlay(filePath);
                foreach (var button in spawnedObjects)
                {
                    Destroy(button);
                }

                spawnedObjects.Clear();
                Hide();
            };
        }
    }

    private void DestroySpawnedObjects()
    {
        foreach (var button in spawnedObjects)
        {
            Destroy(button);
        }

        spawnedObjects.Clear();
    }
    private void Start()
    {
        Init();
    }

    protected override void HandleWindowOpened(BaseUIWindow window)
    {
        //Add all windows that should hide this window when they open
        //if (window.IsWindowOfType(typeof(UIWindow1), typeof(UIWindow2)))
        //{
        //    Hide(true);
        //}
    }

    protected override void HandleWindowClosed(BaseUIWindow window)
    {
        if (!wasOpen) { return; }

        //Add all windows that should trigger this window to reopen when they close
        //if (window.IsWindowOfType(typeof(UIWindow1), typeof(UIWindow2)))
        //{
        //    Show();
        //}
    }

    public static FileDir? GetDirectory(string path, List<FileDir> dirList)
    {
        // Search for the directory with the given path
        var directory = dirList.FirstOrDefault(d => d.path == path);
        if (directory.path == null)
        {
            // Return null if not found
            return null;
        }
        return directory;
    }

    public static List<FileDir> GroupFilesByDirectory(string[] fileArray)
    {
        List<FileDir> dirList = new List<FileDir>();

        // Iterate over each file path and log the directory and file name
        foreach (var filePath in fileArray)
        {
            if (!File.Exists(filePath) || filePath.Contains("/.") || filePath.ToLower().Contains("cache"))
            {
                continue;
            }

            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            if (directory == null || fileName == null)
            {
                continue; // Skip invalid paths
            }

            var existingDir = dirList.FirstOrDefault(d => d.path == directory);
            if (existingDir.path == null)
            {
                // Create new FileDir and add it to the list
                FileDir fileDir = new FileDir
                {
                    path = directory,
                    files = new List<string> { fileName }
                };
                dirList.Add(fileDir);
            }
            else
            {
                // Add the file to the existing FileDir
                existingDir.files.Add(fileName);
            }
        }

        return dirList;
    }

    [System.Serializable]
    public struct FileDir
    {
        public string path;
        public List<string> files; //only the names not full path
    }
}
