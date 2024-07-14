using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text firstText;
    [SerializeField] private TMP_Text secondText;

    [SerializeField] private IconButton switchButton;

    private bool isMain = true;

    private void Awake()
    {
        switchButton.onClick += () =>
        {
            isMain = !isMain;
            string textOne = firstText.text;
            string textTwo = secondText.text;
            firstText.text = textTwo;
            secondText.text = textOne;
        };
    }

    public void SetFirstText(string text)
    {
        if (isMain)
            firstText.text = text;
        else
            secondText.text = text;
    }

    public void SetSecondText(string text)
    {
        if (!isMain)
            firstText.text = text;
        else
            secondText.text = text;
    }
}
