using SmartifyOS;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using UnityEngine;

public class PowerOffUIWindow : BaseUIWindow
{
    [SerializeField] private Button powerOffButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button cancelButton;

    private void Start()
    {
        Init();
    }

    private void Awake()
    {

        powerOffButton.onClick += () =>
        {
            if (Application.isEditor)
            {
                return;
            }
            string s = LinuxCommand.Run("sudo sleep 5s; sudo shutdown -h now");
            Application.Quit();
        };

        restartButton.onClick += () =>
        {
            if (Application.isEditor)
            {
                return;
            }
            string s = LinuxCommand.Run("sudo sleep 5s; sudo reboot -f");
            Application.Quit();
        };

        cancelButton.onClick += () =>
        {
            Hide();
        };
    }

}
