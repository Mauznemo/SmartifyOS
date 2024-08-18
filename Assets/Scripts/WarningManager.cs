using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Audio;
using SmartifyOS.SaveSystem;
using SmartifyOS.StatusBar;
using UnityEngine;

public class WarningManager : MonoBehaviour
{
    [SerializeField] private Sprite WarningIconSprite;

    private StatusBar.StatusEntry statusEntry;

    private void Start()
    {
        statusEntry = StatusBar.AddStatus(WarningIconSprite);
        statusEntry.Hide();

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
        if (open && SaveManager.Load().notifications.trunkWarningWhenDriving)
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
        if (open && SaveManager.Load().notifications.doorWarningWhenDriving)
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
        if (open && SaveManager.Load().notifications.doorWarningWhenDriving)
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
        if (AudioManager.playingWarningSound || !LiveDataController.isDriving)
            return;

        if (!MainController.leftDoorOpen && !MainController.rightDoorOpen && SaveManager.Load().notifications.doorWarningWhenDriving)
            return;

        if (!MainController.trunkOpen && SaveManager.Load().notifications.trunkWarningWhenDriving)
            return;

        StartWarningSound();
    }

    private void TryStopWarningSound()
    {
        if ((MainController.leftDoorOpen || MainController.rightDoorOpen) && SaveManager.Load().notifications.doorWarningWhenDriving)
            return;

        if (MainController.trunkOpen && SaveManager.Load().notifications.trunkWarningWhenDriving)
            return;

        StopWarningSound();
    }

    private void StartWarningSound()
    {
        AudioManager.Instance.StartWarningSound();

        statusEntry.Show();
    }

    private void StopWarningSound()
    {
        AudioManager.Instance.StopWarningSound();

        statusEntry.Hide();
    }
}
