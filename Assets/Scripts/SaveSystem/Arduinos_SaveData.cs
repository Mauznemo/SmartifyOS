namespace SmartifyOS.SaveSystem
{
    public partial class SaveData
    {
        public LightController lightController;
        public MainController mainController;
        public LiveController liveController;
        public LockController lockController;
    }

    public struct LiveController
    {
        public string arduinoPort;
    }

    public struct LightController
    {
        public string arduinoPort;
    }

    public struct MainController
    {
        public string arduinoPort;
    }

    public struct LockController
    {
        public string arduinoPort;
    }
}