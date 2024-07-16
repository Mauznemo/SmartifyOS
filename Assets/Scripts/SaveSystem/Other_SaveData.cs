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
    }

    //You can add new structs here
    public struct Statistics
    {
        public float topSpeed;
        public double totalMinutes;
    }

    public struct InteriorLighting
    {
        public Color leftFeet;
        public Color rightFeet;

        public bool activateLightOnDoorOpen;
        public bool activateLedStripOnDoorOpen;
    }

    public struct Popups
    {
        public bool allowModifyingWhileOn;
    }

    public struct Camera
    {
        public int currentCameraIndex;
        public bool autoFullscreen;
    }

    public partial class EventPaths
    {
        public string setReverseCamConverter;
    }
}