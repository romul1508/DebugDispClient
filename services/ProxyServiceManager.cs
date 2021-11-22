using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.events.infoclasses;
using System.Threading;
using System.Threading.Tasks;

namespace DebugOmgDispClient.services
{
    /// <summary>
    /// The class implements the IProxyServiceManager interface,
    /// is the base for the main service manager of the BackendServiceManager program module
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 21.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public abstract class ProxyServiceManager : IProxyServiceManager
    {
        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;

        private bool? _isClickedDispConsolePTT = false;

        /// <summary>
        /// PTT button status (by default, the button is inactive)
        /// </summary>
        protected bool isClickedDispConsolePTT = false;      // for use in derived classes (by default, the button is inactive)

        /// <summary>
        /// Determines the status of the application (if isInstructedToTerminateService == false, 
        /// the application continues to function, otherwise it must be deactivated)
        /// </summary>
        protected bool isInstructedToTerminateService = false;
                             
        //---------------------------------
        protected AbstractDispModuleClient dispModule = null;
        //--------------------------

        public virtual async Task StartProxyService()
        {
            // int threadId = Thread.CurrentThread.ManagedThreadId;

            // string Tag = "ProxyServiceManager class: Constructor";

            // myLogger.Write($"{Tag}; threadId = {threadId}; Started.... .\n");

            // not yet implemented...
        }

        #region Delegates
        public delegate void ClickedDispConsolePTTHandler(object sender, ClickedDispConsolePTTArgs e);

        public delegate void InstructedToTerminateServiceHandler(object sender, InstructToTerminateServiceArgs e);

        
        #endregion

        #region Properties

       
        public bool? _IsClickedDispConsolePTT
        {
            get {  return _isClickedDispConsolePTT;   }
            set {  _isClickedDispConsolePTT = value;  }
        }

        /// <summary>
        /// Status indicator of the PTT button in the GUI of the dispatch console, 
        /// if it changes, the corresponding event is called
        /// </summary>
        public bool IsClickedDispConsolePTT
        {
            get 
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "ProxyServiceManager class: IsClickedDispConsolePTT property: get";

                logger.Write($"{Tag}; threadId = {threadId}; return isClickedDispConsolePTT = { isClickedDispConsolePTT }.\n");

                return isClickedDispConsolePTT; 
            }
            set 
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "ProxyServiceManager class: IsClickedDispConsolePTT property: set";

                logger.Write($"{Tag}; threadId = {threadId}; isClickedDispConsolePTT = {isClickedDispConsolePTT} .\n");

                if (isClickedDispConsolePTT != value)  // если изменился, генерируется событие
                {
                    logger.Write($"{Tag}; threadId = {threadId}; isClickedDispConsolePTT changed!\n");

                    isClickedDispConsolePTT = value;

                    if (DispConsolePTTClicked != null)
                    {
                        logger.Write($"{Tag}; threadId = {threadId}; DispConsolePTTClicked != null, an event is generated!\n");

                        DispConsolePTTClicked(this, new ClickedDispConsolePTTArgs(isClickedDispConsolePTT));
                    }
                    else
                        logger.Write($"{Tag}; threadId = {threadId}; DispConsolePTTClicked = null, Error...\n");
                }               
                 
            }
        }

        /// <summary>
        /// Application status indicator, if true, must be deactivated
        /// </summary>
        public bool IsInstructedToTerminateService
        {
            get 
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "ProxyServiceManager class: IsInstructedToTerminateService property: get";

                logger.Write($"{Tag}; threadId = {threadId}; return isInstructedToTerminateService = { isInstructedToTerminateService }.\n");

                return isInstructedToTerminateService; 
            }
            set
            {                

                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "ProxyServiceManager class: IsInstructedToTerminateService property: set";

                logger.Write($"{Tag}; threadId = {threadId}; isInstructedToTerminateService = {isInstructedToTerminateService} .\n");          

               
                if (isInstructedToTerminateService != value)  // if changed, an event is generated
                {
                    logger.Write($"{Tag}; threadId = {threadId}; isInstructedToTerminateService changed!\n");

                    isInstructedToTerminateService = value;
                    logger.Write($"{Tag}; threadId = {threadId}; isInstructedToTerminateService = {isInstructedToTerminateService} .\n");

                    if (TerminateServiceInstructed != null)
                    {
                        logger.Write($"{Tag}; threadId = {threadId}; TerminateServiceInstructed != null, an event is generated!\n");
                        TerminateServiceInstructed(this, new InstructToTerminateServiceArgs(isInstructedToTerminateService));
                    }
                    else
                        logger.Write($"{Tag}; threadId = {threadId}; TerminateServiceInstructed = null, Error...\n");
                }
            }
        }


        #endregion

        #region Events

        /// <summary>
        /// Occurs after pressing the "PTT" button in the dispatch console
        /// </summary>
        public event ClickedDispConsolePTTHandler DispConsolePTTClicked;

        /// <summary>
        /// Occurs when, in the user interface provided by the UserInterface class, 
        /// the user exits the application by selecting the <q> option 
        /// </summary>
        public event InstructedToTerminateServiceHandler TerminateServiceInstructed;

        #endregion

        #region Called Events
        /// <summary>
        /// Provides triggering of the "Button clicked (PTT)" event
        /// </summary>
        /// <param name="backendProxyServiceManager"></param>
        /// <param name="e"></param>
        public void ClickedDispConsolePTT(object backendProxyServiceManager, ClickedDispConsolePTTArgs e)
        {
            SetClickedDispConsolePTT();
        }

        /// <summary>
        /// Raises the "End Application Command Received" event
        /// </summary>
        /// <param name="backendProxyServiceManager"></param>
        /// <param name="e"></param>
        public void InstructedToTerminateService(object backendProxyServiceManager, InstructToTerminateServiceArgs e)
        {
            SetInstructedToTerminateService();
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Changes the status of the "PTT" button state in the dispatch console, 
        /// raises the corresponding event
        /// </summary>
        public void SetClickedDispConsolePTT()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "ProxyServiceManager class: SetClickedDispConsolePTT method";

            logger.Write($"{Tag}; threadId = {threadId}; state: started...\n");

            
            if (isClickedDispConsolePTT)          // if the PTT button (in the dispatch console) is enabled
            {
                logger.Write($"{Tag}; threadId = {threadId}; PTT activated! deactivation required...\n");

                logger.Write($"{Tag}; threadId = {threadId}; isClickedDispConsolePTT = false .\n");

                isClickedDispConsolePTT = false;     // turn off the PTT

            }
            else
            {
                logger.Write($"{Tag}; threadId = {threadId}; PTT deactivated! activation required...\n");

                logger.Write($"{Tag}; threadId = {threadId}; isClickedDispConsolePTT = true .\n");

                isClickedDispConsolePTT = true;
            }

            if (DispConsolePTTClicked != null)
            {
                logger.Write($"{Tag}; threadId = {threadId}; DispConsolePTTClicked != null, an event is generated!\n");

                DispConsolePTTClicked(this, new ClickedDispConsolePTTArgs(isClickedDispConsolePTT));
            }
            else
                logger.Write($"{Tag}; threadId = {threadId}; DispConsolePTTClicked = null, Error...\n");
        }

        /// <summary>
        /// Changes the status of the application, 
        /// it is in the final stage (in the phase of freeing resources)
        /// </summary>
        public void SetInstructedToTerminateService()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "ProxyServiceManager class: SetInstructedToTerminateService method";

            logger.Write($"{Tag}; threadId = {threadId}; state: started...\n");

            

            if (!isInstructedToTerminateService)
            {
                logger.Write($"{Tag}; threadId = {threadId}; isInstructedToTerminateService = true .\n");

                isInstructedToTerminateService = true;
            }
            else
                logger.Write($"{Tag}; threadId = {threadId}; isInstructedToTerminateService = { isInstructedToTerminateService } .\n");

            if (TerminateServiceInstructed != null)
            {
                logger.Write($"{Tag}; threadId = {threadId}; TerminateServiceInstructed != null, an event is generated!\n");

                TerminateServiceInstructed(this, new InstructToTerminateServiceArgs(isInstructedToTerminateService));
            }
            else
                logger.Write($"{Tag}; threadId = {threadId}; TerminateServiceInstructed = null, Error...\n");
        }
        #endregion
    }
}
