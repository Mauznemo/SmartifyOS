using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonManager : MonoBehaviour
{
    private bool canTriggerAction1 = true;
    private bool canTriggerAction2 = true;
    private float cooldown = 0.5f;

    private ActionMode actionMode = ActionMode.WinkLights;

    private void Start()
    {
        MainController.OnActionButton1 += MainController_OnActionButton1;
        MainController.OnActionButton2 += MainController_OnActionButton2;
    }

    private void MainController_OnActionButton1(bool down)
    {
        if (!canTriggerAction1) { return; }

        if (!down) { return; }

        switch (actionMode)
        {
            case ActionMode.WinkLights:
                LightController.Instance.WinkLeft();
                break;
            case ActionMode.Volume:
                throw new NotImplementedException();
            case ActionMode.Slider:
                throw new NotImplementedException();
        }

        StartCoroutine(StartAction1Cooldown());
    }

    private void MainController_OnActionButton2(bool down)
    {
        if (!canTriggerAction2) { return; }

        if (!down) { return; }

        switch (actionMode)
        {
            case ActionMode.WinkLights:
                LightController.Instance.WinkRight();
                break;
            case ActionMode.Volume:
                throw new NotImplementedException();
            case ActionMode.Slider:
                throw new NotImplementedException();
        }

        StartCoroutine(StartAction2Cooldown());
    }

    private IEnumerator StartAction1Cooldown()
    {
        canTriggerAction1 = false;
        yield return new WaitForSeconds(cooldown);
        canTriggerAction1 = true;
    }

    private IEnumerator StartAction2Cooldown()
    {
        canTriggerAction2 = false;
        yield return new WaitForSeconds(cooldown);
        canTriggerAction2 = true;
    }

    public enum ActionMode
    {
        WinkLights,
        Volume,
        Slider,
    }

}
