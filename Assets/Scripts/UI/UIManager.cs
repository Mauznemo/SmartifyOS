using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI;
using UnityEngine;

public class UIManager : BaseUIManager
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
