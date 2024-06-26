using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIWindow : BaseUIWindow
{
    private void Start()
    {
        Init();
    }

    protected override void HandleWindowOpened(BaseUIWindow window)
    {
        if (window.IsWindowOfType(typeof(Test2UIWindow), typeof(Test3UIWindow)))
        {
            Hide(true);
        }
    }

    protected override void HandleWindowClosed(BaseUIWindow window)
    {
        if(!wasOpen) { return; }

        if (window.IsWindowOfType(typeof(Test2UIWindow), typeof(Test3UIWindow)))
        {
            Show();
        }
    }
}
