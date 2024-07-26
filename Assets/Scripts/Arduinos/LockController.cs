using UnityEngine;
using SmartifyOS.SerialCommunication;
using System;
using SmartifyOS.SaveSystem;

public class LockController : BaseSerialCommunication
{
    public static LockController Instance { get; private set; }

    public static event Action OnDoorsLocked;
    public static event Action OnDoorsUnlocked;
    public static event Action OnTrunkLockUnlocked;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        portName = SaveManager.Load().lockController.arduinoPort;
        Debug.Log("Lock controller port: " + portName);
        Init();
        Debug.Log("Lock controller connected: " + IsConnected());
    }

    private void Update()
    {
        ReadMessage();
    }

    public void LockDoors(bool lockDoors)
    {
        Send(lockDoors ? "ld" : "ud");
    }

    public void LockDoors()
    {
        Send("ld");
    }

    public void UnlockDoors()
    {
        Send("ud");
    }

    public void UnlockTrunk()
    {
        Send("ut");
    }

    public override void Received(string message)
    {
        switch (message)
        {
            case "ld":
                OnDoorsLocked?.Invoke();
                break;
            case "ud":
                OnDoorsUnlocked?.Invoke();
                break;
            case "ut":
                OnTrunkLockUnlocked?.Invoke();
                break;
        }
    }
}
