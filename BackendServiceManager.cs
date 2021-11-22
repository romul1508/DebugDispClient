using DebugOmgDispClient.services;
using DebugOmgDispClient.events.eventsink;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.services.factory;
using System.Threading;
using System.Threading.Tasks;

namespace DebugOmgDispClient
{
    /// <summary>
    /// BackendServiceManager class - Backend Proxy Service Manager
    /// called directly from Program.Main ()
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 20.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved
    /// </summary>
    public class BackendServiceManager : ProxyServiceManager
    {
        private ISignalRService signalRService = null;

        /// <summary>
        /// BackendServiceManager class constructor
        /// </summary>
        public BackendServiceManager()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            logger = SimpleMultithreadSingLogger.Instance;

            string Tag = "BackendServiceManager class: Constructor: ";

            logger.Write($"\n {Tag}; threadId = {threadId}; state: Started...\n");

            //---------------------------           
            // create the necessary services
            CreatorService creatorService = new CreatorSignalRService();        
            
            signalRService = creatorService.CreateService();                   

            //------------------
            
            CreatorDispModuleClient creatorDispModuleClient = new CreatorDispModuleQtClient();
            dispModule = creatorDispModuleClient.CreateDispModuleClient();
            //-----------------
            // starts listening to the event - clicking the PTT button in the Dispatcher Console            
            EventSink sink = new EventSink(this);
            DispConsolePTTClicked += new ClickedDispConsolePTTHandler(sink.OnDispConsolePTTClicked);

            //--------------
            // turn on the classes for handling the event - "Application deactivation"
            TerminateServiceInstructed += new InstructedToTerminateServiceHandler(sink.OnTerminateServiceInstructed);
            
            dispModule.AfterCompletionOperatingPart += new AbstractDispModuleClient.AfterCompletOperPartHandler(sink.OnAfterCompletOperPart);
        }

        /// <summary>
        /// provides the launch of services to interact with SignalR and the dispatch console
        /// </summary>
        /// <returns></returns>
        public override async Task StartProxyService()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            try
            {
                string Tag = "BackendServiceManager class: StartProxyService method: ";

                logger.Write($"{Tag}; threadId = {threadId}; state: Started...\n");

                // start the service to interact with SignalR and the Dispatcher Console
                // for myself: rewrite the creation of this method using a factory builder class, different service implementations are possible (implement ...) 

                // initialize the artifacts necessary for communication and work with the server (connection, logger and event)
                await signalRService.SignalRServiceInit();     
                logger.Write($"\n Class: BackendServiceManager; StartProxyService method: threadId = {threadId}; signalRServiceInit completed");     

                defineSignalr();

                // connect to the server
                await signalRService.ConnectAsync();

                logger.Write($"{Tag}; threadId = {threadId}; state: Connection to the server has occurred!\n");                
                
                logger.Write($"\n Class: BackendServiceManager; StartProxyService method, threadId = {threadId} : signalR based proxy service started!!");

                // start worker background threads
                dispModule.StartThreads();
                
            }
            catch (System.Exception e)
            {
                logger.Write($"\n BackendServiceManager: StartProxyService metod: threadId = {threadId}: Error in the App. The App will be stopped.");
                IsInstructedToTerminateService = true;
                logger.Write($"\n Class: BackendServiceManager; StartProxyService method, threadId = {threadId} : IsInstructedToTerminateService = true");
            }
            finally
            {
                // IsInstructedToTerminateService = true;
            }
        }

        #region Properties

        public ISignalRService SignalRService
        {
            get { return signalRService; }
        }

        #endregion

        /// <summary>
        /// Shuts down the application
        /// </summary>
        public void Destroy()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "BackendServiceManager class: Destroy method: ";

            logger.Write($"{Tag}; threadId = {threadId} state: Started...\n");

            if (dispModule != null)
            {
                dispModule.CloseThreads();
                logger.Write($"{Tag}; threadId = {threadId} state: Application worker threads closed .\n");
            }

            logger.Write($"{Tag}; threadId = {threadId} state: UserInterface.IsCloseApp = true .\n");
            ConsoleUserInterface.IsCloseApp = true;
        }

        /// <summary>
        /// Client Methods Called on the Server 
        /// </summary>
        private async void defineSignalr()
        {
            #region snippet_ConnectionOn
            _ = signalRService.define("EndAudio");
            _ = signalRService.define("StartAudioReceive");
            _ = signalRService.define("StartAudioStream");
            _ = signalRService.define("UpdateUsers");                       
            #endregion
        }

    }
}
