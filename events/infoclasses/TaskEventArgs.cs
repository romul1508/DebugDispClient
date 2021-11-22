using DebugOmgDispClient.tasks.abstr;

namespace DebugOmgDispClient.events.infoclasses
{
    /// <summary>
    /// Provides information about the corresponding event (base class, contains general properties)
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 21.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    abstract public class TaskEventArgs
    {
        protected int idProdThread = -1;     // Worker background thread ID
        protected ATask task = null;         // the task, within which the necessary set of operations has been completed,
                                             // goes into the status - THE_OPERATING_PART_IS_COMPLETED

        /// <summary>
        /// Message being sent
        /// </summary>        
        public virtual string MsgInfo
        {
            get => "";
        }

        /// <summary>
        /// Worker background thread id
        /// </summary>
        public int IdProdThread
        {
            get { return idProdThread; }
            set { idProdThread = value; }
        }

        /// <summary>
        /// A task that is waiting for subtasks to complete
        /// </summary>
        public ATask TaskToProdThread
        {
            get { return task; }
            set { task = value; }
        }
    }
}
