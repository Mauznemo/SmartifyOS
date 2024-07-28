using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [SerializeField] private Transform steeringWheel;
    [SerializeField] private Transform[] frontWheelParts;
    [SerializeField] private Transform[] wheels;

    [SerializeField] private float smoothingSteering = 5f;
    [SerializeField] private float wheelRotationSpeedMultiplier = 6f;

    private void Update()
    {
        Quaternion stRot = steeringWheel.transform.localRotation;
        Quaternion targetRot = Quaternion.Euler(stRot.eulerAngles.x, stRot.eulerAngles.y, LiveDataController.steeringWheelAngle);
        steeringWheel.transform.localRotation = Quaternion.Slerp(stRot, targetRot, smoothingSteering * Time.deltaTime);

        float speed = MainController.isInReverse ? -LiveDataController.speedKmh : LiveDataController.speedKmh;

        float rotationAngle = speed * Time.deltaTime * wheelRotationSpeedMultiplier;

        for (int i = 0; i < wheels.Length; i++)
        {
            if (i < 2)
                wheels[i].Rotate(-rotationAngle, 0, 0, Space.Self);
            else
                wheels[i].Rotate(rotationAngle, 0, 0, Space.Self);

        }

        for (int i = 0; i < frontWheelParts.Length; i++)
        {
            Vector3 rot = frontWheelParts[i].transform.localEulerAngles;
            frontWheelParts[i].transform.localEulerAngles = new Vector3(rot.x, LiveDataController.wheelAngle, rot.z);
        }
    }
}
