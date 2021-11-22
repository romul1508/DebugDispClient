using System.Threading.Tasks;

namespace DebugOmgDispClient
{
    /// <summary>
    /// The main class of the program module
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 20.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {    
            BackendServiceManager backendServiceManager = new BackendServiceManager();
            await backendServiceManager.StartProxyService();           
        }
    }
}
