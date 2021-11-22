using DebugOmgDispClient.common;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.pipe.stream;
using DebugOmgDispClient.tasks.abstr;
using DebugOmgDispClient.tasks.parameters;
using DebugOmgDispClient.Tasks;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace DebugOmgDispClient.services.strategy.dispmodule.execute.tasks
{

    /// <summary>
    /// Run task # 1 (OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS) from the MainLinesCommunicThread workflow
    /// [ConcreteStrategy]
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 23.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class Task1ExecMainLinesCommunicThrdStrategy : TaskExecCommunicThrdStrategy
    {
        
        int mPriorTasksForSecondProductThreadCount = 0;      // number of items in the list of current tasks for the second worker background thread
        int mPriorTasksForThirdProductThreadCount = 0;       // number of items in the list of current tasks for the third worker background thread

        IDispModuleClient dispModule = null;                 // provides interaction with the dispatch console

        /// <summary>
        /// Implementation of a specific strategy for the incoming task number 1
        /// </summary>
        /// <param name="pipeServer">A specific pipe that provides information exchange with the Qt client</param>
        /// <param name="mPriorTasksForSecondProductionThreadCount">number of items in the list of current tasks for the second worker background thread</param>
        /// <param name="mPriorTasksForThirdProductionThreadCount">number of items in the list of current tasks for the third worker background thread</param>
        public Task1ExecMainLinesCommunicThrdStrategy(NamedPipeServerStream pipeServer)
        {
            this.pipeServer = pipeServer;            
        }

        /// <summary>
        /// Implementation of a specific strategy for the incoming task number 1
        /// </summary>
        /// <param name="pipeServer">A specific pipe that provides information exchange with the Qt client</param>
        /// <param name="mPriorTasksForSecondProductionThreadCount">number of items in the list of current tasks for the second worker background thread</param>
        /// <param name="mPriorTasksForThirdProductionThreadCount">number of items in the list of current tasks for the third worker background thread</param>
        public Task1ExecMainLinesCommunicThrdStrategy(NamedPipeServerStream pipeServer, int mPriorTasksForSecondProductionThreadCount, int mPriorTasksForThirdProductionThreadCount, IDispModuleClient dispModule)
        {
            this.pipeServer = pipeServer;
            this.mPriorTasksForSecondProductThreadCount = mPriorTasksForSecondProductionThreadCount;     
            this.mPriorTasksForThirdProductThreadCount = mPriorTasksForThirdProductionThreadCount;
            this.dispModule = dispModule;
        }


        /// <summary>
        /// checks the possibility of a new audio call tx-rx
        /// </summary>
        /// <param name="pipeServer">Pipe stream (for reading and writing forwarded messages)</param>
        /// <returns>returns: 1 - the client is ready to accept an audio call, 2 - the client is talking (the connection is busy), 3 - an error while establishing a network connection</returns>
        private int chekIndividVoiceCallToDispatchConsоle()
        {
            int res = (int)ReadinessAcceptVoiceCall.READY_TO_RECEIVE_AUDIO_CALL;

            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task1ExecMainLinesCommunicThrdStrategy class, chekIndividVoiceCallToDispatchConsоle method: ";

            logger.Write($" {Tag}; threadId = {threadId}; state: Started...\n");

            try
            {
                IStreamString streamString = new StreamString(pipeServer);

                logger.Write($"\n {Tag}: threadId = {threadId}: StreamString streamString created successfully!");

                logger.Write($"\n {Tag}: threadId = {threadId}: Let's write the line <id_cmd = 1> to the pipe file");
                                
                //----------------------------------                
                streamString.WriteString("id_cmd=OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS");
                
                int len = 9;
                
                string gotFromFileData = streamString.ReadString(len);

                logger.Write($"\n {Tag}: threadId = {threadId}: gotFromFileData = {gotFromFileData}");

                //string result = gotFromFileData.TrimEnd();

                // logger.Write($"\n {Tag}: threadId = {threadId}: result = {result} .");

                StringBuilder strRes = new StringBuilder();

                int count = 0;
                char simb = '\0';
                while(count < len)
                {
                    simb = gotFromFileData.ElementAt(count);

                    // logger.Write($"\n {Tag}: threadId = {threadId}: count = {count}, simb = {simb}");

                    if (simb != ' ')
                    {
                        strRes.Append(simb);
                        // myLogger.Write($"\n {Tag}: threadId = {threadId}: simb added!");
                        count++;
                    }
                    else
                        break;
                }

                string result = strRes.ToString();

                if (result.Equals("id_cmd=10") )
                {
                    logger.Write($"\n {Tag}: threadId = {threadId}: res = READY_TO_RECEIVE_AUDIO_CALL");
                    res = (int)ReadinessAcceptVoiceCall.READY_TO_RECEIVE_AUDIO_CALL;
                }
                

                else if (result.Equals("id_cmd=9") )
                {
                    logger.Write($"\n {Tag}: threadId = {threadId}: res = CLIENT_IS_TALKING");
                    res = (int)ReadinessAcceptVoiceCall.CLIENT_IS_TALKING;
                }
                else
                {
                    logger.Write($"\n {Tag}, threadId = {threadId}: ERROR = incorrect data format [Exchange with the dispatch console] .");
                    res = 0;
                }

            }
            catch (Exception e)
            {
                logger.Write($"{Tag}: threadId = {threadId}: Error! Exception e = { e.ToString() }...\n");
                logger.Write($"\n {Tag}: threadId = {threadId}: res = CONNECT_ERROR_COMMAND");
                res = (int)ReadinessAcceptVoiceCall.CONNECT_ERROR_COMMAND;
            }


            return res;
        }

        /// <summary>
        /// Complete task 1
        /// </summary>
        /// <param name="task">The resulting task with parameters</param>
        /// <param name="mPriorTasksForSecondProductionThreadCount">List size List[ITask] mPriorTasksForSecondProductionThread current tasks of the second workflow 
        /// (defined and initialized in OmgDispClient.service.DispModuleClient.cs)</param>
        /// <returns></returns>
        public override int TaskExecute(ITask task)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task1ExecMainLinesCommunicThrdStrategy class, TaskExecute method: ";

            logger.Write($" {Tag}; threadId = {threadId}; state: Started...\n");

            lock (sync)
            {
                int resultTask = -1;
                
                List<IParamTask> _taskParams = task.getTaskParams();


                resultTask = chekIndividVoiceCallToDispatchConsоle();

                if (resultTask == (int)ReadinessAcceptVoiceCall.READY_TO_RECEIVE_AUDIO_CALL)
                {
                    logger.Write($"\n {Tag}: threadId = {threadId}: step3: the client is ready to receive an audio call!");

                    // the command id_cmd = 1 must be sent to the dispatch console ("Get background information") с параметрами with parameters
                    // the task (with parameters) is set for the second thread (to provide information exchange, agree with the Qt client to establish a tx-rx voice channel)

                    // string taskDiscription = "Get background information!";
                    string taskDiscription19 = "Agree with the dispatch console to establish a tx-rx voice channel";
                    string taskDiscription20 = "Agree with the dispatch console to establish a rx-tx voice channel";

                    List<IParamTask> taskParams = new List<IParamTask>();

                    int foreachTaskParams = 0;
                    logger.Write($"\n { Tag }: threadId = {threadId}:  foreachTaskParams = { foreachTaskParams }");

                    logger.Write($"\n { Tag }: threadId = {threadId}:  _taskParams.Count = { _taskParams.Count }");

                    foreach (IParamTask paramTask in _taskParams)
                    {
                        
                        IParamTask taskParam = null;


                        switch (paramTask.IdType)
                        {
                            case (int)ParamTypeID.BYTE:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (byte)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.SHORT:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (short)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.USHORT:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (ushort)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.WCHAR:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (char)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.INT:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (int)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.UINT:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (uint)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.FLOAT:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (float)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.LONG:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (long)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.ULONG:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (ulong)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.DOUBLE:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (double)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.STRING:
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, paramTask.IdType, (string)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.OBJECT:
                                object obj = paramTask.Value;
                                Type obj_type = obj.GetType();
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, obj_type, (object)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            case (int)ParamTypeID.ARRAY:
                                object array_obj = paramTask.Value;
                                Type obj_arr_type = array_obj.GetType();
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, obj_arr_type, (object)paramTask.Value, paramTask.ParameterDescription);
                                break;
                            default:
                                object _obj = paramTask.Value;
                                Type objType = _obj.GetType();
                                taskParam = new ParamTask(paramTask.IdParam, paramTask.NameParam, objType, (object)paramTask.Value, paramTask.ParameterDescription);
                                break;
                        }
                        
                        taskParams.Add(taskParam);

                        logger.Write($"\n { Tag }: threadId = {threadId}:  Parameter added successfully");

                        foreachTaskParams++;
                        logger.Write($"\n { Tag }: threadId = {threadId}:  foreachTaskParams = { foreachTaskParams }");
                    }

                   
                    ATask taskId19 = new OperTask(mPriorTasksForSecondProductThreadCount, (int)CmdTYPE.AGREE_WITH_DISP_CONSOLE_TO_ESTABL_TXRX_VOICE_CHANNEL, (int)PriorityTask.MIDDLE, 
                        taskDiscription19, (int)Scenario.START_AUDIO_PROXY_TX_RX, (int)StatusTASK.AWAIT_EXECUTION, taskParams);

                    // An event will be added to dispModule - "task completed",
                    // dispModule generates the corresponding event when it occurs,
                    // the listener and handler is the BackendProxyServiceManager backendProxyServiceManager,
                    // which stores the corresponding information and statuses in global objects,
                    // and also transfers data to other recipients, for example, ProxySignalRService,
                    // and that, in turn, can inform VoIP in this way, the statuses of tasks and their sequence are monitored,
                    // which allows you to fully control the progress of tasks
                    ATask taskId20 = new OperTask(mPriorTasksForThirdProductThreadCount, (int)CmdTYPE.AGREE_WITH_DISP_CONSOLE_TO_ESTABL_RXTX_VOICE_CHANNEL, (int)PriorityTask.MIDDLE, 
                        taskDiscription20, (int)Scenario.START_AUDIO_PROXY_TX_RX, (int)StatusTASK.AWAIT_EXECUTION, taskParams);

                    // we call an event by which tasks taskId18 and taskId19 arrive in the lists of tasks List<ITask>mPriorTasksForSecondProductThread and List<ITask>mPriorTasksForThirdProductThread

                    logger.Write($"\n {Tag}: threadId = {threadId}: Task <id=19> transferred to the second worker thread for execution");

                    AddTaskToProductThread(this, taskId19, GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD);

                    logger.Write($"\n {Tag}: threadId = {threadId}: Task <id=20> transferred to the third worker thread for execution");

                    AddTaskToProductThread(this, taskId20, GlobalConstants.MSG_EXCHANGE_CONN_TXRX_THREAD);

                    resultTask = 1;
                    // the task is set (for the fourth and fifth streams) - to open the exchange of RTP packets
                    // you also need to give a signal to the server - open an RTP stream 
                }
                else if (resultTask == (int)ReadinessAcceptVoiceCall.CLIENT_IS_TALKING)
                {
                    logger.Write($"\n {Tag}: threadId = {threadId}: the client is talking (the connection is busy)");
                    // 1. the priorities of the speaking user are mapped to the caller
                    // 2. if the priority of the caller is higher, then the corresponding command is transmitted to the dispatch console)


                }
                else if (resultTask == (int)ReadinessAcceptVoiceCall.NO_NETWORK_CONNECTION)
                {
                    logger.Write($"\n {Tag}: threadId = {threadId}: step3: error while establishing network connection");
                    // it is necessary to transfer information to the server: "it is not possible to provide a voice call at the moment"

                }
                else if (resultTask == (int)ReadinessAcceptVoiceCall.CONNECT_ERROR_COMMAND)
                {
                    logger.Write($"\n {Tag}: threadId = {threadId}: step3: connection error [for command exchange], the channel name may be incorrectly specified");

                    logger.Write($"\n {Tag}: threadId = {threadId}: step3: connection error [for command exchange], the channel name may be incorrectly specified");                    
                }

                logger.Write($"\n {Tag}: threadId = {threadId}: resultTask = {resultTask}");

                return resultTask;
            }
        }       
    }
}
