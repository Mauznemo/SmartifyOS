using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private void Start()
    {
        var text = GetComponent<TMP_Text>();
        text.text = $"Build: {Application.version}";
    }
}
