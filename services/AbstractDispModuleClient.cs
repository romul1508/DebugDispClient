using DebugOmgDispClient.common;
using DebugOmgDispClient.events.infoclasses;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.tasks.abstr;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DebugOmgDispClient.services
{
    /// <summary>
    /// Abstract class, partially implements the IDispModuleQtClient interface, 
    /// contains the most important events in terms of interaction with the dispatch console for receiving / transmitting sound 
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public abstract class AbstractDispModuleClient : IDispModuleClient
    {
        protected List<ATask> hPriorTasksForSecondProductThrd = new List<ATask>();              // contains tasks with high priority (executed first)
        protected List<ATask> mPriorTasksForSecondProductThrd = new List<ATask>();              // contains tasks with medium priority (Tx -> Rx cmd/msg)

        protected List<ATask> hPriorTasksForThirdProductThrd = new List<ATask>();               // high priority task list (executed first)
        protected List<ATask> mPriorTasksForThirdProductThrd = new List<ATask>();               // list of tasks for the third thread (Rx -> Tx cmd/msg)

        protected List<ATask> mPriorTasksForFourthProductThrd = new List<ATask>();              // List of current tasks for the fourth thread (transferring sound to the dispatcher console)
        protected List<ATask> hPriorTasksForFourthProductThrd = new List<ATask>();              // List of current tasks with high priority for the fourth thread (transferring sound to the dispatcher console)

        protected List<ATask> mPriorTasksForFifthProductThrd = new List<ATask>();               // List of current tasks for the fifth thread (sound transmission to the dispatcher console)
        protected List<ATask> hPriorTasksForFifthProductThrd = new List<ATask>();               // List of current tasks with high priority for the fifth thread (transferring sound to the dispatcher console)
        //------------------------
        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;
        //------------------------
        /// <summary>
        /// shows the status of any specific task - "Operational part completed",
        /// if the state is true, the operational part of the task is completed, otherwise - false;
        /// </summary>
        private volatile bool isCompletionOperatingPart = false;
        //------------------------      
        protected volatile int currNumTask = -1;
        protected volatile int currIdTask = -1;
        protected volatile int currPriority = -1;
        protected volatile int currIdScenario = -1;
        protected volatile int currIdProdThread = -1;
        //------------------------
        public virtual void StartThreads()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;            

            string Tag = "AbstractDispModuleClient class: StartThreads method: ";

            logger.Write($"{Tag}: threadId = {threadId}: Started!");
        }
        //------------------------
        #region Events
        /// <summary>
        /// Defines the event - "The operational part of the task is completed",
        /// functional operations within the task are completed, 
        /// it remains to wait for the child tasks (subtasks) to be completed.
        /// this event is very significant in terms of application management
        /// </summary>
        /// <param name="sender">who sent the event</param>
        /// <param name="e">contains all the necessary information related to the event</param>
        public delegate void AfterCompletOperPartHandler(object sender, AfterCompletOperPartArgs e);
        public event AfterCompletOperPartHandler AfterCompletionOperatingPart;
        #endregion


        #region List Tasks
        /// <summary>
        /// List of current tasks for the second thread (exchange cmd and / or msg with the dispatch console)
        /// </summary>
        public List<ATask> MPriorTasksForSecondProductionThread
        {
            get { return mPriorTasksForSecondProductThrd; }
        }

        /// <summary>
        /// List of high priority tasks for the second thread (exchange cmd and / or msg with the dispatch console)
        /// </summary>
        public List<ATask> HPriorTasksForSecondProductThread
        {
            get { return hPriorTasksForSecondProductThrd; }
        }

        /// <summary>
        ///  Task list for the third thread of the production thread (Rx -> Tx cmd / msg)
        /// </summary>
        public List<ATask> MPriorTasksForThirdProductThrd
        {
            get { return mPriorTasksForThirdProductThrd; }         // list of current tasks for the third thread 
        }

        /// <summary>
        ///  List of tasks for the third stream of the production stream (Rx -> Tx cmd/msg)
        /// </summary>
        public List<ATask> HPriorTasksForThirdProductThrd
        {
            get { return hPriorTasksForThirdProductThrd; }         // list of current tasks with high priority for the third thread 
        }

        /// <summary>
        /// List of current tasks for the fourth thread (transferring sound to the dispatcher console)
        /// </summary>
        public List<ATask> MPriorTasksForFourthProductThrd
        {
            get { return mPriorTasksForFourthProductThrd; }
        }

        /// <summary>
        /// List of current tasks with high priority for the fourth thread (transferring sound to the dispatcher console)
        /// </summary>
        public List<ATask> HPriorTasksForFourthProductThrd
        {
            get { return hPriorTasksForFourthProductThrd; }
        }

        /// <summary>
        /// List of current tasks for the fifth thread (transferring sound from the dispatch console)
        /// </summary>
        public List<ATask> MPriorTasksForFifthProductThread
        {
            get { return mPriorTasksForFifthProductThrd; }
        }

        /// <summary>
        /// List of current tasks with high priority for the fifth thread (audio transmission from the dispatch console)
        /// </summary>
        public List<ATask> HPriorTasksForFifthProductThrd
        {
            get { return hPriorTasksForFifthProductThrd; }
        }
        #endregion

        public virtual int CloseThreads()
        {
            throw new NotImplementedException();
        }


        #region Event generators

        // AfterCompletionOperatingPartHandler
        /// <summary>
        /// allows you to explicitly generate an event - "Completed the operational part of the task"  
        /// </summary>
        /// <param name="sender">event dispatching object</param>
        /// <param name="task">Parameter problem</param>
        /// <param name="toIdThread">id of the workflow to which the current task is assigned</param>
        public void AfterCompletedOperatingPart(object sender, int numTask, int idTask, int priority, int idScenario, int toIdThread)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Class AbstractDispModuleClient: method AfterCompletedOperatingPart: ";

            logger.Write($"{Tag}: threadId = {threadId}: Event AfterCompletedOperatingPart started!");

                    
            AfterCompletionOperatingPart(sender, new AfterCompletOperPartArgs(numTask, idTask, priority, idScenario, toIdThread));
        }

        #endregion

        #region Properties

        /// <summary>
        /// shows the status of any specific task - "Operational part completed",
        /// if the state is true, the operational part of the task is completed, otherwise - false;
        /// </summary>
        public bool IsCompletionOperatingPart
        {
            get { return isCompletionOperatingPart; }
            set
            {
                
                if (isCompletionOperatingPart != value)  // if changed, an event is generated
                {
                    isCompletionOperatingPart = value;

                    if (isCompletionOperatingPart == true)
                    {
                        AfterCompletionOperatingPart(this, new AfterCompletOperPartArgs(currNumTask, currIdTask, currPriority, currIdScenario, currIdProdThread));
                    }
                }

            }
        }

        #endregion
        //----------------------------------------------

        #region Event handlers
        /// <summary>
        /// Event Handler - Add a task to the repository
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTaskToProdThreadAdded(object sender, AddedTaskToProdThreadArgs e)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Class AbstractDispModuleClient: method OnTaskToProdThreadAdded: ";

            logger.Write($"{Tag}: threadId = {threadId}: Event <OnTaskToProdThreadAdded> started!");
                        
            try
            {
                logger.Write($"{Tag}: threadId = {threadId}:    MsgInfo =         {e.MsgInfo}\n");
                logger.Write($"{Tag}: threadId = {threadId}:    IdProdThread =    {e.IdProdThread}\n");
                logger.Write($"{Tag}: threadId = {threadId}:    IdTask =          {e.TaskToProdThread.IdTask}\n");
                
                switch (e.IdProdThread)
                {
                    case GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD:
                        logger.Write($"{Tag}: threadId = {threadId}:  IdProdThread = MSG_EXCHANGE_CONN_TXRX_THREAD \n");
                        if (e.TaskToProdThread.Priority == (int)PriorityTask.MIDDLE)
                        {
                            MPriorTasksForSecondProductionThread.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  The current task has been added to the repository .\n");
                        }
                        else if (e.TaskToProdThread.Priority == (int)PriorityTask.HIGH)
                        {
                            HPriorTasksForSecondProductThread.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  High priority task added to repository .\n");
                        }
                        break;
                    case GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD:
                        logger.Write($"{Tag}: threadId = {threadId}:  IdProdThread = EXCHANGE_MSG_TO_ESTABL_CONN_RXTX_THREAD \n");
                        if (e.TaskToProdThread.Priority == (int)PriorityTask.MIDDLE)
                        {
                            MPriorTasksForThirdProductThrd.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  The current task has been added to the repository .\n");
                        }
                        else if (e.TaskToProdThread.Priority == (int)PriorityTask.HIGH)
                        {
                            HPriorTasksForThirdProductThrd.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  High priority task added to repository .\n");
                        }
                        break;
                    case GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX:
                        logger.Write($"{Tag}: threadId = {threadId}:  IdProdThread = SEND_VOICE_MSG_TO_DISP_CONSOLE_TXRX \n");
                        if (e.TaskToProdThread.Priority == (int)PriorityTask.MIDDLE)
                        {
                            MPriorTasksForFourthProductThrd.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  The current task has been added to the repository .\n");
                        }
                        else if (e.TaskToProdThread.Priority == (int)PriorityTask.HIGH)
                        {
                            HPriorTasksForFourthProductThrd.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  High priority task added to repository .\n");
                        }
                        break;
                    case GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX:
                        logger.Write($"{Tag}: threadId = {threadId}:  IdProdThread = SEND_VOICE_MSG_FROM_DISP_CONSOLE_RXTX \n");
                        if (e.TaskToProdThread.Priority == (int)PriorityTask.MIDDLE)
                        {
                            MPriorTasksForFourthProductThrd.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  The current task has been added to the repository .\n");
                        }
                        else if (e.TaskToProdThread.Priority == (int)PriorityTask.HIGH)
                        {
                            HPriorTasksForFourthProductThrd.Add(e.TaskToProdThread);
                            logger.Write($"{Tag}: threadId = {threadId}:  High priority task added to repository .\n");
                        }
                        break;
                    default:
                        logger.Write($"{Tag}: threadId = {threadId}: Error: no workflow number set");
                        break;
                }
            }
            catch (System.Exception ex)
            {
                logger.Write($"{Tag}: threadId = {threadId}:  Error: {ex.ToString()}\n");
            }
        }


        /// <summary>
        /// Event handler - "Task status changed"
        /// </summary>
        /// <param name="sender">instance dispatching the event</param>
        /// <param name="e">contains information related to the event</param>
        public void OnTaskStatusChanged(object sender, ChangedTaskStatusArgs e)
        {
            logger.Write("Class AbstractDispModuleClient:  method OnTaskStatusChanged...");

            logger.Write($"Class AbstractDispModuleClient:  method OnTaskStatusChanged: MsgInfo =         {e.MsgInfo}\n");
            logger.Write($"Class AbstractDispModuleClient:  method OnTaskStatusChanged: IdProdThread =    {e.IdProdThread}\n");
            logger.Write($"Class AbstractDispModuleClient:  method OnTaskStatusChanged: IdTask =          {e.TaskToProdThread.IdTask}\n");

            // finds its handler (events) by task ID, thread ID and script ID
            // can be very useful for monitoring task states and performing necessary operations

            // Also, the event allows you to track the end of the task and transfer the corresponding information to the parent task
            // All changeable statuses of copies of task objects (parent and child tasks) must be updated in the storage (in task lists)

            // to realize...
        }

        #endregion
    }
}
