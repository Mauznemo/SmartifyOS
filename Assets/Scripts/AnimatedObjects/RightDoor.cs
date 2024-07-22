using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Animation;
using UnityEngine;

public class RightDoor : BaseAnimatedObject
{
    private void Start()
    {
        MainController.OnRightDoorOpened += MainController_OnRightDoorOpened;
    }

    private void MainController_OnRightDoorOpened(bool open)
    {
        RotateY(open ? 140 : 180, 0.7f);
    }
}
