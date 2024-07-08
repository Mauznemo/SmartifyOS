using UnityEngine;

namespace SmartifyOS.SaveSystem
{
    public partial class Other_SaveData
    {
        //Add your custom structs here as a public variable
        public Statistics statistics;
        public InteriorLighting interiorLighting;
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
}