namespace SmartifyOS.SaveSystem
{
    public partial class SaveData
    {
        //Add your custom structs here as a public variable
        public LightController lightController;
        public MainController mainController;
        public LiveController liveController;
    }

    //You can add new structs here
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
}