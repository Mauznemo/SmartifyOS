using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowTest : MonoBehaviour
{
    [SerializeField] private Button[] windowButtons;

    [SerializeField] private BaseUIWindow[] windows;

    private void Awake()
    {
        for (int i = 0; i < windows.Length; i++)
        {
            int index = i;
            windowButtons[i].onClick.AddListener(() =>
            {
                var window = windows[index];
                window.Show();
            });
        }
    }
}
