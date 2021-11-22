
namespace DebugOmgDispClient.adapters
{
    /// <summary>
    /// Class - adapter, allows you to use a specific desired interface (which the parties have agreed on) 
    /// interacting with inappropriate (obsolete or otherwise)
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class TxRxAdapter : concreteTxRxTargets
    {
        private TxRxAdaptee adaptee = new TxRxAdaptee();
        public override void Request()
        {
            // Possibly do some other work
            // and then call SpecificRequest
            adaptee.SpecificRequest();
        }
    }
}
