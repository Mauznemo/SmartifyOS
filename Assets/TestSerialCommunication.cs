using UnityEngine;
using SmartifyOS.SerialCommunication;
using SmartifyOS.StatusBar;
using SmartifyOS.Notifications;

public class TestSerialCommunication : BaseSerialCommunication
{
    [SerializeField] private Sprite noGpsSprite;

    private StatusBar.StatusEntry noGpsStatusEntry;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        ReadMessage();
    }

    public override void Received(string message)
    {
        Debug.Log($"Received: {message}");

        if (message == "no-gps" && noGpsStatusEntry == null)
        {
            noGpsStatusEntry = StatusBar.AddStatus(noGpsSprite);

            NotificationManager.SendNotification(NotificationType.Warning, "No GPS signal found");
        }
        else if(message == "gps")
        {
            noGpsStatusEntry?.Remove();
        }
    }
}
