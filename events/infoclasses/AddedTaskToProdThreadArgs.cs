using DebugOmgDispClient.tasks.abstr;

namespace DebugOmgDispClient.events.infoclasses
{
    /// <summary>
    /// Sends information related to the event - "New task arrival"
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 26.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class AddedTaskToProdThreadArgs: TaskEventArgs
    {
        private string msgInfo = "isAddedTaskToProdThread = true";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="task">Current tasks for temporary storage</param>
        /// <param name="idProdThread">Worker background thread ID</param>
        public AddedTaskToProdThreadArgs(ATask task, int idProdThread)
        {
            this.task = task;
            this.idProdThread = idProdThread;
        }

        /// <summary>
        /// Message being sent
        /// </summary>
        public override string MsgInfo
        {
            get { return msgInfo; }            
        }    
    }
}
