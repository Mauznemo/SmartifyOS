using UnityEngine;
using SmartifyOS.SerialCommunication;

public class MainController : BaseSerialCommunication
{
    //You can remove this and the code in Awake() if you don't want an instance
    public static MainController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //portName = "COM1"; //You can also set the port from code
        Init();
    }

    private void Update()
    {
        ReadMessage();
    }

    public override void Received(string message)
    {
        Debug.Log($"Received: {message}");
    }
}
