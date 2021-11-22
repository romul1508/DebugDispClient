namespace DebugOmgDispClient.events.infoclasses
{

    /// <summary>
    /// Provides information that the event is due to the user terminating the application, 
    /// all open network connections must be closed,
    /// and also free up other allocated resources 
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 20.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class InstructToTerminateServiceArgs
    {
        private string msgInfo = "isInstructToTerminateService = true";

        /// <summary>
        /// the state of the current proxy server (false - the proxy continues to work, 
        /// true - to be deactivated)
        /// </summary>
        private bool isInstructToTerminateService = false;

        public InstructToTerminateServiceArgs(bool isInstructedToTerminateService)
        {
            this.isInstructToTerminateService = isInstructedToTerminateService;
        }

        public string MsgInfo
        {
            get { return msgInfo; }
            set { msgInfo = value; }
        }

        public bool IsInstructToTerminateService
        {
            get { return isInstructToTerminateService; }
            set { isInstructToTerminateService = value; }
        }
    }
}
