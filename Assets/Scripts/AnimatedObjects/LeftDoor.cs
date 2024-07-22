using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Animation;
using UnityEngine;

public class LeftDoor : BaseAnimatedObject
{
    private void Start()
    {
        MainController.OnLeftDoorOpened += MainController_OnLeftDoorOpened;
    }

    private void MainController_OnLeftDoorOpened(bool open)
    {
        RotateY(open ? 220 : 180, 0.7f);
    }
}
