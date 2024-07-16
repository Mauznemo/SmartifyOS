using System;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using SmartifyOS.UI.MediaPlayer;
using UnityEngine;

public class AppListUIWindow : BaseUIWindow
{


    [SerializeField] private IconButton androidAutoButton;

    private void Awake()
    {
        androidAutoButton.onClick += () => {Debug.Log("Android Auto Button Clicked");};
    }

    private void Start()
    {
        Init();
    }

    
}
