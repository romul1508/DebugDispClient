using DebugOmgDispClient.Interfaces;
using System.Collections.Generic;
using DebugOmgDispClient.tasks.abstr;
using DebugOmgDispClient.common;

namespace DebugOmgDispClient.Tasks
{
    /// <summary>
    /// Specific implementation of the ITask interface for solving current operational tasks
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>    
    public class OperTask : ATask
    {
        /// <summary>
        /// medium priority (default)
        /// </summary>
        private int priority = (int)PriorityTask.MIDDLE;        // medium priority (default)

        /// <summary>
        /// serial number of the task (numbering starts from 0 immediately after starting the application)
        /// </summary>
        private int numTask = 0;                                // the serial number of the task (numbering starts from 0 immediately
                                                                // after the application is launched)
        /// <summary>
        /// task id
        /// </summary>
        private int idTask = (int)CmdTYPE.OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS;

        /// <summary>
        /// Brief test description of the problem
        /// </summary>
        private string taskDescription = "The subscriber requested to open a voice message channel, confirm readiness.";    // default

        private int idScenario = (int)Scenario.NONE;              // pending execution by default

        /// <summary>
        /// list of task parameters
        /// </summary>
        private List<IParamTask> taskParams = null;               // list of task parameters

        /// <summary>
        /// list of child nodes, if empty, then there are no child tasks, is a "sheet" (the lowest priority task in the general task hierarchy)
        /// </summary>
        private List<ATask> childTasks = new List<ATask>();        // if empty, then there are no child tasks,
                                                                   // is a "sheet" (the lowest priority task in the overall task hierarchy)


        #region Constructors
        public OperTask(int numTask, int idTask)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            taskParams = new List<IParamTask>();
        }

        public OperTask(int numTask, int idTask, int idScenario)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.idScenario = idScenario;
            taskParams = new List<IParamTask>();
        }

        public OperTask(int numTask, int idTask, int idScenario, ITask _parentTask)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.idScenario = idScenario;
            taskParams = new List<IParamTask>();
            parentTask = new OperTask(_parentTask.NumTask, _parentTask.IdTask, _parentTask.IdScenario); 
        }

        public OperTask(int numTask, int idTask, string taskDescription)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.taskDescription = taskDescription;
            taskParams = new List<IParamTask>();
        }

        public OperTask(int numTask, int idTask, int priority, int idScenario)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.priority = priority;
            this.idScenario = idScenario;
            taskParams = new List<IParamTask>();
        }

        public OperTask(int numTask, int idTask, int priority, string taskDescription)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.priority = priority;
            this.taskDescription = taskDescription;
            taskParams = new List<IParamTask>();
        }

        public OperTask(int numTask, int idTask, List<IParamTask> taskParams)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.taskParams = taskParams;
        }

        public OperTask(int numTask, int idTask, int idScenario, List<IParamTask> taskParams)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.idScenario = idScenario;
            this.taskParams = taskParams;
        }

        public OperTask(int numTask, int idTask, string taskDescription, List<IParamTask> taskParams)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.taskDescription = taskDescription;
            this.taskParams = taskParams;
        }

        public OperTask(int numTask, int idTask, int priority, int idScenario, List<IParamTask> taskParams)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.priority = priority;
            this.idScenario = idScenario;
            this.taskParams = taskParams;
        }

        public OperTask(int numTask, int idTask, int priority, string taskDescription, List<IParamTask> taskParams)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.priority = priority;
            this.taskDescription = taskDescription;
            this.taskParams = taskParams;
        }

        public OperTask(int numTask, int idTask, int priority, string taskDescription, int statusTask, List<IParamTask> taskParams)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.priority = priority;
            this.taskDescription = taskDescription;
            this.taskParams = taskParams;
            this.statusTask = statusTask;
        }

        public OperTask(int numTask, int idTask, int priority, string taskDescription, int idScenario, int statusTask, List<IParamTask> taskParams)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.priority = priority;
            this.taskDescription = taskDescription;
            this.taskParams = taskParams;
            this.statusTask = statusTask;
            this.IdScenario = idScenario;

        }

        public OperTask(int numTask, int idTask, int priority, string taskDescription, int idScenario, List<IParamTask> taskParams, ITask _parentTask)
        {
            this.numTask = numTask;
            this.idTask = idTask;
            this.priority = priority;
            this.taskDescription = taskDescription;
            this.taskParams = taskParams;
            this.IdScenario = idScenario;

            if(_parentTask != null)
                parentTask = new OperTask(_parentTask.NumTask, _parentTask.IdTask, _parentTask.Priority, _parentTask.TaskDescription, _parentTask.IdScenario, _parentTask.getTaskParams() );

        }
        #endregion

        /// <summary>
        /// returns the priority of the task (HIGH - high, MIDDLE - medium)
        /// </summary>
        public override int Priority
        {
            get => priority;            
        }


        /// <summary>
        /// serial number of the task (numbering starts from 0 immediately after starting the application)
        /// </summary>
        public override int NumTask
        {
            get => numTask;
        }


        /// <summary>
        /// returns the task identifier
        /// </summary>
        public override int IdTask
        {
            get => idTask;
        }

        /// <summary>
        /// returns a short description of the task
        /// </summary>
        public override string TaskDescription
        { 
            get => taskDescription;
            set => taskDescription = value;
        }


        /// <summary>
        /// identifier of the scenario within which this task is executed
        /// </summary>
        public override int IdScenario 
        { 
            get => idScenario; 
            set => idScenario = value; 
        }


        /// <summary>
        /// List of subtasks, child tasks
        /// </summary>
        public List<ATask> ChildTasks { get => childTasks; }


        /// <summary>
        /// last completed task
        /// </summary>
        /// <returns></returns>
        public override List<IParamTask> getTaskParams()
        {
            return taskParams;
        }
    }
}
