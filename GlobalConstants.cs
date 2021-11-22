namespace DebugOmgDispClient
{
    /// <summary>
    /// Contains global immutable constants
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 20.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public static class GlobalConstants
    {        
        /// <summary>
        /// First production worker (background) stream
        /// </summary>        
        public const int MAIN_AREAS_WORK = 0;

        /// <summary>
        /// Production workflow (second)
        /// </summary>
        public const int MSG_EXCHANGE_CONN_TXRX_THREAD = 1;

        /// <summary>
        /// Production workflow (third)
        /// </summary>
        public const int MSG_EXCHANGE_CONN_RXTX_THREAD = 2;

        /// <summary>
        /// Fourth production stream
        /// </summary>
        public const int MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX = 3;

        /// <summary>
        /// Fifth production work stream
        /// </summary>
        public const int MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX = 4;

        //---------------------------------------
        /// <summary>
        /// IP:Port Server
        /// </summary>
        public const string IP_SERVER = "://10.1.6.140:5000/";

        public const string DestIP = "10.1.6.140";

        //---------------------------------------
        /// <summary>
        /// Local IP
        /// </summary>
        public const string IP_LOCAL = "10.1.6.160";

        //---------------------------------------
        /// <summary>
        /// IP RTP-Server
        /// </summary>
        public const string IpRTPServer = "10.1.6.140";

        /// <summary>
        /// Port RTP-server
        /// </summary>
        public const int ServerUDPPort = 7468;
        //--------------------------------------
        /// <summary>
        /// Local UDP port
        /// </summary>
        public const int LocalUDPPort = 4367;
        //-------------------------------------
        /// <summary>
        /// Dispatch Console Local IP
        /// </summary>
        public const string IpDispConsole = "127.0.0.1";          // localhost (default),
                                                                  // it is supposed to work on one device
    }
}
