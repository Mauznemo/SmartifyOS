using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Notifications;
using SmartifyOS.SaveSystem;
using UnityEngine;

public class LedManager : MonoBehaviour
{
    public static LedManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public static bool interiorLightOn { get; private set; }

    private void Start()
    {
        MainController.OnLeftDoorOpened += MainController_OnLeftDoorOpened;
        MainController.OnRightDoorOpened += MainController_OnRightDoorOpened;

        StartCoroutine(SetToSavedColor());
    }

    private void MainController_OnRightDoorOpened(bool open)
    {
        if (SaveManager.Load().interiorLighting.activateLedStripOnDoorOpen)
        {
            if (open)
            {
                SetLedColorWithoutSave(LedStrip.Right, Color.white);
            }
            else
            {
                SetLedColorWithoutSave(LedStrip.Right, GetSavedColor(LedStrip.Right));
            }
        }

        if (SaveManager.Load().interiorLighting.activateLightOnDoorOpen)
        {
            if (open)
            {
                SetInteriorLight(true);
            }
            else if (!MainController.leftDoorOpen)
            {
                SetInteriorLight(false);
            }
        }
    }

    private void MainController_OnLeftDoorOpened(bool open)
    {
        if (SaveManager.Load().interiorLighting.activateLedStripOnDoorOpen)
        {
            if (open)
            {
                SetLedColorWithoutSave(LedStrip.Left, Color.white);
            }
            else
            {
                SetLedColorWithoutSave(LedStrip.Left, GetSavedColor(LedStrip.Left));
            }
        }

        if (SaveManager.Load().interiorLighting.activateLightOnDoorOpen)
        {
            if (open)
            {
                SetInteriorLight(true);
            }
            else if (!MainController.rightDoorOpen)
            {
                SetInteriorLight(false);
            }
        }
    }

    private IEnumerator SetToSavedColor()
    {
        yield return new WaitForSeconds(1f);

        SetLedColor(LedStrip.Left, GetSavedColor(LedStrip.Left));
        SetLedColor(LedStrip.Right, GetSavedColor(LedStrip.Right));

        yield return new WaitForSeconds(0.2f);

        MainController_OnRightDoorOpened(MainController.rightDoorOpen);
        MainController_OnLeftDoorOpened(MainController.leftDoorOpen);
    }

    public Color GetSavedColor(LedStrip ledStrip)
    {
        switch (ledStrip)
        {
            case LedStrip.Left:
                var colorL = SaveManager.Load().interiorLighting.leftFeet;
                if (colorL == null)
                {
                    SaveManager.Load().interiorLighting.leftFeet = Color.magenta;
                    colorL = Color.magenta;
                }
                return colorL.Value;
            case LedStrip.Right:
                var colorR = SaveManager.Load().interiorLighting.rightFeet;
                if (colorR == null)
                {
                    SaveManager.Load().interiorLighting.rightFeet = Color.magenta;
                    colorR = Color.magenta;
                }
                return colorR.Value;
            default:
                throw new ArgumentOutOfRangeException(nameof(ledStrip), ledStrip, null);
        }
    }

    public void SetInteriorLight(bool on)
    {
        interiorLightOn = on;
        MainController.Instance.ActivateLight(on);
    }

    public void SetLedColor(LedStrip ledStrip, Color color)
    {
        MainController.Instance.SetLedColor(color, ledStrip);

        switch (ledStrip)
        {
            case LedStrip.Left:
                SaveManager.Load().interiorLighting.leftFeet = color;
                break;
            case LedStrip.Right:
                SaveManager.Load().interiorLighting.rightFeet = color;
                break;
        }

    }

    public void SetLedColorWithoutSave(LedStrip ledStrip, Color color)
    {
        MainController.Instance.SetLedColor(color, ledStrip);
    }
}


public enum LedStrip
{
    Left,
    Right,
}