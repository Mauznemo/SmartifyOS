using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Linux;
using SmartifyOS.UI.Components;
using UnityEngine;

public class TemAppLauncher : MonoBehaviour
{
    [SerializeField] private IconButton androidAutoButton;


    private void Awake()
    {
        androidAutoButton.onClick += () =>
        {
            RunLinuxShellScript.Run("~/SmartifyOS/android-auto/start.sh");
        };
    }
}
