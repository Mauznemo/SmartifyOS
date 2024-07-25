using System.Threading;

namespace SmartifyOS.SerialCommunication
{
    public class BaseLiveSerialCommunication : BaseSerialCommunication
    {
        private Thread serialThread;
        private bool isRunning = false;

        /// <summary>
        /// Initialize the live serial communication on it's own thread. Set the <see cref="portName"/> first.
        /// </summary>
        protected void InitLive()
        {
            Init();

            if (emulationMode)
            {
                isRunning = true;
                return;
            }

            StartSerialThread();
        }

        private void StartSerialThread()
        {
            if (isRunning)
                return;

            isRunning = true;
            serialThread = new Thread(ReadSerialData);
            serialThread.Start();
        }

        private void ReadSerialData()
        {
            while (isRunning)
            {
                ReadLatestMessage();

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Stop the thread
        /// </summary>
        protected void StopSerialThread()
        {
            isRunning = false;
            if (emulationMode)
                return;
            serialThread.Join();
        }
    }
}


