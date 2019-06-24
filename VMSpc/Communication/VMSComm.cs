﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using VMSpc.DevHelpers;
using System.Windows;
using System.Timers;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using VMSpc.Parsers;
using static VMSpc.Constants;


namespace VMSpc.Communication
{
    public class VMSComm
    {
        //Data readers
        private SerialPort portReader;
        private Socket wifiReader;
        private StreamReader logReader;

        private Timer portCheckTimer;
        private Timer logReadTimer;
        private Timer keepJibAwakeTimer;

        private Action InitializeDataReader;
        private Dictionary<int, Action> dataReaderMap;

        private int dataReaderType;
        public int DataReaderType
        {
            get { return dataReaderType; }
            set { ChangeDataReader(value); }
        }

        private string portString;
        private int comPort;
        public int ComPort
        {
            get { return comPort; }
            set { ChangeComPort(value); }
        }

        private ulong messageCount;
        public string MessageCount { get { return ("" + messageCount); } }
        private ulong lastMessageCount;
        private ulong badMessageCount;
        public string BadMessageCount { get { return ("" + badMessageCount); } }
        private int parseBehavior;
        public int ParseBehavior { get { return parseBehavior; } set { ChangeParseBehavior(value); } }


        private MessageExtractor extractor;

        private J1708Parser j1708Parser;
        private J1939Parser j1939Parser;


        private string logPlayerFile;
        public string LogPlayerFile { get { return logPlayerFile; } set { ChangeLogPlayerFile(value); } }

        public string LogRecordingFile;
        public bool LogRecordingEnabled;
        public byte LogType;


        public VMSComm()
        {
            messageCount = 0;
            lastMessageCount = 0;
            badMessageCount = 0;
            extractor = new MessageExtractor();

            LogRecordingEnabled = false;
            LogType = LOGTYPE_RAWLOG;
            LogRecordingFile = null; //CHANGEME - retrieve from config

            dataReaderType = USB;
            dataReaderMap = new Dictionary<int, Action>
            {
                { USB, InitPortReader },
                { SERIAL, InitPortReader },
                { WIFI, null },
                { LOGPLAYER, InitLogReader }
            };

            comPort = 9;
            portString = "COM10"; //CHANGEME - port should be retrieved from config or inferred. User should also be able to override
            logPlayerFile = "j1939log.vms";   //CHANGEME - should rely on user input
            parseBehavior = PARSE_ALL;  //CHANGEME - should initially come from config

            j1939Parser = new J1939Parser();
            j1708Parser = new J1708Parser();

            SetDataReader();
        }

        ~VMSComm()
        {
            CloseDataReader();
        }

        #region Business Logic

        /// <summary>   Initializes all communications activity. Begins the InitializeDataReader() thread and sends messages to the JIB to keep it in VMS mode  </summary>
        public void StartComm()
        {
            try
            {
                InitializeDataReader();
                //InitLogReader();
                KeepJibAwake(null, null);
                keepJibAwakeTimer = CREATE_TIMER(KeepJibAwake, 10000);
            }
            catch { } // CHANGEME - put something useful here
        }

        /// <summary>
        /// Receives the message from the data reader, gets the message as a CanMessage from the MessageExtractor, and passes the parsed message to the appropriate parser
        /// </summary>
        private void ProcessData(string message)
        {
            CanMessage canMessage = extractor.GetMessage(message);
            if (LogRecordingEnabled)
                AddLogRecord(message, canMessage);
            if (canMessage == null || canMessage.messageType == INVALID_CAN_MESSAGE)
            {
                badMessageCount++;
                return;
            }
            if (canMessage.messageType == J1939 && parseBehavior != IGNORE_1939)
                j1939Parser.Parse((J1939Message)canMessage);
            else if (canMessage.messageType == J1708 && parseBehavior != IGNORE_1708)
                j1708Parser.Parse((J1708Message)canMessage);
            messageCount++;
        }

        private void AddLogRecord(string message, CanMessage canMessage)
        {
            string logEntry = "" + message;
            if (LogType == LOGTYPE_PARSEREADY || LogType == LOGTYPE_FULL)
                logEntry += canMessage.ToString();
            if (LogType == LOGTYPE_FULL)
            {
                if (canMessage.messageType == J1939 && ParseBehavior != IGNORE_1939)
                    logEntry += canMessage.ToParsedString(j1939Parser);
                else if (canMessage.messageType == J1708 && ParseBehavior != IGNORE_1708)
                    logEntry += canMessage.ToParsedString(j1708Parser);
            }
            logEntry += "\nEnd of Message\n\n";
            using (StreamWriter logWriter = new StreamWriter(LogRecordingFile, true))
                logWriter.WriteLine(logEntry);
        }

        #endregion //Business Logic

        #region Communication Settings

        /// <summary>   Closes any existing connections and sets portReader, logReader, and wifiReader to null  </summary>
        private void CloseDataReader()
        {
            System.Threading.Thread ClosingThread = null;
            if (portReader != null && portReader.IsOpen)
                ClosingThread = new System.Threading.Thread(ClosePortReader);
            if (logReader != null)
            {
                logReadTimer.Dispose();
                logReader.Close();
            }
            if (wifiReader != null)
            {
                wifiReader.Shutdown(SocketShutdown.Both);
                wifiReader.Close();
            }
            if (ClosingThread != null)
                ClosingThread.Start();
            logReader = null;
            wifiReader = null;
        }

        /// <summary>   Sets the data reader to either InitPortReader(), InitWifiReader(), or InitLogReader(), depending on the current dataReaderType  </summary>
        private void SetDataReader()
        {
            InitializeDataReader = dataReaderMap[dataReaderType];
        }

        private void ChangeComPort(int newPort)
        {
            if (newPort == comPort || dataReaderType != USB)
                return;
            CloseDataReader();
            comPort = newPort;
            //newPort + 1, because it comes in from a dropdown (0-indexed), but first value is "COM1"
            portString = "COM" + (newPort + 1);
            StartComm();
        }

        private void ChangeLogPlayerFile(string filename)
        {
            logPlayerFile = filename;
            if (dataReaderType == LOGPLAYER)
            {
                CloseDataReader();
                SetDataReader();
                StartComm();
            }
        }

        /// <summary>   Changes the I/O channel to either Wifi, RS-232, USB, or Logplayer   </summary>
        public void ChangeDataReader(int newType)
        {
            if (newType == dataReaderType)
                return;
            CloseDataReader();
            dataReaderType = newType;
            SetDataReader();
            StartComm();
        }

        private void ChangeParseBehavior(int newBehavior)
        {
            parseBehavior = newBehavior;
            //TODO - implement favoring parse behaviors in J1708Parser and J1939Parser
        }

        #endregion //Communication Settings

        #region WIFI Reader


        private void InitWifiReader()
        {
            wifiReader = new Socket(SocketType.Stream, ProtocolType.Tcp);
            // CHANGEME - finish
        }

        #endregion //WIFI Reader

        #region Port Reader
        /// <summary>
        /// Creates a new event handler and attaches it to the portReader. Calling this method sets a trigger on HandleCommPortData whenever data is received
        /// </summary>
        private void InitPortReader()
        {
            portReader = new SerialPort(portString, 9600, Parity.None, 8, StopBits.One);   
            portReader.DataReceived += new SerialDataReceivedEventHandler(HandleCommPortData);
            portReader.Open(); // Begin communications 
            portCheckTimer = CREATE_TIMER(CheckPort, 5000);
        }

        /// <summary>
        /// Automatically called every time data comes through the USB Port. Splits all messages in the buffer and send them to ProcessData
        /// </summary>
        private void HandleCommPortData(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                //break the buffer into an array of messages and process them individually.
                //each valid, individual message in the buffer ends in a newline character
                try
                {
                    string buffer = portReader.ReadExisting();
                    foreach (string message in buffer.Split('\n'))
                        ProcessData(message);
                }
                catch
                {
                }
            });
        }

        private void ClosePortReader()
        {
            keepJibAwakeTimer.Dispose();
            portCheckTimer.Dispose();
            portReader.Close();
            portReader = null;
            System.Threading.Thread.CurrentThread.Abort();
        }
        #endregion //Port Reader

        #region Log Reader
        /// <summary>
        /// Opens the log file to be read by the program. Sets a timer to call ReadLogEntry() every 100ms
        /// </summary>
        private void InitLogReader()
        {
            logReader = new StreamReader(logPlayerFile);
            logReadTimer = CREATE_TIMER(ReadLogEntry, 100);

        }

        /// <summary>
        /// Called every 100ms when using the log reader. Reads the next line of the log file and passes the message to ProcessData(). 
        /// Returns to the beginning of the file once reaching the end
        /// </summary>
        private void ReadLogEntry(Object source, ElapsedEventArgs e)
        {
            string line = " ";
            while (line != null && line.Length < 2)
                line = logReader.ReadLine();
            if (line != null && (line[0] == 'J' || line[0] == 'R'))
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ProcessData(line);
                });
            else if (line != null)
                ReadLogEntry(null, null);
            else
            {
                logReader.DiscardBufferedData();
                logReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            }
        }

        #endregion //Log Reader

        #region Event Timer Callbacks

        /// <summary>
        /// sends a "V" message to the JIB to keep it in VMS mode
        /// </summary>
        private void KeepJibAwake(Object source, ElapsedEventArgs e)
        {
            portReader.Write("V");
        }

        /// <summary>
        /// Checks whether or not data is being received. If no data is being received, creates a new CommChecker object to automatically find the right port.
        /// Resets the port if it can find the correct one.
        /// </summary>
        private void CheckPort(Object source, ElapsedEventArgs e)
        {
            if (lastMessageCount != messageCount)
            {
                lastMessageCount = messageCount;
                return;
            }

        }
        #endregion //Event Timer Callbacks
    }
}
