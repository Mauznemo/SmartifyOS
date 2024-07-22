using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Animation;
using UnityEngine;

public class LeftPopupLight : BaseAnimatedObject
{
    private void Start()
    {
        LightController.Instance.OnLeftLightStateChanged += LightController_OnLeftLightStateChanged;
    }

    private void LightController_OnLeftLightStateChanged(LightController.LightState state)
    {
        RotateX(state == LightController.LightState.Up ? 54 : 0, 0.6f);
    }
}
