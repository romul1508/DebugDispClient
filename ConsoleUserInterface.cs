using System;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.common;
using DebugOmgDispClient.models;
using DebugOmgDispClient.rtp;

namespace DebugOmgDispClient
{
    /// <summary>
    /// Provides the display screen of key current interaction data data with dispatch console
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 20.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class ConsoleUserInterface
    {
        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;

        static object locker = new object();

        private static int statusCurrMonitor = (int)StatusCurrGUI.AT_WORK;
        private static bool isCloseApp = false;

        public static void interact()
        {
            lock (locker)
            {
                
                logger.Write("\n Class: UserInterface; interact method: Console available .");

                while (isCloseApp == false)
                {
                    Console.WriteLine("To view information about the current connection to the server, click <i>");
                    Console.WriteLine("    <a> to view information about current connections with the dispatcher console");
                    Console.WriteLine("    <b> to display a list of active worker threads");
                    Console.WriteLine("    <c> to display a list of available system users");
                    Console.WriteLine("    <d> to audio track data");
                    Console.WriteLine("    <e> to display a list of unsolved problems");                    
                    Console.WriteLine("    <f> to stop the current screen");
                    Console.WriteLine("    <q> to exit");
                    var task = Console.ReadLine();
                    if (task == "i")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <i> .");

                        Console.WriteLine("ID connection = {0} .", GlobalObjects.ConnectionId);

                        Console.WriteLine("Current voice session name:  {0}", GlobalObjects.CurrentVoiceSessionName);

                        Console.WriteLine("Server RTP Port = {0} .", GlobalConstants.ServerUDPPort);

                        Console.WriteLine("Local UDP port = {0} .", GlobalConstants.LocalUDPPort);

                        Console.WriteLine("Ip RTP Server = {0} .", GlobalConstants.IpRTPServer);

                        Console.WriteLine("Token = {0} .", GlobalObjects.Token);

                        Console.WriteLine("Server IP = {0} .", GlobalConstants.IP_SERVER);


                    }
                    else if (task == "a")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <a> .");

                        Console.WriteLine("Number of worker threads = {0} .", GlobalObjects.NumThreads);

                        Console.WriteLine("Local IpAddr  = ([{0}]) .", GlobalConstants.IP_LOCAL);

                        Console.WriteLine("Local Ip dispetcher console = ([{0}]) .", GlobalConstants.IpDispConsole);


                        for (int i = 0; i < GlobalObjects.NumThreads; i++)
                        {
                            Console.WriteLine("Pipe channel name [{0}] = {1} .", i, GlobalObjects.GetPipeChannelName(i));

                            Console.WriteLine("Pipe connection status [{0}] = {1} .", i, GlobalObjects.GetPipeConnStatus(i));

                        }
                    }
                    else if (task == "b")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <b> .");

                        for (int i = 0; i < GlobalObjects.NumThreads; i++)
                        {
                            Console.WriteLine("Workflow state [{0}] = {1} .", i, GlobalObjects.GetIsRunningThreads(i));

                            Console.WriteLine("Working hours [{0}] = {1} .", i, GlobalObjects.GetWorkingHours(i));
                        }
                    }
                    else if (task == "c")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <c> .");

                    }
                    else if (task == "d")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <d> .");

                        Console.WriteLine("Codec name = {0} .", ProvCommunicServer.codecName);

                        Console.WriteLine("Time stamp = {0} .", ProvCommunicServer.timestamp);

                        Console.WriteLine("Sequence number = {0} .", ProvCommunicServer.sequenceNumber);

                        Console.WriteLine("Sample rate = {0} .", ProvCommunicServer.sampleRate);

                        Console.WriteLine("Frame size = {0} .", ProvCommunicServer.frameSize);

                        Console.WriteLine("ssrc = {0} .", ProvCommunicServer.ssrc);

                    }
                    else if (task == "e")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <e> .");
                    }
                    else if (task == "f")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <f> .");

                    }
                    else if (task == "q")
                    {
                        logger.Write("\n Class: UserInterface; interact method: user clicked <q> .");

                        // Next you need to execute a command that checks out open workflows if they exist,
                        // then you need to close these threads.

                        isCloseApp = true;
                    }

                }                
                
            }

            if (isCloseApp)
            {                
                GlobalObjects.SetIsRunningThreads(GlobalConstants.MAIN_AREAS_WORK, false);
                GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, false);
                GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, false);
                GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, false);
                GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, false);                
            }            
        }

        public static int StatusCurrMonitor
        {
            get => statusCurrMonitor;
            set => statusCurrMonitor = value;
        }

        // allows the user interface to terminate
        public static bool IsCloseApp
        {
            get => isCloseApp;
            set => isCloseApp = value;
        }
    }
}
