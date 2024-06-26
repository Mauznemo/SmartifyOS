using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseWindowButton : Button
{
    protected override void Awake()
    {
        base.Awake();

        onClick.AddListener(() =>
        {
            var window = GetComponentInParent<BaseUIWindow>();
            if (window == null)
            {
                throw new System.NullReferenceException("The Close Window Button couldn't find a BaseUIWindow in its parent!");
            }

            window.Hide();
        });
    }
}
