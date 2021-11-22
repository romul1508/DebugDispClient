using DebugOmgDispClient.common;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.models.decorator.user;
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
    /// Execute task # 19 (AGREE_WITH_DISPATCH_CONSOLE_TO_ESTABLISH_TXRX_VOICE_CHANNEL) from the worker thread exchangeMsgToEstablishConnTxRxThread
    /// [ConcreteStrategy]
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 26.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>    
    public class Task19ExecSecondCommunicThrdStrategy : TaskExecCommunicThrdStrategy
    {
        private int mPriorTasksForFourthProductionThreadCount = 0;       // number of items in the list of current tasks for the fourth worker background thread

        /// <summary>
        /// Implementation of a specific strategy for the incoming task number 19
        /// </summary>
        /// <param name="pipeServer">A specific pipe that provides information exchange with the Qt client</param>

        public Task19ExecSecondCommunicThrdStrategy(NamedPipeServerStream pipeServer, int mPriorTasksForFourthProductionThreadCount)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task19ExecSecondCommunicThrdStrategy class, Constructor: ";

            logger.Write($" {Tag}; threadId = {threadId}; state: Started...\n");

            this.pipeServer = pipeServer;
            this.mPriorTasksForFourthProductionThreadCount = mPriorTasksForFourthProductionThreadCount;            
        }

        /// <summary>
        /// Ensures the execution of task No. 19 (preparation of an audio path to establish a tx-rx voice channel from the server to the dispatch console)
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public override int TaskExecute(ITask task)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task19ExecSecondCommunicThrdStrategy class, TaskExecute method: ";

            logger.Write($" {Tag}; threadId = {threadId}; state: Started...\n");

            lock (sync)
            {
                int resultTask = -1;
                
                List<IParamTask> _taskParams = task.getTaskParams();

                UserFullData user = null;

                int foreachTaskParam = 0;

                logger.Write($"\n { Tag }: threadId = {threadId}:  foreachTaskParam = { foreachTaskParam }");

                foreach (IParamTask param in _taskParams)
                {
                    try
                    {
                        switch (param.IdParam)
                        {
                            case (int)IdUserProperty.ID_USER:              
                                logger.Write($"\n { Tag }: threadId = {threadId}:  IdParam = ID_USER .");                
                                user = new UserFullData((int)param.Value);
                                logger.Write($"\n { Tag }: threadId = {threadId}:  IdParam.Id_user = { user.Id } .");
                                break;                         
                           case (int)IdUserProperty.CONN_ID:
                                logger.Write($"\n { Tag }: threadId = {threadId}:  IdParam = Id_CONN .");
                                user.ConnectionId = (string)param.Value;
                                logger.Write($"\n { Tag }: threadId = {threadId}:  IdParam.ConnectionId = {user.ConnectionId} .");
                                break;
                            case (int)IdUserProperty.USER_PRIORITY:
                                logger.Write($"\n { Tag }: threadId = {threadId}:  IdParam = USER_PRIORITY .");
                                user.Priority = (int)param.Value;
                                logger.Write($"\n { Tag }: threadId = {threadId}:  user.User_priority = {user.Priority} .");
                                break;
                            default:
                                logger.Write($"\n { Tag }: threadId = {threadId}: Error! User properties not defined, param.IdParam = {param.IdParam} .");                                
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Write($"\n { Tag }: Error. Exception e = {e.ToString()}");
                    }

                    foreachTaskParam++;
                    logger.Write($"\n { Tag }: threadId = {threadId}:  foreachTaskParam = { foreachTaskParam }");
                }

                resultTask = chekIndividVoiceCallToDispatchConsоle(user);

                logger.Write($"\n { Tag }: threadId = {threadId}:  resultTask = { resultTask }");
                
                if (resultTask == (int)ReadinessAcceptVoiceCall.READY_TO_RECEIVE_AUDIO_CALL)
                {
                    logger.Write("\n Class: Task19ExeSecondCommunicThrdStrategy, metod: TaskExecute: the client is ready to receive an audio call!");

                    // the fourth working thread must receive the command id_cmd = 7("Start sending voice messages") with parameters,
                    // the task is set(with parameters) - to transmit voice messages(rtp packets[tx - rx])


                    string taskDiscription8 = "START_SEND_VOICE_MSG (tx-rx voice channel)";

                    List<IParamTask> taskParams = new List<IParamTask>();

                    int foreachTaskParams = 0;
                    logger.Write($"\n { Tag }: threadId = {threadId}:  foreachTaskParams = { foreachTaskParams }");

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

                        logger.Write($"\n { Tag }: threadId = {threadId}:  parameters are attached to the task!");
                        
                        foreachTaskParams++;
                        logger.Write($"\n { Tag }: threadId = {threadId}:  foreachTaskParams = { foreachTaskParams }");
                    }
                    
                    ATask taskId8 = new OperTask(mPriorTasksForFourthProductionThreadCount, (int)CmdTYPE.START_SEND_VOICE_MSG, (int)PriorityTask.MIDDLE, taskDiscription8, 
                        (int)Scenario.START_AUDIO_PROXY_TX_RX, (int)StatusTASK.AWAIT_EXECUTION, taskParams);


                    logger.Write("\n Class: Task19ExeSecondCommunicThrdStrategy, metod: TaskExecute: Task <id=8> transferred to the fourth worker thread for execution");

                    AddTaskToProductThread(this, taskId8, GlobalConstants.MSG_SEND_VOICE_TO_DISP_CONSOLE_TXRX);

                    logger.Write($"\n { Tag }: threadId = {threadId}:  Task added to repository");

                    resultTask = 1;
                    // the task (to the fourth stream) is set - to open the exchange of RTP packets.
                    // The fourth stream signals the server to open an RTP stream. 
                }
                else if (resultTask == (int)ReadinessAcceptVoiceCall.NO_NETWORK_CONNECTION)
                {
                    logger.Write("\n mainLinesCommunicThread metod: step3: error while establishing network connection");
                    // here it is necessary to transfer information to the server: it is currently not possible to provide a voice call
                }

                return resultTask;
            }
        }

        /// <summary>
        /// checks the readiness of the start of the tx-rx audio call
        /// </summary>
        /// <param name="pipeServer">Pipe stream (for reading and writing forwarded messages)</param>
        /// <returns>returns: 1 - client is ready to receive rtp packets, 0 - an error occurred</returns>
        private int chekIndividVoiceCallToDispatchConsоle(UserFullData user)
        {
            // throw new NotImplementedException();
            int res = 1;

            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task19ExecuteSecondCommunicThreadStrategy class, chekIndividVoiceCallToDispatchConsоle method: ";

            logger.Write($" {Tag}; threadId = {threadId}; state: Started...\n");

            try
            {
                IStreamString streamString = new StreamString(pipeServer);

                logger.Write($"\n {Tag}, threadId = {threadId}: StreamString streamString created successfully!");

                logger.Write($"\n {Tag}, threadId = {threadId}: Let's write the line <id_cmd = 2> to the pipe file");

                StringBuilder str_command_2 = new StringBuilder("id_cmd=GET_BACKGROUND_INFO; id_user=");

                if (user != null)
                {
                    str_command_2.Append(user.Id);

                    logger.Write($"\n {Tag}, threadId = {threadId}: step 1: str_command_1 created!, str_command_2 = { str_command_2 }");

                    //-----------------------                    

                    if (user.ConnectionId != "")
                    {
                        
                        str_command_2.Append("; id_conn=");
                        
                        str_command_2.Append(user.ConnectionId);

                        logger.Write($"\n {Tag}, threadId = {threadId}: step 2: user_name added!, str_command_1 = { str_command_2 }");
                        
                    }
                    else 
                    {
                        logger.Write($"\n {Tag}, threadId = {threadId}: step 2: Error: Is not set user.ConnectionId = { user.ConnectionId }");
                    }

                    str_command_2.Append("; user_priority=");

                    str_command_2.Append(user.Priority);
                }
                else
                {
                    logger.Write($"\n {Tag}, threadId = {threadId}: Error: user = null");
                    return -1;
                }


                // you should end up with something like this:
                // string cmd2 = "id_cmd=2; id_user=9; id_conn=15; user_priority=1; ipAddrPort=10.1.6.87:5000"
                //-------------------------------
                string cmd2 = str_command_2.ToString();


                logger.Write($"\n {Tag}, threadId = {threadId}:  str_command_1 = { cmd2 }");

                //----------------------------------
                int len = 8;
               
                streamString.WriteString(cmd2);
                logger.Write($"\n {Tag}, threadId = {threadId}: The message was written to the pipe .");

                string gotFromFileData = streamString.ReadString(len);

                logger.Write($"\n {Tag}, threadId = {threadId}: Data received from the pipe: gotFromFileData = {gotFromFileData} .");

                StringBuilder strRes = new StringBuilder();

                int count = 0;
                char simb = '\0';
                while (count < len)
                {
                    simb = gotFromFileData.ElementAt(count);

                    // myLogger.Write($"\n {Tag}: threadId = {threadId}: count = {count}, simb = {simb}");

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

                if (result.Equals("id_cmd=8") )
                {
                    res = (int)ReadinessAcceptVoiceCall.READY_TO_RECEIVE_AUDIO_CALL;
                    logger.Write($"\n {Tag}, threadId = {threadId}: res = READY_TO_RECEIVE_AUDIO_CALL .");
                }
                else
                {
                    logger.Write($"\n {Tag}, threadId = {threadId}: ERROR = incorrect data format [Exchange with the dispatch console] .");
                    res = 0;
                }
            }
            catch (Exception e)
            {
                logger.Write($"{Tag}, threadId = {threadId}: Error! Exception e = { e.ToString() }...\n");
                res = (int)ReadinessAcceptVoiceCall.CONNECT_ERROR_COMMAND;
                logger.Write($"\n {Tag}, threadId = {threadId}: res = CONNECT_ERROR_COMMAND .");
            }

            return res;
        }
    }
}
