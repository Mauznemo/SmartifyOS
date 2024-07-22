using UnityEngine;
using SmartifyOS.QuickSettings;
using SmartifyOS.Notifications;
using SmartifyOS.UI;

public class OpenTrunkQuickSettingsEntry : BaseQuickSettingsEntry
{
    private void Start()
    {
        Init();
    }

    //Executed if this script is on a gameobject together with QuickSettings.Button component
    protected override void OnClick()
    {
        if(LiveDataController.speedKmh < 5)
        {
            ModalWindow.Create().Init("Open Trunk?", "Are you sure you want to open the trunk?", ModalWindow.ModalType.YesNo, () =>{

                LockController.Instance.UnlockTrunk();
            }, () => {});
        }
        else
        {
            NotificationManager.SendNotification(NotificationType.Error, "Can't open trunk while driving");
        }
    }
}