using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Animation;
using UnityEngine;

public class Trunk : BaseAnimatedObject
{
    private void Start()
    {
        MainController.OnTrunkOpened += MainController_OnTrunkOpened;
    }

    private void MainController_OnTrunkOpened(bool open)
    {
        RotateX(open ? -54 : 0, 0.6f);
    }
}
