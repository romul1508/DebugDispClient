namespace DebugOmgDispClient.events.infoclasses
{
    /// <summary>
    /// Provides information about the corresponding event (click of the PTT button in the Dispatcher Console)
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class ClickedDispConsolePTTArgs
    {
        private string msgInfo = "isClickedDispConsolePTT changed";

        /// <summary>
        /// button state (true - enabled, false - disabled)
        /// </summary>
        private bool isClickedDispConsolePTT = false;

        public ClickedDispConsolePTTArgs(bool isClickedDispConsolePTT)
        {
            this.isClickedDispConsolePTT = isClickedDispConsolePTT;
        }

        public string MsgInfo
        {
            get { return msgInfo; }            
        }

        public bool IsClickedDispConsolePTT
        {
            get { return isClickedDispConsolePTT; }            
        }
    }
}
