
namespace DebugOmgDispClient.Interfaces
{
    /// <summary>
    /// Strategy pattern interface for executing workflows of current and urgent (high-priority) tasks
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public interface ITaskExecutionStrategy
    {
        /// <summary>
        /// To perform the task
        /// </summary>
        /// <returns></returns>
        int TaskExecute(ITask task);        
    }

}
