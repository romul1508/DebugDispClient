using DebugOmgDispClient.Interfaces;


namespace DebugOmgDispClient.services.factory
{
    /// <summary>
    /// Creator class, an instance of the SignalRService class is created
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 28.10.2021
    /// version: 1.0.1 
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class CreatorSignalRService : CreatorService
    {
        
        public override ISignalRService CreateService()
        {
            
            return new SignalRService();
        }
    }
}
