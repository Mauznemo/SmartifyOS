using System;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using SmartifyOS.UI.MediaPlayer;
using UnityEngine;

public class AppListUIWindow : BaseUIWindow
{
    [SerializeField] private Type type;
    [SerializeField] private IconButton androidAutoButton;
    [SerializeField] private IconButton reverseCameraButton;
    [SerializeField] private IconButton realtimeInfoButton;
    [SerializeField] private IconButton ambientButton;
    [SerializeField] private IconButton mediaPlayerButton;
    [SerializeField] private IconButton settingsButton;

    private void Awake()
    {
        mediaPlayerButton.onClick += () => {UIManager.Instance.ShowUIWindow<FilePlayer>(); Hide(); };
        settingsButton.onClick += () => {UIManager.Instance.ShowUIWindow<SettingsUIWindow>(); Hide(); };
    }

    private void Start()
    {
        Init();
    }

    
}
