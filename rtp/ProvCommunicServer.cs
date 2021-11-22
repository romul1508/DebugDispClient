using DebugOmgDispClient.common;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.models;
using DebugOmgDispClient.models.decorator.user;
using DebugOmgDispClient.services;
using DebugOmgDispClient.tasks.abstr;
using DebugOmgDispClient.tasks.parameters;
using DebugOmgDispClient.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DebugOmgDispClient.rtp
{
    /// <summary>
    /// Provides communication with the server
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov    
    /// </summary>
    public class ProvCommunicServer
    {
        public static long timestamp = 0;        

        public static int sequenceNumber = 0;

        public static int sampleRate = 16000;

        public static int frameSize = 60;

        public static int ssrc = 0;

        public static string codecName = "Opus";

        public static bool rx = false;
        public static bool tx = false;


        public static volatile bool isRunTX = false;
        public static volatile bool isRunRX = false;  

        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;
        
        private static readonly object sync = new object();     // for lock

        private static string Tag = "ProvCommunicServer";

        /// <summary>
        /// Initiates a voice call tx-> rx (checking the readiness of the dispatch console to accept a call and more)
        /// </summary>
        public static void startAudioProxy()
        {
            // creates a task for the main background thread (main)
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "ProvCommunicServerP class: startAudioProxy method: ";

            logger.Write($"{Tag}; state: Started...\n");
            
            try
            {
                lock (sync)
                {
                    logger.Write($"{Tag}; threadId = {threadId}; isRunRX = {isRunRX}...\n");
                    
                    if (isRunRX == false )
                    {

                        // int threadId = Thread.CurrentThread.ManagedThreadId;

                        
                        int id_user = 9;                                // user ID, must come from the server

                        string user_name = "Ivanov";

                        string id_conn = "15";                          // network connection identifier

                        int user_priority = (int)PriorityTask.MIDDLE;   // user priority
                                                                        //string ipAddrPort = "10.1.6.87:5000";           // User ip address
                                                                        //string user_token = "";                         // user token (for interacting with the server)

                        logger.Write($"{Tag}; threadId = {threadId} ; User data setting...\n");

                        List<IParamTask> taskParams = new List<IParamTask>();

                        if (GlobalObjects.HubConnects.Count > 0)
                        {
                            // logger.Write($"{Tag}; threadId = {threadId} ; User data setting...\n");

                            var ufd = GlobalObjects.HubConnects.ElementAt(0);

                            IParamTask idUserParamTask = new ParamTask((int)IdUserProperty.ID_USER, "id_user", (int)ParamTypeID.INT, ufd.Id, "User ID");
                            taskParams.Add(idUserParamTask);

                            //IParamTask userNameParamTask = new ParamTask((int)IdUserProperty.USER_NAME, "user_name", (int)ParamTypeID.STRING, ufd.UserName, "User name");
                            //taskParams.Add(userNameParamTask);

                            IParamTask idConnParamTask = new ParamTask((int)IdUserProperty.CONN_ID, "id_conn", (int)ParamTypeID.STRING, ufd.ConnectId, "Network connection identifier");
                            taskParams.Add(idConnParamTask);

                            IParamTask userPriority = new ParamTask((int)IdUserProperty.USER_PRIORITY, "user_priority", (int)ParamTypeID.INT, (int)PriorityUser.MIDDLE, "User priority");
                            taskParams.Add(userPriority);
                        }

                        else
                        {
                            IParamTask idUserParamTask = new ParamTask((int)IdUserProperty.ID_USER, "id_user", (int)ParamTypeID.INT, id_user, "User ID");
                            taskParams.Add(idUserParamTask);

                            // IParamTask userNameParamTask = new ParamTask((int)IdUserProperty.USER_NAME, "user_name", (int)ParamTypeID.STRING, user_name, "User name");
                            // taskParams.Add(userNameParamTask);

                            IParamTask idConnParamTask = new ParamTask((int)IdUserProperty.CONN_ID, "id_conn", (int)ParamTypeID.STRING, id_conn, "Network connection identifier");
                            taskParams.Add(idConnParamTask);

                            IParamTask userPriority = new ParamTask((int)IdUserProperty.USER_PRIORITY, "user_priority", (int)ParamTypeID.INT, user_priority, "User priority");
                            taskParams.Add(userPriority);
                            
                        }

                        isRunRX = true;

                        logger.Write($"{Tag}; threadId = {threadId} ; Task parameters created! \n");

                        // OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS = 1
                        string taskDiscription = "Subscriber requested to open voice msg channel, confirm readiness!";
                        
                        ATask task1 = new OperTask(GlobalObjects.MPriorTasks.Count, (int)CmdTYPE.OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS,
                            (int)PriorityTask.MIDDLE, taskDiscription, (int)Scenario.START_AUDIO_PROXY_TX_RX, taskParams, null);

                        logger.Write($"{Tag}; threadId = {threadId} ; Task successfully created! \n");

                        GlobalObjects.MPriorTasks.Add(task1);

                        logger.Write($"{Tag}; threadId = {threadId} ; Task added to repository! \n");

                        isRunRX = true;
                    }
                    
                    if(isRunTX == false )
                    {

                        isRunTX = true;
                    }
                }
            }
            catch(Exception e)
            {
                logger.Write($"{Tag}: threadId = {threadId} ; Exception = {e.ToString()}...\n");
            }
        }

        /// <summary>
        /// Stops audio transmission and playback
        /// </summary>
        public static void stopAudioProxy()
        {
            string Tag = "ProvCommunicServer class: stopAudioProxy method: ";

            int threadId = Thread.CurrentThread.ManagedThreadId;

            logger.Write($"Class: ProvCommunicServer; method: stopAudioProxy(); threadId = {threadId}; state: Started...\n");
            
        }

        /// <summary>
        /// Implements work with sound in the tx-rx direction
        /// </summary>
        public static void voiceReleased()
        {
            //string Tag = "ProvCommunicServer class: voiceReleased method: ";

            // int threadId = Thread.CurrentThread.ManagedThreadId;

            //logger.Write($"Class: ProvCommunicServer; method: voiceReleased(); threadId = {threadId}; state: Started...\n");

            if (rx) 
            {
                // logger.Write($"Class: ProvCommunicServer; method: voiceReleased(); threadId = {threadId}; rx = true .\n");

                // track.stop(); 
            }

            if (tx) 
            {
                // logger.Write($"Class: ProvCommunicServer; method: voiceReleased(); threadId = {threadId}; tx = true .\n");

                // record.stop(); 
            }

            tx = false;
            rx = false;
            sequenceNumber = 0;
            timestamp = 0;

            // logger.Write($"Class: ProvCommunicServer; method: voiceReleased(); threadId = {threadId}; end method: tx = false, rx = false, sequenceNumber = 0, timestamp = 0  .\n");
        }
    }
}
