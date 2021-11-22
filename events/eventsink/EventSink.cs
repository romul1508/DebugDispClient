using DebugOmgDispClient.events.infoclasses;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.models;
using DebugOmgDispClient.rtp;
using System;
using System.Threading;
using DebugOmgDispClient.common;

namespace DebugOmgDispClient.events.eventsink
{
    /// <summary>
    /// Event handler class, created to offload source code in key application classes
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 22.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>

    public class EventSink
    {
        object subscriber = null;       // class - subscriber and handler,  

        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriber"> required for objects of classes of subscribers and handlers at the same time </param>
        public EventSink(object subscriber)
        {
            this.subscriber = subscriber;
        }

        /// <summary>
        /// Event handler for pressing the PTT button in the dispatch console GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDispConsolePTTClicked(object sender, ClickedDispConsolePTTArgs e)
        {
            // an event is dispatched from the BackendProxyServiceManager class
            // the BackendProxyServiceManager class is informed about the event occurrence from the DispModuleClient class
            // the DispModuleClient class learns about the event from the first background thread,
            // which directly conducts a pipe exchange with the dispatch console
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "EventSink class: OnDispConsolePTTClicked method: ";

            logger.Write($"Class EventSink:  method OnDispConsolePTTClicked: threadId = {threadId}:  started...\n");

            try
            {
                BackendServiceManager proxyServiceManager = (BackendServiceManager)sender;
                if(e.IsClickedDispConsolePTT)   // if PTT is on
                {
                    logger.Write($"Class EventSink:  method OnDispConsolePTTClicked: threadId = {threadId}:  started voice...\n");

                    proxyServiceManager.SignalRService.invoke("StartVoicePrivate", GlobalObjects.ConnectionId);
                }
                else 
                {
                    // PTT off
                    logger.Write("Class EventSink:  method OnDispConsolePTTClicked: threadId = {threadId}: stopped voice...\n");

                    proxyServiceManager.SignalRService.invoke("StopVoicePrivate", GlobalObjects.ConnectionId );

                    ProvCommunicServer.voiceReleased();
                    ProvCommunicServer.stopAudioProxy();
                }
            }
            catch (Exception ex)
            {
                logger.Write($"Class EventSink:  method OnDispConsolePTTClicked: threadId = {threadId}: Error: Exception ex = {ex.ToString()}\n");
            }
        }

        /// <summary>
        /// Event handler - user selects option "q" (terminate application)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTerminateServiceInstructed(object sender, InstructToTerminateServiceArgs e)
        {
            // an event is dispatched from the BackendProxyServiceManager class
            // the BackendProxyServiceManager class is informed about the occurrence of an event from the UserInterface class (method Interact ())
            // in the console menu, the user selected the option - "q"(Quit Application)

            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "EventSink class: OnTerminateServiceInstructed method: ";

            logger.Write($"Class EventSink:  method OnTerminateServiceInstructed: threadId = {threadId}:  started...\n");

            try
            {
                BackendServiceManager proxyServiceManager = (BackendServiceManager)sender;
                
                logger.Write($"Class EventSink:  method OnTerminateServiceInstructed: threadId = {threadId}:  OnTerminateServiceInstructed...\n");
                logger.Write($"Class EventSink:  method OnTerminateServiceInstructed: threadId = {threadId}: {e.MsgInfo}\n");


                proxyServiceManager.SignalRService.disconnect();
                proxyServiceManager.Destroy();
                
            }
            catch (Exception ex)
            {
                logger.Write($"Class EventSink:  method OnTerminateServiceInstructed: threadId = {threadId}:  Error: {ex.ToString()}\n");
            }
        }

        /// <summary>
        /// Event handler - "After the execution of the operational part of the task"
        /// </summary>
        /// <param name="e"></param>
        public void OnAfterCompletOperPart(object sender, AfterCompletOperPartArgs e)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            string Tag = "Class EventSink: method OnAfterCompletionOperatingPart: ";
            logger.Write($"Class EventSink:  method OnAfterCompletionOperatingPart: threadId = {threadId}:  started...\n");

            try
            {
                BackendServiceManager proxyServiceManager = (BackendServiceManager)subscriber;

                logger.Write($"Class EventSink: method OnAfterCompletionOperatingPart: threadId = {threadId}: proxyServiceManager created!");
                logger.Write($"Class EventSink:  method OnAfterCompletionOperatingPart: threadId = {threadId}: Event:  {e.MsgInfo}\n");
                logger.Write($"Class EventSink:  method OnAfterCompletionOperatingPart: threadId = {threadId}: IdProdThread = {e.IdProdThread} .\n");

                // e.IdProdThread

                switch (e.IdProdThread)
                {
                    case GlobalConstants.MAIN_AREAS_WORK:

                        logger.Write($" {Tag}: threadId = {threadId}: IdProdThread = MAIN_AREAS_WORK\n");

                        if (e.TaskToProdThread.IdScenario ==(int)Scenario.START_AUDIO_PROXY_TX_RX)
                        {
                            logger.Write($" {Tag}: threadId = {threadId}: IdScenario = START_AUDIO_PROXY_TX_RX\n");


                            if (e.TaskToProdThread.IdTask == (int)CmdTYPE.OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS)
                            {
                                logger.Write($" {Tag}: threadId = {threadId}: IdTask = SUBSCRIBER_REQUESTED_TO_OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS\n");
                                // in the array of tasks of the main thread there is a task, the status changes

                                // if there are no child tasks, the status changes to - Task completed, and the parent task is notified accordingly


                                // then the handler instructions are executed...

                            }
                        }
                        break;
                    case GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD:

                        logger.Write($" {Tag}: threadId = {threadId}: IdProdThread = MSG_EXCHANGE_CONN_TXRX_THREAD\n");
                        
                        if (e.TaskToProdThread.IdScenario == (int)Scenario.START_AUDIO_PROXY_TX_RX)
                        {
                            logger.Write($" {Tag}: threadId = {threadId}: IdScenario = START_AUDIO_PROXY_TX_RX\n");

                            if (e.TaskToProdThread.IdTask == (int)CmdTYPE.AGREE_WITH_DISP_CONSOLE_TO_ESTABL_TXRX_VOICE_CHANNEL)
                            {
                                logger.Write($" {Tag}: IdTask = {threadId}: IdTask = AGREE_WITH_DISPATCH_CONSOLE_TO_ESTABL_TXRX_VOICE_CHANNEL\n");                                
                                
                                // proxyServiceManager.IsClickedDispConsolePTT = true;
                            }
                        }
                        break;
                    case GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD:
                        logger.Write($" {Tag}: threadId = {threadId}: IdProdThread = MSG_EXCHANGE_CONN_RXTX_THREAD\n");
                                      
                        break;
                    case GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX:
                        logger.Write($" {Tag}: threadId = {threadId}: IdProdThread = MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX\n");

                        break;
                    case GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX:
                        logger.Write($" {Tag}: threadId = {threadId}: IdProdThread = MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX\n");

                        break;
                    default:
                        logger.Write($"Class BackendProxyServiceManager:  method OnTaskToSecondProdThreadAdded: threadId = {threadId}, Error: no workflow number set");
                        break;
                }
            }
            catch(Exception ex)
            {
                logger.Write($"Class BackendProxyServiceManager:  method OnTaskToSecondProdThreadAdded: threadId = {threadId}, Error: Exception ex = { ex.ToString() } ");
            }
        }

        #region Properties

        /// <summary>
        /// a reference to an object of the class of the event subscriber and the handler at the same time
        /// this allows you to offload the business logic in the key classes of the application
        /// </summary>
        public object Subscriber
        {
            get { return subscriber; }

            set { subscriber = value; }
        }
        
        #endregion
    }
}
