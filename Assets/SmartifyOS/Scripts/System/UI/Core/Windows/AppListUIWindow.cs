using System;
using System.Collections.Generic;
using System.Linq;
using SmartifyOS.Linux;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using SmartifyOS.UI.MediaPlayer;
using UnityEngine;
using UnityEngine.UI;

public class AppListUIWindow : BaseUIWindow
{
    public event Action OnShown;
    public event Action OnHidden;

    [SerializeField] private IconButton androidAutoButton;

    //For controlwheel
    [SerializeField] private List<IconButton> buttons = new List<IconButton>();
    private Outline[] buttonOutlines;
    private int selectedButton = -1;

    private void Awake()
    {
        buttons = GetComponentsInChildren<IconButton>().ToList();
        buttonOutlines = buttons.Select(x => x.GetComponent<Outline>()).ToArray();

        androidAutoButton.onClick += () => { RunLinuxShellScript.Run("~/SmartifyOS/Scripts/StartAndroidAuto.sh"); };
    }

    private void Start()
    {
        Init();
    }

    protected override void OnShow()
    {
        OnShown?.Invoke();

        for (int i = 0; i < buttonOutlines.Length; i++)
        {
            buttonOutlines[i].enabled = false;
        }

        selectedButton = -1;
    }

    protected override void OnHide()
    {
        OnHidden?.Invoke();
    }

    /// <summary>
    /// Scroll though the app list (useful for external input devices like a rotary encoder)
    /// </summary>
    /// <param name="dir">Scroll direction</param>
    public void Scroll(int dir)
    {
        selectedButton = (selectedButton + dir + buttons.Count) % buttons.Count;

        for (int i = 0; i < buttonOutlines.Length; i++)
        {
            buttonOutlines[i].enabled = i == selectedButton;
        }
    }

    /// <summary>
    /// Press the selected button (Change selected with <see cref="Scroll"/>)
    /// </summary>
    public void Press()
    {
        if (selectedButton < 0 || selectedButton >= buttons.Count)
        {
            Hide();
            return;
        }

        buttons[selectedButton].TriggerClick();
    }
}
