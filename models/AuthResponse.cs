namespace DebugOmgDispClient.models
{
    /// <summary>
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 26.10.2021     
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class AuthResponse
    {        
        /// <summary>
        /// Error
        /// </summary>
        public string Error { get; set; }       

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string Username { get; set; }
    }
}
