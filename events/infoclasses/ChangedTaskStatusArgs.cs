using DebugOmgDispClient.tasks.abstr;

namespace DebugOmgDispClient.events.infoclasses
{
    /// <summary>
    /// Sends information related to the event - "Task status changed"
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 28.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class ChangedTaskStatusArgs: TaskEventArgs
    {
        private string msgInfo = "Task status changed";


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="task">The current task in which the status has changed</param>
        /// <param name="idProdThread">Worker background thread ID</param>
        public ChangedTaskStatusArgs(ATask task, int idProdThread)
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


