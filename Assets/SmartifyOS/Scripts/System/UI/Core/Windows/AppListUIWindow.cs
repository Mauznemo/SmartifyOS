using System;
using System.Linq;
using SmartifyOS.Linux;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using SmartifyOS.UI.MediaPlayer;
using UnityEngine;
using UnityEngine.UI;

public class AppListUIWindow : BaseUIWindow
{
    [SerializeField] private IconButton androidAutoButton;

    //For controlwheel
    [SerializeField] private IconButton[] buttons;
    private Outline[] buttonOutlines;
    private int selectedButton = -1;

    private void Awake()
    {
        buttonOutlines = buttons.Select(x => x.GetComponent<Outline>()).ToArray();

        androidAutoButton.onClick += () => { RunLinuxShellScript.Run("~/SmartifyOS/Scripts/StartAndroidAuto.sh"); };
    }

    private void Start()
    {
        Init();

        ControlwheelManager.OnButtonPressed += ControlwheelManager_OnButtonPressed;
        ControlwheelManager.OnChanged += ControlwheelManager_OnChanged;
    }

    protected override void OnShow()
    {
        for (int i = 0; i < buttonOutlines.Length; i++)
        {
            buttonOutlines[i].enabled = false;
        }

        selectedButton = -1;

        ControlwheelManager.SetMode(ControlwheelManager.Mode.AppList);
    }

    protected override void OnHide()
    {
        ControlwheelManager.SetDefaultMode();
    }

    private void ControlwheelManager_OnButtonPressed()
    {
        if (ControlwheelManager.GetMode() != ControlwheelManager.Mode.AppList)
        { return; }

        if (selectedButton < 0 || selectedButton >= buttons.Length)
        {
            ControlwheelManager.SetDefaultMode();
            Hide();
            return;
        }

        ControlwheelManager.SetDefaultMode();
        buttons[selectedButton].TriggerClick();
    }

    private void ControlwheelManager_OnChanged(int dir)
    {
        if (ControlwheelManager.GetMode() != ControlwheelManager.Mode.AppList)
        { return; }

        selectedButton = (selectedButton + dir + buttons.Length) % buttons.Length;

        for (int i = 0; i < buttonOutlines.Length; i++)
        {
            buttonOutlines[i].enabled = i == selectedButton;
        }
    }
}
