using DebugOmgDispClient.events.infoclasses;

namespace DebugOmgDispClient.Interfaces
{
    /// <summary>
    /// Interface, ensures interaction with the dispatch console
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 23.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public interface IDispModuleClient
    {
        /// <summary>
        /// Runs workers background streams
        /// </summary>
        public void StartThreads();

        /// <summary>
        /// Allows you to explicitly generate an event - "Made the operating part of the task"  
        /// </summary>
        /// <param name="sender">Object sending an event</param>
        /// <param name="task">Task with parameters</param>
        /// <param name="toIdThread">The identifier of the working flux to which the current task is assigned</param>
        public void AfterCompletedOperatingPart(object sender,  int numTask, int idTask, int priority, int idScenario, int toIdThread);

        /// <summary>
        /// Property showing whether the operating part of the task
        /// </summary>
        public bool IsCompletionOperatingPart { get; set; }

        ///// <summary>
        ///// Event Handler - Add a task to the repository
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        public void OnTaskToProdThreadAdded(object sender, AddedTaskToProdThreadArgs e);


        /// <summary>
        /// Event handler - "Task status changed"
        /// </summary>
        /// <param name="sender">instance, посылающий событие</param>
        /// <param name="e">Содержит информацию, связанную с событием</param>
        public void OnTaskStatusChanged(object sender, ChangedTaskStatusArgs e);


        /// <summary>
        /// Closes the worker threads of the application and releases the occupied resources
        /// </summary>
        /// <returns></returns>
        public int CloseThreads();
        
    }
}
