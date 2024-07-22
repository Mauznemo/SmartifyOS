using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartifyOS.Animation
{
    public class BaseAnimatedObject : MonoBehaviour
    {
        public void RotateY(float value, float duration = 0.5f)
        {
            var rot = transform.localEulerAngles;
            LeanTween.rotateLocal(gameObject, new Vector3(rot.x, value, rot.z), duration).setEaseInOutCubic(); ;
        }

        public void RotateX(float value, float duration = 0.5f)
        {
            var rot = transform.localEulerAngles;
            LeanTween.rotateLocal(gameObject, new Vector3(value, rot.y, rot.z), duration).setEaseInOutCubic(); ;
        }

        public void RotateZ(float value, float duration = 0.5f)
        {
            var rot = transform.localEulerAngles;
            LeanTween.rotateLocal(gameObject, new Vector3(rot.x, rot.y, value), duration).setEaseInOutCubic(); ;
        }
    }

}

