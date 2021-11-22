using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.models;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using DebugOmgDispClient.common;
using DebugOmgDispClient.tasks.abstr;
using DebugOmgDispClient.services.strategy.dispmodule.execute.tasks;

namespace DebugOmgDispClient.services
{
    /// <summary>
    /// Key class for interacting with Qt client
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 27.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>    
    public class DispModuleClient: AbstractDispModuleClient
    {

        private static int numThreads
            = GlobalObjects.NumThreads;    // Five threads are used to interact with the dispatch console
                                           // the first stream - to select the main directions of information exchange (assumes the use of global application data)! Works at the application level.
                                           // second thread for exchanging messages (commands and command results) to establish a connection, tx-rx (Android-> AstraLinux) 
                                           // the third thread for exchanging messages (commands and command results) to establish a connection, rx-tx (AstraLinux-Android) 
                                           // the fourth stream is required to transmit sound over the tx-rx path (the server sends, the Qt client plays)   
                                           // the fifth stream is needed to transmit sound over the rx-tx path (the Qt client sends, the Android client plays) 


        private Thread[] servers = new Thread[numThreads];       // worker background threads


        //-----------------------------
        // Synchronization Objects for Workflows
        private readonly object sync1 = new object();     // for lock of the first thread (Constants.MAIN_AREAS_WORK)
        private readonly object sync2 = new object();     // for the lock of the second thread (Constants.MSG_EXCHANGE_CONN_TXRX_THREAD)
        private readonly object sync3 = new object();     // for the "lock" of the third thread (Constants.EXCHANGE_MSG_TO_ESTABL_CONN_RXTX_THREAD)
        private readonly object sync4 = new object();     // for the fourth stream (Constants.SEND_VOICE_MSG_TO_DISP_CONSOLE_TXRX)
        private readonly object sync5 = new object();     // for lock of the fifth thread (Constants.SEND_VOICE_MSG_FROM_DISP_CONSOLE_RXTX)
        //-----------------------------
        private readonly object sync = new object();
        //-----------------------------
        
        private readonly string TAG = "DispModuleClient";
        //-----------------------------
        
        public static SimpleMultithreadSingLogger logger;

        public DispModuleClient()
        {
            logger = SimpleMultithreadSingLogger.Instance;

            logger.Write("\n Class: DispModuleClient: start constructor .");

            lock (sync)
            {

                // Initializing worker threads
                Init();
                logger.Write("\n Class: DispModuleClient; Constructor: Workflow initialization done .");

            }
        }


        /// <summary>
        /// Initializes worker threads 
        /// </summary>
        private void Init()
        {
            logger.Write("\n Class: DispModuleClient, Init method; Workflows are being initialized...");

            GlobalObjects.SetPipeConnStatus(GlobalConstants.MAIN_AREAS_WORK, (int)PipeConnStatus.IS_BEING_ESTABLISHED);
            GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)PipeConnStatus.IS_BEING_ESTABLISHED);
            GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)PipeConnStatus.IS_BEING_ESTABLISHED);
            GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)PipeConnStatus.IS_BEING_ESTABLISHED);
            GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)PipeConnStatus.IS_BEING_ESTABLISHED);

            servers[GlobalConstants.MAIN_AREAS_WORK] = new Thread(mainLinesCommunicThread);
            servers[GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD] = new Thread(exchangeMsgToEstablishConnTxRxThread);
            servers[GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD] = new Thread(exchangeMsgToEstablishConnRxTxThread);
            servers[GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX] = new Thread(soundTransmissViaTxRxPathThread);
            servers[GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX] = new Thread(soundTransmissViaRxTxPathThread);

            // set the statuses of the operating mode           
            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_STANDBY_STATE);
            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_STANDBY_STATE);
            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);

            // sets statuses of further suitability of worker threads (true - useful,
            // should be used, false - useless, should be deactivated)
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MAIN_AREAS_WORK, true);                
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, true);            
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, true);            
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, true);            
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, true);

            logger.Write("\n Class: DispModuleClient, Init method; Workflow initialization done!");
        }        
        
        public override void StartThreads()
        {
            logger.Write("\n Class: DispModuleClient: StartThreads method;  worker threads started!");

            // Запустим их
            servers[GlobalConstants.MAIN_AREAS_WORK].Start();
            servers[GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD].Start();
            servers[GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD].Start();
            servers[GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX].Start();
            servers[GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX].Start();

            logger.Write("\n Class: DispModuleQtClient; StartThreads method: user interface started!");
            ConsoleUserInterface.interact();

        }


        /// <summary>
        /// Creates a background thread for processing and forwarding top-level command messages
        /// </summary>
        /// <param name="data"></param>
        private void mainLinesCommunicThread(object data)
        {
            // the thread is focused on the maximum retention of the current connection with the dispatch console
            // works in a constant loop, listens for incoming messages from the Qt client,
            // responds according to the lists of current commands (high priority and medium priority)

            string Tag = "DispModuleClient class, mainLinesCommunicThread method: ";
            
            logger.Write($"\n {Tag} Thread mainLinesCommunicThread Started...\n");

            lock (sync1)
            {
                NamedPipeServerStream pipeServer =
                    new NamedPipeServerStream("firstLines", PipeDirection.InOut, 1);

                GlobalObjects.SetPipeChannelName(GlobalConstants.MAIN_AREAS_WORK, "mainLines");

                logger.Write("\n mainLinesCommunicThread metod: pipeServer created successfully.");

                int threadId = Thread.CurrentThread.ManagedThreadId;

                logger.Write($"mainLinesCommunicThread metod: thread[{threadId}].");

                logger.Write($"\n {Tag} We are waiting for a connection from the client side.");

                pipeServer.WaitForConnection();

                logger.Write("\n mainLinesCommunicThread metod: The pipeServer passed the WaitForConnection, the connection was created!.");

                GlobalObjects.SetPipeConnStatus(GlobalConstants.MAIN_AREAS_WORK, (int)PipeConnStatus.ESTABLISHED);
                GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_STANDBY_STATE);

                logger.Write($"mainLinesCommunicThread method: Client connected on thread[{threadId}].");


                int while_cycle_temp = 0;

                while (GlobalObjects.GetIsRunningThreads(GlobalConstants.MAIN_AREAS_WORK) == true)
                {
                    
                    logger.Write($" {Tag}: while_cycle_temp = { while_cycle_temp } .");

                    int currWorkingHours = (int)RetrStatus.IN_STANDBY_STATE; 
                    try
                    {

                        // The presence of cumulative current tasks for all work streams is checked (it is required to determine the status of the work mode)

                        if (СheckingForALLCurrentTasks())
                        {
                            logger.Write($" {Tag}: Workflows have work tasks .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_ACTIVE_MODE);
                            currWorkingHours = (int)RetrStatus.IN_ACTIVE_MODE;
                        }
                        else
                        {
                            logger.Write($" {Tag}: No current tasks, repeater in standby mode .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);
                            currWorkingHours = (int)RetrStatus.IN_STANDBY_STATE;
                        }

                        //-------------------------------------
                        // If there are current tasks, we will complete them, starting with the highest priority
                        if (Util.СheckForCurrentTasks())
                        {
                            // the highest priority tasks are performed first (if any)
                            bool isThereTask = false;       // no tasks by default

                            int foreachHPriorTasks = 0;

                            foreach (ATask task in GlobalObjects.HPriorTasks)
                            {
                                logger.Write($" {Tag}: foreachHPriorTasks = { foreachHPriorTasks } .");

                                if (task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION)
                                {
                                    logger.Write($" {Tag}: task.StatusTask = AWAIT_EXECUTION .");
                                    isThereTask = true;                                    
                                    break;
                                }

                                foreachHPriorTasks++;
                            }
                            //--------------
                            if (isThereTask)
                            {
                                logger.Write($" {Tag}: There are high priority tasks .");

                                // the highest priority tasks are carried out here
                                // not implemented...

                            }
                            //-----------------
                            isThereTask = false;       // by default there are no current tasks for the mainLinesCommunicThread thread

                            int foreachMPriorTasks = 0;
                            logger.Write($" {Tag}: foreachMPriorTasks = { foreachMPriorTasks } .");

                            foreach (ATask task in GlobalObjects.MPriorTasks)
                            {
                                // logger.Write($" {Tag}: foreachMPriorTasks = { foreachMPriorTasks } .");

                                if (task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION)
                                {
                                    isThereTask = true;
                                    logger.Write($"\n {Tag}: isThereTask = true .");

                                    if (currWorkingHours == (int)RetrStatus.IN_STANDBY_STATE)
                                    {
                                        logger.Write($"\n {Tag}: step2: repeater standby...");

                                        GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_ACTIVE_MODE);
                                        GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                                        GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                                        GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_ACTIVE_MODE);
                                        GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_ACTIVE_MODE);
                                        currWorkingHours = (int)RetrStatus.IN_ACTIVE_MODE;
                                    }

                                   logger.Write($"\n {Tag}: step2: there are current tasks!");
                                    break;
                                }

                                foreachMPriorTasks++;
                                logger.Write($" {Tag}: foreachMPriorTasks = { foreachMPriorTasks } .");
                            }

                            //------------------------------

                            if (isThereTask)
                            {
                                logger.Write($"\n {Tag}: current tasks are in progress...");

                                foreachMPriorTasks = 0;
                                logger.Write($"\n {Tag}: foreachMPriorTasks = { foreachMPriorTasks } .");

                                // выполняем текущие задачи
                                foreach (ATask task in GlobalObjects.MPriorTasks)
                                {
                                                                    
                                    int resultTask = -1;
                                        
                                    switch (task.IdTask)
                                    {
                                        case (int)CmdTYPE.OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS:

                                        logger.Write($"\n {Tag}: task.StatusTask = {task.StatusTask}");

                                        if (task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION)
                                        {
                                            
                                            task.StatusTask = (int)StatusTASK.TASK_AT_WORK;
                                            IsCompletionOperatingPart = false;

                                            logger.Write($"\n {Tag}: task.IdTask = OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS...");

                                            TaskExecCommunicThrdStrategy task1ExecStrategy
                                                = new Task1ExecMainLinesCommunicThrdStrategy(pipeServer, mPriorTasksForSecondProductThrd.Count, mPriorTasksForThirdProductThrd.Count, this);
                                                     
                                            task1ExecStrategy.TaskToProdThreadAdded += new TaskExecCommunicThrdStrategy.AddedTaskToProdThreadHandler(OnTaskToProdThreadAdded);

                                            task.TaskStatusChanged += new ATask.ChangedTaskStatusHandler(OnTaskStatusChanged);      // как пример, в этой задаче это не особо нужно

                                            int res = task1ExecStrategy.TaskExecute(task);

                                            logger.Write($"\n {Tag}: Mission accomplished! res = { res } .");

                                            if (res == 1)
                                            {
                                                task.StatusTask = (int)StatusTASK.OPER_PART_IS_COMPLETED;
                                                logger.Write($"\n {Tag}: task.StatusTask = THE_OPERATING_PART_IS_COMPLETED .");

                                                currNumTask = task.NumTask;
                                                currIdTask = task.IdTask;
                                                currPriority = task.Priority;
                                                currIdScenario = task.IdScenario;
                                                currIdProdThread = GlobalConstants.MAIN_AREAS_WORK;
                                                    
                                                IsCompletionOperatingPart = true;
                                            }                                                    
                                        }   
                                        break;                                             
                                    }

                                    foreachMPriorTasks++;
                                    logger.Write($" {Tag}: foreachMPriorTasks = { foreachMPriorTasks } .");                                    
                                }
                            }
                            else
                                logger.Write($"\n mainLinesCommunicThread metod: step2: no current tasks");
                            //-----------
                            // the pipe is read, if there are new messages, we execute the corresponding commands or events
                            // Not yet implemented...
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MAIN_AREAS_WORK) == (int)RetrStatus.IN_STANDBY_STATE)
                        {
                            logger.Write($"\n {Tag}: the stream fell asleep for 250 milliseconds, threadId = {threadId}");
                            Thread.Sleep(250);
                            logger.Write($"\n {Tag}: the stream woke up .");
                        }
                        else if (GlobalObjects.GetWorkingHours(GlobalConstants.MAIN_AREAS_WORK) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Write($"\n mainLinesCommunicThread metod: thread error exited, exception = {e}");
                    }

                    while_cycle_temp++;
                }
                logger.Write("\n mainLinesCommunicThread metod: thread mainLinesCommunicThread exited!.");
            }            
        }


        /// <summary>
        /// for messaging (commands and results of command execution) to establish 
        /// and further maintain a connection, tx-> rx, (Android-> AstraLinux) 
        /// second stream
        /// </summary>
        /// <param name="data"></param>
        private void exchangeMsgToEstablishConnTxRxThread(object data)
        {
            string Tag = "DispModuleQtClient class, exchangeMsgToEstablishConnTxRxThread method: ";
            logger.Write("Thread exchangeMsgToEstablishConnTxRxThread Started...\n");

            lock (sync2)
            {
                NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("secondTxRxLines", PipeDirection.InOut, numThreads);

                logger.Write("\n exchangeMsgToEstablishConnTxRxThread metod: pipeServer created successfully.");

                int threadId = Thread.CurrentThread.ManagedThreadId;

                logger.Write($"exchangeMsgToEstablishConnTxRxThread metod: thread[{threadId}].");


                logger.Write("\n We are waiting for a connection from the client side.");

                pipeServer.WaitForConnection();

                logger.Write("\n exchangeMsgToEstablishConnTxRxThread metod: The pipeServer passed the WaitForConnection, the connection was created!.");

                GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)PipeConnStatus.ESTABLISHED);
                GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);

                logger.Write($"exchangeMsgToEstablishConnTxRxThread method: Client connected on thread[{threadId}].");

                
                int while_cycle_temp = 0;
                logger.Write($"\n { Tag }  while_cycle_temp = {  while_cycle_temp } .");

                while (GlobalObjects.GetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD) == true)
                {
                   
                    int currWorkingHours = (int)RetrStatus.IN_STANDBY_STATE;
                    try
                    {

                        // We check the availability of current tasks (it is required to determine the status of the operating mode)

                        if (СheckingForALLCurrentTasks())
                        {
                            logger.Write($"\n { Tag }  There are current tasks!");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_ACTIVE_MODE);
                            currWorkingHours = (int)RetrStatus.IN_ACTIVE_MODE;
                        }
                        else
                        {
                            logger.Write($"\n { Tag }  no current tasks .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);
                            currWorkingHours = (int)RetrStatus.IN_STANDBY_STATE;
                        }
                        //-------------------------------------

                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            logger.Write($"\n { Tag }  repeater in active mode .");

                            // If there are current tasks, we will complete them, starting with the highest priority
                            if (СheckingForCurrentTasks(hPriorTasksForSecondProductThrd))
                            {
                                // priority tasks are carried out
                                logger.Write($"\n { Tag }  there are high priority tasks!");

                                // to realize...

                            }

                            if (СheckingForCurrentTasks(mPriorTasksForSecondProductThrd))
                            {
                                // current tasks are in progress
                                logger.Write($"\n { Tag }  current tasks are in progress!");

                                // current tasks are in progress                                

                                int foreachMPriorTasks = 0;

                                logger.Write($"\n {Tag}: foreachMPriorTasks = { foreachMPriorTasks } .");


                                foreach (ATask task in mPriorTasksForSecondProductThrd)
                                {
                                    if (task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION)
                                    {
                                        logger.Write($"\n {Tag}: task.StatusTask = TASK_AT_WORK .");

                                        task.StatusTask = (int)StatusTASK.TASK_AT_WORK;
                                        IsCompletionOperatingPart = false;

                                        int resultTask = -1;

                                        logger.Write($"\n {Tag}: task.IdTask = { task.IdTask }  .");

                                        switch (task.IdTask)
                                        {
                                            
                                            case (int)CmdTYPE.AGREE_WITH_DISP_CONSOLE_TO_ESTABL_TXRX_VOICE_CHANNEL:

                                                logger.Write($"\n {Tag}: task.IdTask = AGREE_WITH_DISP_CONSOLE_TO_ESTABL_TXRX_VOICE_CHANNEL  .");

                                                TaskExecCommunicThrdStrategy task19ExecStrategy
                                                    = new Task19ExecSecondCommunicThrdStrategy(pipeServer, mPriorTasksForFourthProductThrd.Count);

                                                task19ExecStrategy.TaskToProdThreadAdded += new TaskExecCommunicThrdStrategy.AddedTaskToProdThreadHandler(OnTaskToProdThreadAdded);

                                                
                                                task.TaskStatusChanged += new ATask.ChangedTaskStatusHandler(OnTaskStatusChanged);      // позволяет отслеживать полное выполнение задачи 

                                                resultTask = task19ExecStrategy.TaskExecute(task);

                                                logger.Write($"\n {Tag}: task # 19 completed, resultTask = { resultTask }  .");


                                                if (resultTask == 1)
                                                {
                                                    logger.Write($"\n {Tag}: task # 19 completed successfully!");                                                    

                                                    task.StatusTask = (int)StatusTASK.OPER_PART_IS_COMPLETED;
                                                    logger.Write($"\n {Tag}: task.StatusTask = OPER_PART_IS_COMPLETED");
                                                    
                                                    currNumTask = task.NumTask;
                                                    currIdTask = task.IdTask;
                                                    currPriority = task.Priority;
                                                    currIdScenario = task.IdScenario;
                                                    currIdProdThread = GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD;
                                                    
                                                    IsCompletionOperatingPart = true;
                                                    logger.Write($"\n {Tag}: AfterCompletedOperatingPart completed!");
                                                }
                                                else
                                                    logger.Write($"\n {Tag}: task # 19 completed with an error!");                                                

                                                break;                                                 
                                        }
                                    }
                                }

                                foreachMPriorTasks++;
                                logger.Write($"\n { Tag }  foreachMPriorTasks = { foreachMPriorTasks } .");
                            }
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD) == (int)RetrStatus.IN_STANDBY_STATE)
                        {
                            logger.Write($"\n { Tag }  he stream ( threadId = { threadId } ) fell asleep for 250 milliseconds...");
                            Thread.Sleep(250);
                            logger.Write($"\n { Tag }  the stream woke up! .");
                        }
                        else if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Write($"\n { Tag }  Error, { e.ToString() } .");
                    }

                    while_cycle_temp++;

                    logger.Write($"\n while_cycle_temp = { while_cycle_temp } .");
                }
            }
            logger.Write("\n exchangeMsgToEstablishConnTxRxThread metod: thread exchangeMsgToEstablishConnTxRxThread exited!.");
        }


        /// <summary>
        /// for messaging (commands and command execution results) to establish and further maintain a connection,
        /// rx-> tx, (AstraLinux-> Android) 
        /// third stream
        /// </summary>
        /// <param name="data"></param>
        private void exchangeMsgToEstablishConnRxTxThread(object data)
        {
            logger.Write("Thread exchangeMsgToEstablishConnRxTxThread Started...\n");            

            lock (sync3)
            {
                string Tag = "DispModuleQtClient class, exchangeMsgToEstablishConnRxTxThread method: ";

                NamedPipeServerStream pipeServer =
                    new NamedPipeServerStream("ThirdRxTxLines", PipeDirection.InOut, numThreads);

                logger.Write("\n exchangeMsgToEstablishConnRxTxThread metod: pipeServer created successfully.");

                int threadId = Thread.CurrentThread.ManagedThreadId;

                logger.Write($"exchangeMsgToEstablishConnRxTxThread metod: thread[{threadId}].");
               
                logger.Write("\n We are waiting for a connection from the client side.");

                pipeServer.WaitForConnection();

                logger.Write("\n exchangeMsgToEstablishConnRxTxThread metod: The pipeServer passed the WaitForConnection, the connection was created!.");

                GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)PipeConnStatus.ESTABLISHED);
                GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);

                logger.Write($"exchangeMsgToEstablishConnRxTxThread method: Client connected on thread[{threadId}].");

                
                int while_cycle_temp = 0;
                logger.Write($" {Tag} : while_cycle_temp = { while_cycle_temp } .");


                while (GlobalObjects.GetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD) == true)
                {
                                        
                    try
                    {

                        // We check the availability of current tasks (it is required to determine the status of the operating mode)

                        if (СheckingForALLCurrentTasks())
                        {
                            logger.Write($" {Tag} : Workflows have tasks .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_ACTIVE_MODE);
                        }
                        else
                        {
                            logger.Write($" {Tag} : Repeater standby .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            logger.Write($" {Tag} : current tasks are in progress...");

                            // If there are current tasks, we will complete them, starting with the highest priority
                            if (СheckingForCurrentTasks(hPriorTasksForThirdProductThrd))
                            {
                                logger.Write($" {Tag} : high priority tasks are being performed...");
                                // the priority tasks of the third thread are in progress
                                // to realize...

                            }

                            if (СheckingForCurrentTasks(mPriorTasksForThirdProductThrd))
                            {
                                logger.Write($" {Tag} : current tasks are in progress...");
                                // Next, the current tasks are performed
                                // to realize...

                            }
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD) == (int)RetrStatus.IN_STANDBY_STATE)
                        {
                            logger.Write($" {Tag} : the stream fell asleep for 250 ms .");
                            Thread.Sleep(250);
                            logger.Write($" {Tag} : the stream has awakened! .");
                        }
                        else if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            Thread.Sleep(50);
                        }

                    }
                    catch (Exception e)
                    {
                        logger.Write($" {Tag} : Exception e = { e.ToString() } .");
                    }

                }

                while_cycle_temp++;
                logger.Write($" {Tag} : while_cycle_temp = { while_cycle_temp } .");

            }

            logger.Write("\n exchangeMsgToEstablishConnRxTxThread metod: thread exchangeMsgToEstablishConnRxTxThread exited!.");
        }



        /// <summary>
        /// the fourth stream is required to transmit sound over the tx-rx path (the server sends, the Qt client plays)
        /// </summary>
        /// <param name="data"></param>
        private void soundTransmissViaTxRxPathThread(object data)
        {
            string Tag = "DispModuleQtClient class, soundTransmissViaTxRxPathThread method: ";

            int threadId = Thread.CurrentThread.ManagedThreadId;

            logger.Write($"{Tag}, threadId = { threadId }:, Thread soundTransmissViaTxRxPathThread Started...\n");


            lock (sync4)
            {
                NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("SoundTransTxRxLines", PipeDirection.InOut, numThreads);

                logger.Write($"\n {Tag}, threadId = { threadId }: soundTransmissVmetod: pipeServer created successfully.");

                logger.Write($" {Tag}, threadId = thread[{threadId}] .");

                logger.Write($"\n {Tag}, We are waiting for a connection from the client side .");

                pipeServer.WaitForConnection();

                logger.Write($"\n {Tag}: threadId = { threadId }: The pipeServer passed the WaitForConnection, the connection was created!.");

                GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)PipeConnStatus.ESTABLISHED);
                GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);

                logger.Write($"soundTransmissViaTxRxPathThread method: Client connected on thread[{threadId}].");

                int while_cycle_temp = 0;

                logger.Write($"\n { Tag },   while_cycle_temp = {  while_cycle_temp } .");

                while (GlobalObjects.GetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX) == true)
                {
                    logger.Write($"{TAG}, method: soundTransmissViaTxRxPathThread: while_cycle_temp = { while_cycle_temp } .");

                    int currWorkingHours = (int)RetrStatus.IN_STANDBY_STATE;

                    try
                    {
                        // a check is made for the presence of current tasks (it is required to determine the status of the operating mode)

                        if (СheckingForALLCurrentTasks())
                        {
                            logger.Write($"\n { Tag }: threadId = { threadId }:  There are current tasks!");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_ACTIVE_MODE);
                            currWorkingHours = (int)RetrStatus.IN_ACTIVE_MODE;
                        }
                        else
                        {
                            logger.Write($"\n { Tag }: threadId = { threadId }:  no current tasks .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);
                            currWorkingHours = (int)RetrStatus.IN_STANDBY_STATE;
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            logger.Write($"\n { Tag }: threadId = { threadId }:  repeater in active mode .");

                            // If there are current tasks, we will complete them, starting with the highest priority
                            if (СheckingForCurrentTasks(hPriorTasksForFourthProductThrd))
                            {
                                logger.Write($"{TAG}, threadId = { threadId }: there are current high priority tasks .");

                                // we carry out the priority tasks of the fourth stream
                                // to realize...

                            }
                            else
                                logger.Write($"{TAG}, threadId = { threadId }: no high priority tasks .");


                            if (СheckingForCurrentTasks(mPriorTasksForFourthProductThrd))
                            {
                                logger.Write($"{TAG}, threadId = { threadId }: there are current tasks .");

                                // выполним текущие задачи

                                int foreach_cycle_temp = 0;

                                foreach (ATask task in mPriorTasksForFourthProductThrd)
                                {
                                    logger.Write($"{TAG}, threadId = { threadId }: foreach_cycle_temp = { foreach_cycle_temp } .");

                                    if (task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION)
                                    {
                                        logger.Write($"{TAG}, threadId = { threadId }: task.StatusTask = AWAIT_EXECUTION .");

                                        task.StatusTask = (int)StatusTASK.TASK_AT_WORK;

                                        logger.Write($"\n {Tag}:  threadId = { threadId }: task.StatusTask = TASK_AT_WORK .");

                                        logger.Write($"\n {Tag}: threadId = { threadId }: task.IdTask = { task.IdTask }  .");

                                        
                                        int resultTask = -1;

                                        switch (task.IdTask)
                                        {
                                            
                                            case (int)CmdTYPE.START_SEND_VOICE_MSG:
                                                
                                                logger.Write($"{TAG}, threadId = { threadId }: task.IdTask = START_SENDING_VOICE_MSG ");

                                                TaskExecCommunicThrdStrategy task8ExecStrategy = new Task8ExecFourthCommunicThrdStrategy(pipeServer);

                                                task8ExecStrategy.TaskToProdThreadAdded += new TaskExecCommunicThrdStrategy.AddedTaskToProdThreadHandler(OnTaskToProdThreadAdded);

                                                task.TaskStatusChanged += new ATask.ChangedTaskStatusHandler(OnTaskStatusChanged);      // позволяет отслеживать полное выполнение задачи 

                                                logger.Write($"{TAG}, threadId = { threadId }: task8ExecutionStrategy created");

                                                resultTask = task8ExecStrategy.TaskExecute(task);

                                                logger.Write($"{TAG}, threadId = { threadId }: task # 8 completed, resultTask = { resultTask } .");

                                                if (resultTask == 1)
                                                {
                                                    logger.Write($"\n {Tag}: threadId = { threadId }: task # 8 completed successfully!");

                                                    // you should first get the actual task (after executing taskExecute,
                                                    // and you need to add the corresponding property to Task9ExecuteMainLinesCommunicThreadStrategy),
                                                    // implement.........................                                                    
                                                }
                                                else
                                                    logger.Write($"\n {Tag}: threadId = { threadId }: task # 8 completed with an error!");

                                                break;
                                                 
                                        }
                                    }

                                    foreach_cycle_temp++;
                                }
                            }
                            else
                                logger.Write($"{TAG}, threadId = { threadId }: no current tasks .");
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX) == (int)RetrStatus.IN_STANDBY_STATE)
                        {
                            Thread.Sleep(250);
                        }
                        else if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            Thread.Sleep(50);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Write($"{TAG}, method: soundTransmissViaTxRxPathThread: an error occurred in the loop; Exception e = { e.ToString() }");
                    }
                    while_cycle_temp++;
                }
            }
            logger.Write("\n soundTransmissViaTxRxPathThread metod: thread soundTransmissViaTxRxPathThread exited!");
        }


        /// <summary>
        /// the fifth stream is required to transmit sound over the rx-tx path (the client sends, the Android client plays)
        /// </summary>
        /// <param name="data"></param>
        private void soundTransmissViaRxTxPathThread(object data)
        {
            logger.Write("Thread soundTransmissViaRxTxPathThread Started...\n");
            string Tag = "soundTransmissViaRxTxPathThread metod: ";

            lock (sync5)
            {

                NamedPipeServerStream pipeServer =
                new NamedPipeServerStream("SoundTransRxTxLines", PipeDirection.InOut, numThreads);

                logger.Write("\n soundTransmissViaRxTxPathThread metod: pipeServer created successfully.");

                int threadId = Thread.CurrentThread.ManagedThreadId;

                logger.Write($"soundTransmissViaRxTxPathThread metod: thread[{threadId}].");
                                
                logger.Write($"\n {Tag} We are waiting for a connection from the client side.");

                pipeServer.WaitForConnection();

               logger.Write("\n soundTransmissViaRxTxPathThread metod: The pipeServer passed the WaitForConnection, the connection was created!.");

                GlobalObjects.SetPipeConnStatus(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)PipeConnStatus.ESTABLISHED);
                GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);

                logger.Write($"soundTransmissViaTxRxPathThread method: Client connected on thread[{threadId}].");

                
                while (GlobalObjects.GetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX) == true)
                {
                    try
                    {
                        // We check the availability of current tasks (it is required to determine the status of the operating mode)

                        if (СheckingForALLCurrentTasks())
                        {
                            logger.Write($"\n {Tag} There are current tasks .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_ACTIVE_MODE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_ACTIVE_MODE);
                        }
                        else
                        {
                            logger.Write($"\n {Tag} No current tasks .");

                            GlobalObjects.SetWorkingHours(GlobalConstants.MAIN_AREAS_WORK, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, (int)RetrStatus.IN_STANDBY_STATE);
                            GlobalObjects.SetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, (int)RetrStatus.IN_STANDBY_STATE);
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            logger.Write($"\n {Tag} Repeater in active mode! .");

                            if (СheckingForCurrentTasks(hPriorTasksForFifthProductThrd))
                            {
                                logger.Write($"\n {Tag} Priority tasks in progress ...");                                
                                

                            }

                            if (СheckingForCurrentTasks(mPriorTasksForFifthProductThrd))
                            {
                                logger.Write($"\n {Tag} Tasks in progress...");
                                

                            }
                        }
                        //-------------------------------------
                        if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX) == (int)RetrStatus.IN_STANDBY_STATE)
                        {
                            logger.Write($"\n {Tag} : The stream fell asleep for 250 ms .");
                            Thread.Sleep(250);
                            logger.Write($"\n {Tag} : The stream has awakened .");
                        }
                        else if (GlobalObjects.GetWorkingHours(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX) == (int)RetrStatus.IN_ACTIVE_MODE)
                        {
                            Thread.Sleep(50);
                        }

                    }
                    catch (Exception e)
                    {
                        logger.Write($"\n {Tag} : Error: Exception e = { e.ToString() } .");
                    }                    
                }                
            }

            logger.Write("\n soundTransmissViaRxTxPathThread metod: thread soundTransmissViaRxTxPathThread exited!.");
        }



        /// <summary>
        /// Checks if there is at least one open issue (for all production workflows)
        /// </summary>
        /// <returns>if there are tasks, returns TRUE</returns>
        private bool СheckingForALLCurrentTasks()
        {
            bool isThereTaskThreads = false;       // there are no current tasks by default

            int threadId = Thread.CurrentThread.ManagedThreadId;
            string Tag = "DispModuleQtClient class, СheckingForALLCurrentTasks method: ";

            
                logger.Write($"\n {Tag}: threadId = {threadId} :  method started!");

            // Check for the presence of current tasks (it is required to determine the status of the operating mode)
            if (Util.СheckForCurrentTasks())
                {
                    logger.Write($"\n {Tag}: threadId = {threadId} : Util.СheckingForCurrentTasks() = true .");

                    return true;
                }
                //------------------           

                if (СheckingForCurrentTasks(hPriorTasksForSecondProductThrd))
                {
                    logger.Write($"\n {Tag}:  threadId = {threadId} : СheckingForCurrentTasks(hPriorTasksForSecondProductThrd) = true .");

                    return true;
                }
                //--------------------------------
                if (СheckingForCurrentTasks(mPriorTasksForSecondProductThrd))
                {
                    logger.Write($"\n {Tag}:  threadId = {threadId} :  СheckingForCurrentTasks(mPriorTasksForSecondProductThrd) = true .");

                    return true;
                }
                //------------------
                if (СheckingForCurrentTasks(mPriorTasksForThirdProductThrd))
                {
                    logger.Write($"\n {Tag}:  threadId = {threadId} :  СheckingForCurrentTasks(mPriorTasksForThirdProductThrd) = true .");

                    return true;
                }
                //--------------------
                if (СheckingForCurrentTasks(hPriorTasksForThirdProductThrd))
                {
                    logger.Write($"\n {Tag}:  threadId = {threadId} :  СheckingForCurrentTasks(hPriorTasksForThirdProductThrd) = true .");

                    return true;
                }
                //------------------
                if (СheckingForCurrentTasks(mPriorTasksForFourthProductThrd))
                {
                    logger.Write($"\n {Tag}:  threadId = {threadId} :  СheckingForCurrentTasks(mPriorTasksForFourthProductThrd) = true .");

                    return true;
                }
                //------------------
                if (СheckingForCurrentTasks(hPriorTasksForFourthProductThrd))
                {
                   logger.Write($"\n {Tag}:  threadId = {threadId} :  СheckingForCurrentTasks(hPriorTasksForFourthProductThrd) = true .");

                    return true;
                }
                //------------------
                if (СheckingForCurrentTasks(mPriorTasksForFifthProductThrd))
                {
                    logger.Write($"\n {Tag}:  threadId = {threadId} :  СheckingForCurrentTasks(mPriorTasksForFifthProductThrd) = true .");

                    return true; 
                }
                //------------------
                if (СheckingForCurrentTasks(hPriorTasksForFifthProductThrd))
                {
                    logger.Write($"\n {Tag}:  threadId = {threadId} :  СheckingForCurrentTasks(hPriorTasksForFifthProductThrd) = true .");

                    return true;
                }            

            return isThereTaskThreads;
        }


        /// <summary>
        /// Checks if there are open tasks in the task list
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns>true if there are tasks, false otherwise</returns>
        private bool СheckingForCurrentTasks(List<ATask> tasks)
        {
            bool isThereTask = false;
            foreach (ATask task in tasks)
            {
                if (task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION || task.StatusTask == (int)StatusTASK.TASK_AT_WORK)
                   return true;                
            }
            return isThereTask;
        }


        /// <summary>
        /// Closes background threads
        /// </summary>
        /// <returns>1- if all threads have finished their work, 0 otherwise </returns>
        public override int CloseThreads()
        {
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MAIN_AREAS_WORK, false);
            
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD, false);
            
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD, false);

            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX, false);
            
            GlobalObjects.SetIsRunningThreads(GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX, false);

            Thread.Sleep(500);

            if (servers[GlobalConstants.MAIN_AREAS_WORK].IsAlive)
            {
                Thread.Sleep(300);
                if (servers[GlobalConstants.MAIN_AREAS_WORK].IsAlive)
                {
                    logger.Write("Thread stream mainLinesCommunicThread did not finish...\n");
                    return 0;
                }
                   
            }
            if (servers[GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD].IsAlive)
                return 0;
            if (servers[GlobalConstants.MSG_EXCHANGE_CONN_RXTX_THREAD].IsAlive)
                return 0;
            if (servers[GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX].IsAlive)
                return 0;
            if (servers[GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_RXTX].IsAlive)
                return 0;

            // then you need to ensure that the UserInterface is closed
            // to realize...

            // UserInterface.StatusCurrMonitor = (int)StatusCurrGUI.STOPPED;

            return 1;
        }        
    }
}
