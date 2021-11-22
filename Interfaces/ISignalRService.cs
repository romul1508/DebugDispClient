using System.Threading.Tasks;

namespace DebugOmgDispClient.Interfaces
{
    /// <summary>
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 20.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public interface ISignalRService
    {
        /// <summary>
        /// Initializes the SignalR service
        /// </summary>
        /// <returns></returns>
        public Task SignalRServiceInit();

        /// <summary>
        /// Sets the connection to the SignalR server
        /// </summary>
        /// <returns></returns>
        public Task<bool> ConnectAsync();

        /// <summary>
        /// Causes remote methods on the server
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="userConnID"></param>
        /// <returns></returns>        

        public Task invoke(string methodName, string userConnID);

        /// <summary>
        /// This method is called on the server
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public Task define(string methodName);

        /// <summary>
        /// Closes server connection
        /// </summary>
        /// <returns></returns>
        public Task disconnect();
    }
}
