using DebugOmgDispClient.Interfaces;


namespace DebugOmgDispClient.services.factory
{
    /// <summary>
    /// The class is the creator! An instance of the DispModuleClient class is created
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class CreatorDispModuleQtClient: CreatorDispModuleClient
    {       

        public override AbstractDispModuleClient CreateDispModuleClient()
        {
            return new DispModuleClient();
        }
    }
}
