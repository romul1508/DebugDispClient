using System.Collections.Generic;

namespace DebugOmgDispClient.Interfaces
{
    /// <summary>
    /// Interface used for all tasks
    /// author: Roman Ermakov
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Serial number of the task (numbering starts from 0 immediately after starting the application
        /// </summary>
        public int NumTask { get; }


        /// <summary>
        /// returns the task ID
        /// </summary>
        public int IdTask { get; }


        /// <summary>
        /// returns a short description of the task
        /// </summary>
        public string TaskDescription { get; set; }


        /// <summary>
        /// returns the status of the task (1 - pending, 2 - in progress, 3 - completed)
        /// </summary>
        public int StatusTask { get; set; }

        /// <summary>
        /// returns the priority of the task (HIGH - high, MIDDLE - medium)
        /// </summary>
        public int Priority { get; }


        /// <summary>
        /// ID of the scenario within which the current task is executed
        /// </summary>
        public int IdScenario { get; set; }


        /// <summary>
        /// A copy (clone) of the parent task, contains the necessary data
        /// </summary>
        public ITask ParentTask { get; set; }


        /// <summary>
        /// returns a list of parameters
        /// </summary>
        public List<IParamTask> getTaskParams();


        /// <summary>
        /// last completed task
        /// </summary>
        public static uint LastCompletedTask { get; set; }      
    }
}
