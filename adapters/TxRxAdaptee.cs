using System;


namespace DebugOmgDispClient.adapters
{
    /// <summary>
    /// Adapter class using an incompatible interface
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 23.10.2021    
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class TxRxAdaptee
    {
        public void SpecificRequest()
        {
            Console.WriteLine("Called SpecificRequest()");
        }
    }
}
