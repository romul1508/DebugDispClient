using DebugOmgDispClient.Tasks;


namespace DebugOmgDispClient.events.infoclasses
{

    /// <summary>
    /// Sends information related to the AfterCompletionOperatingPartHandler 
    /// event (generated after changing the status of the StatusTASK.TASK_AT_WORK task to THE_OPERATING_PART_IS_COMPLETED)
    /// "Completed the operational part of the task"
    ///  author: Roman Ermakov
    ///  e-mail: romul1508@gmail.com
    /// sinc 01.11.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </ >
    public class AfterCompletOperPartArgs: TaskEventArgs
    {
        private string msgInfo = "AfterCompletionOperPart = true";


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="task">Current tasks for temporary storage</param>
        /// <param name="idProdThread">Worker background thread ID</param>
        public AfterCompletOperPartArgs(int numTask, int idTask, int priority, int idScenario, int idProdThread)
        {
            task = new OperTask(numTask, idTask, priority, idScenario);
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
