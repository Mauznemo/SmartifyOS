using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Animation;
using UnityEngine;

public class RightPopupLight : BaseAnimatedObject
{
    private void Start()
    {
        LightController.Instance.OnRightLightStateChanged += LightController_OnRightLightStateChanged;
    }

    private void LightController_OnRightLightStateChanged(LightController.LightState state)
    {
        RotateX(state == LightController.LightState.Up ? 54 : 0, 0.6f);
    }
}
