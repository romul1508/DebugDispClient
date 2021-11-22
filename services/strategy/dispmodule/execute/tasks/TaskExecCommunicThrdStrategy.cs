using DebugOmgDispClient.events.infoclasses;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.tasks.abstr;
using System.IO.Pipes;
using System.Threading;


namespace DebugOmgDispClient.services.strategy.dispmodule.execute.tasks
{
    /// <summary>
    /// Abstract class for performing tasks related to information pipe-exchange,
    /// implements the interface
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 27.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public abstract class TaskExecCommunicThrdStrategy : ITaskExecutionStrategy
    {
        protected NamedPipeServerStream pipeServer = null;
        //-----------------
        protected readonly object sync = new object();     // для lock
        //-----------------
        public delegate void AddedTaskToProdThreadHandler(object sender, AddedTaskToProdThreadArgs e);
        public event AddedTaskToProdThreadHandler TaskToProdThreadAdded;
        //-----------------
        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;

        public virtual int TaskExecute(ITask task)
        {            
            return 1;
        }

        /// <summary>
        /// Adds a task to the List [ITask] of the worker background thread 
        /// </summary>
        /// <param name="sender">event dispatching object</param>
        /// <param name="task">Parameter problem</param>
        /// <param name="toIdThread">id of the workflow to which the current task is assigned</param>
        // protected void AddTaskToProductionThread(object sender, ITask task, int toIdThread)
        protected void AddTaskToProductThread(object sender, ATask task, int toIdThread)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "TaskExecCommunicThreadStrategy class, AddTaskToProductThread method: ";            

            //--------------------------

            logger.Write($"{Tag}; threadId = {threadId}; state: Event TaskToProdThreadAdded Started...\n");

            TaskToProdThreadAdded(sender, new AddedTaskToProdThreadArgs(task, toIdThread));
        }        
    }
}
