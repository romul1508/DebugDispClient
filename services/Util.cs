using DebugOmgDispClient.logging.Internal;
using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.models;
using System.Threading;
using DebugOmgDispClient.common;

namespace DebugOmgDispClient.services
{
    /// <summary>
    /// Designed to solve auxiliary and secondary tasks (for the main first thread)
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class Util
    {
        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;

        /// <summary>
        /// Checks for current tasks
        /// </summary>
        /// <returns> true if there are tasks, false otherwise </returns>
        public static bool СheckForCurrentTasks()
        {
            bool isThereTask = false;       // no tasks by default

            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Util class, СheckForCurrentTasks method: ";

            // int len = GlobalObjects.HPriorTasks.Count;

            logger.Write($"\n { Tag }: threadId = {threadId}:  GlobalObjects.HPriorTasks.Count = { GlobalObjects.HPriorTasks.Count }");

            int foreachHPriorTasks = 0;
            int foreachMPriorTasks = 0;

            logger.Write($"\n { Tag }: threadId = {threadId}:  foreachHPriorTasks = { foreachHPriorTasks }");

            foreach (ITask task in GlobalObjects.HPriorTasks)
            {
                if(task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION || task.StatusTask == (int)StatusTASK.TASK_AT_WORK)
                {
                    isThereTask = true;
                    return isThereTask;
                }

                foreachHPriorTasks++;
                logger.Write($"\n { Tag }: threadId = {threadId}:  foreachHPriorTasks = { foreachHPriorTasks }");
            }

            logger.Write($"\n { Tag }: threadId = {threadId}:  GlobalObjects.MPriorTasks.Count = { GlobalObjects.MPriorTasks.Count }");

            foreach (ITask task in GlobalObjects.MPriorTasks)
            {
                if (task.StatusTask == (int)StatusTASK.AWAIT_EXECUTION || task.StatusTask == (int)StatusTASK.TASK_AT_WORK)
                {
                    isThereTask = true;
                    return isThereTask;
                }

                foreachMPriorTasks++;
                logger.Write($"\n { Tag }: threadId = {threadId}:  foreachMPriorTasks = { foreachMPriorTasks }");
            }

            logger.Write($"\n { Tag }: threadId = {threadId}:  isThereTask = { isThereTask }");

            return isThereTask;
        }
    }
}
