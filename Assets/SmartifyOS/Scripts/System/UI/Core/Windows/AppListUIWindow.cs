using System;
using SmartifyOS.Linux;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using SmartifyOS.UI.MediaPlayer;
using UnityEngine;

public class AppListUIWindow : BaseUIWindow
{


    [SerializeField] private IconButton androidAutoButton;

    private void Awake()
    {
        androidAutoButton.onClick += () => { RunLinuxShellScript.Run("~/SmartifyOS/Scripts/StartAndroidAuto.sh"); };
    }

    private void Start()
    {
        Init();
    }


}
