using DebugOmgDispClient.Interfaces;

namespace DebugOmgDispClient.services.factory
{
    /// <summary>
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 21.09.2021
    /// version: 1.0.1
    /// sinc 28.10.2021
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public abstract class CreatorService
    {
        public abstract ISignalRService CreateService();
    }
}
