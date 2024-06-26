using UnityEngine;
using SmartifyOS.SerialCommunication;

public class TestLiveSerialCommunication : BaseLiveSerialCommunication
{
    //You can remove this and the code in Awake() if you don't want an instance
    public static TestLiveSerialCommunication Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //portName = "COM1"; //You can also set the port from code
        InitLive();
    }

    private void OnDestroy()
    {
        StopSerialThread();
    }

    public override void Received(string message)
    {
        Debug.Log($"Received: {message}");
    }
}
