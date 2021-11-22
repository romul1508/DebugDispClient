using DebugOmgDispClient.models.decorator.user;
using DebugOmgDispClient.tasks.abstr;
using System.Collections.Generic;

namespace DebugOmgDispClient.models
{
    /// <summary>
    /// Provides work with global objects
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 26.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class GlobalObjects
    {
              
        private static ICollection<HubConnectEntity> hubConnects = new List<HubConnectEntity>();
        //------------------------------------

        public static int destinServerPort = 7654;

        //-----------------------------------        

        public static int localPort = 4567;

        //------------------------------------

        // public static int localUDPPort = 4367;
        //------------------------------------

        public static string connectionId = "";

        public static string token = "";

        public static string currentVoiceSessionName = "";       

        

        public static int numThreads = 5;                                           // number of worker threads

        private static List<ATask> hPriorTasks = new List<ATask>();                 // contains tasks with high priority (executed first)
        private static List<ATask> mPriorTasks = new List<ATask>();                 // contains tasks with medium priority

        private static int[] workingHours = new int[numThreads];                    // contains information about the operating state (operating mode)
                                                                                    // in which the worker threads are located  (NONE || IN_ACTIVE_MODE || IN_STANDBY_STATE)

        private static int[] pipeConnStats = new int[numThreads];                   // stores the statuses of pipe connections

        private static bool[] isRunningThreads = new bool[numThreads];              // contains the state of working background threads (true - started and functioning, false - not used)

        private static string[] pipeChannelNames = new string[numThreads];          // pipe names


        public static string ConnectionId
        {
            get { return connectionId; }

            set { connectionId = value; }
        }


                
        public static ICollection<HubConnectEntity> HubConnects
        {
            get { return hubConnects; }
        }

        /// <summary>
        /// Token (for interacting with the server)
        /// </summary>
        public static string Token
        {
            get { return token; }

            set { token = value; }
        }


        public static string CurrentVoiceSessionName
        {
            get { return currentVoiceSessionName; }

            set { currentVoiceSessionName = value; }
        }


        /// <summary>
        /// List of high priority (operational) tasks
        /// </summary>
        public static List<ATask> HPriorTasks
        {
            get { return hPriorTasks; }
        }

        /// <summary>
        /// List of current tasks
        /// </summary>
        public static List<ATask> MPriorTasks
        {
            get { return mPriorTasks; }
        }

        /// <summary>
        /// The number of working (cyclic) threads
        /// </summary>
        public static int NumThreads
        {
            get { return numThreads; }

            // set { users = value; }
        }

        /// <summary>
        /// returns information about the operating status (operating mode), 
        /// in which the worker threads are located  (NONE || IN_ACTIVE_MODE || IN_STANDBY_STATE)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetWorkingHours(int index)
        {
            return workingHours[index];
        }

        /// <summary>
        /// set information about the working state (operating mode), 
        /// in which the worker threads are located  (NONE || IN_ACTIVE_MODE || IN_STANDBY_STATE)
        /// </summary>
        public static void SetWorkingHours(int index, int value)
        {
            workingHours[index] = value;
        }

        /// <summary>
        /// Returns the name of the workflow pipe
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetPipeChannelName(int index)
        {
            return pipeChannelNames[index];
        }

        /// <summary>
        /// Sets the name of the workflow pipe
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void SetPipeChannelName(int index, string value)
        {
            pipeChannelNames[index] = value;
        }

        /// <summary>
        /// returns the status of a specific pipe connection 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetPipeConnStatus(int index)
        {
            return pipeConnStats[index];
        }

        /// <summary>
        /// sets the status of a specific pipe connection
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void SetPipeConnStatus(int index, int value)
        {
            pipeConnStats[index] = value; ;
        }


        /// <summary>
        /// returns the states of threads (exists / does not exist)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetIsRunningThreads(int index)
        {
            return isRunningThreads[index];
        }

        /// <summary>
        /// sets the states of threads (exists / does not exist)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void SetIsRunningThreads(int index, bool value)
        {
            isRunningThreads[index] = value;
        }

    }
}
