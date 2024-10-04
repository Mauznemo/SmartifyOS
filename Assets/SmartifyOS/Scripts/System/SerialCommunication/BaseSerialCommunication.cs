using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;

namespace SmartifyOS.SerialCommunication
{
    public class BaseSerialCommunication : MonoBehaviour
    {
        protected SerialPort serialPort;
        [SerializeField] protected string portName;
        [SerializeField] protected int baudRate = 9600;

        [HideInInspector]
        public bool emulationMode = false;
        [HideInInspector]
        public Action<string> emulationResponse;

        /// <summary>
        /// Initialize the serial communication. Set the <see cref="portName"/> first.
        /// </summary>
        protected void Init()
        {
            if (emulationMode)
            {
                Debug.Log("Emulation Mode Active");
                return;
            }

            if (string.IsNullOrEmpty(portName))
            {
                Debug.Log($"Port for Arduino is null");
                return;
            }

            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
        }

        /// <summary>
        /// Check if the serial connection is open
        /// </summary>
        /// <returns><see cref="true"/> if open/connected</returns>
        public bool IsConnected()
        {
            if (emulationMode)
            {
                return true;
            }

            return serialPort != null && serialPort.IsOpen;
        }

        /// <summary>
        /// Read all messages that where sent from the connected serial device
        /// </summary>
        protected void ReadMessage()
        {
            if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
            {
                string message = serialPort.ReadLine();
                char[] charsToTrim = { ' ', '\n', '\r', '\t' };
                string trimmed = message.Trim(charsToTrim);
                Received(trimmed);
            }
        }

        /// <summary>
        /// Read the latest message that was sent from the connected serial device
        /// </summary>
        protected void ReadLatestMessage()
        {
            if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
            {
                string message = serialPort.ReadLine();
                if (message.Contains("\n"))
                {
                    serialPort.DiscardOutBuffer();
                    serialPort.DiscardInBuffer();
                }

                char[] charsToTrim = { ' ', '\n', '\r', '\t' };
                string trimmed = message.Trim(charsToTrim);

                if (string.IsNullOrEmpty(trimmed))
                {
                    return;
                }

                Received(trimmed);
            }
        }

        /// <summary>
        /// Send a message to the connected serial device
        /// </summary>
        /// <param name="message">Message to send</param>
        public void Send(string message)
        {
            if (emulationMode)
            {
                emulationResponse?.Invoke(message);
                return;
            }

            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Write(message + "\n");
            }
            else
            {
                Debug.Log($"Arduino on {portName} not connected!");
            }
        }

        /// <summary>
        /// Gets called every time a a new message was received from the serial device
        /// </summary>
        /// <param name="message">Received message</param>
        public virtual void Received(string message)
        {

        }

        void OnApplicationQuit()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}

