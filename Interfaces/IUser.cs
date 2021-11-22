
namespace DebugOmgDispClient.Interfaces
{

    /// <summary>
    /// User model
    /// IUser interface - for storing information about the system user 
    /// (for example, it can be a mobile client)
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 26.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public interface IUser {
        
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }


        /// <summary>
        /// SignalR network connection identifier
        /// </summary>
        public string ConnectionId { get; set; }


        /// <summary>
        /// User priority
        /// </summary>
        public int Priority { get; set; }
    }
}
