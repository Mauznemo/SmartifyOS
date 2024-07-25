using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using UnityEngine;

public class WarningManager : MonoBehaviour
{
    private void Start()
    {
        MainController.OnRightDoorOpened += MainController_OnRightDoorOpened;
        MainController.OnLeftDoorOpened += MainController_OnLeftDoorOpened;
        MainController.OnTrunkOpened += MainController_OnTrunkOpened;

        LiveDataController.OnStartedDriving += LiveDataController_OnStartedDriving;
        LiveDataController.OnStoppedDriving += LiveDataController_OnStoppedDriving;
    }

    private void LiveDataController_OnStoppedDriving()
    {
        StopWarningSound();
    }

    private void LiveDataController_OnStartedDriving()
    {
        TryStartWarningSound();
    }

    private void MainController_OnTrunkOpened(bool open)
    {
        if (open)
        {
            TryStartWarningSound();
        }
        else
        {
            TryStopWarningSound();
        }
    }

    private void MainController_OnLeftDoorOpened(bool open)
    {
        if (open)
        {
            TryStartWarningSound();
        }
        else
        {
            TryStopWarningSound();
        }
    }

    private void MainController_OnRightDoorOpened(bool open)
    {
        if (open)
        {
            TryStartWarningSound();
        }
        else
        {
            TryStopWarningSound();
        }
    }

    private void TryStartWarningSound()
    {
        if (AudioManager.playingWarningSound)
            return;

        if (!MainController.leftDoorOpen && !MainController.rightDoorOpen && !MainController.trunkOpen)
            return;

        StartWarningSound();
    }

    private void TryStopWarningSound()
    {
        if (MainController.leftDoorOpen || MainController.rightDoorOpen || MainController.trunkOpen)
            return;

        StopWarningSound();
    }

    private void StartWarningSound()
    {
        AudioManager.Instance.StartWarningSound();
    }

    private void StopWarningSound()
    {
        AudioManager.Instance.StopWarningSound();
    }
}
