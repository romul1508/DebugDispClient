using System;


namespace DebugOmgDispClient.adapters
{
    /// <summary>
    /// Designed to perform tasks with interfaces, convenient for the builder of the server part
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    interface TxRxTargets
    {
        public void Request();
        
    }

    public class concreteTxRxTargets: TxRxTargets
    {
        public virtual void Request()
        {
            Console.WriteLine("Called Target Request()");
        }
    }
}
