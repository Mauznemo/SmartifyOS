using UnityEngine;

namespace SmartifyOS.SaveSystem
{
    public partial class SaveData
    {
        //Add your custom structs here as a public variable
        public Statistics statistics;
        public InteriorLighting interiorLighting;
        public Popups popups;
        public Camera camera;
        public Timezone timezone;
        public Notifications notifications = new Notifications();
    }

    //You can add new structs here
    public struct Statistics
    {
        public float topSpeed;
        public double totalMinutes;
    }

    public struct InteriorLighting
    {
        public Color? leftFeet;
        public Color? rightFeet;

        public bool activateLightOnDoorOpen;
        public bool activateLedStripOnDoorOpen;
    }

    public struct Popups
    {
        public bool autoCloseOnPowerOff;
        public bool allowModifyingWhileOn;
    }

    public struct Camera
    {
        public int currentCameraIndex;
        public bool autoFullscreen;
    }

    public struct Timezone
    {
        public int hourOffset;
        public int minuteOffset;
    }

    public class Notifications
    {
        public bool doorWarningWhenDriving = true;
        public bool trunkWarningWhenDriving = true;
        public bool ignoreErrors;
    }
}