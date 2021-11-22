using DebugOmgDispClient.common;
using DebugOmgDispClient.events.infoclasses;
using DebugOmgDispClient.Interfaces;
using System;
using System.Collections.Generic;


namespace DebugOmgDispClient.tasks.abstr
{
    /// <summary>
    /// Abstract class - Current task, implements the ITask interface
    /// contains the properties and events necessary for operation
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 22.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public abstract class ATask : ITask
    {
        /// <summary>
        /// parent task, if null (no parents) then this is the root (main) task
        /// </summary>                                                                                           
        protected ITask parentTask = null;

        protected volatile int statusTask = (int)StatusTASK.AWAIT_EXECUTION;   // pending execution by default

        #region Events

        public delegate void ChangedTaskStatusHandler(object sender, ChangedTaskStatusArgs e);

        /// <summary>
        /// Occurs after the status of a task has changed
        /// </summary>
        public event ChangedTaskStatusHandler TaskStatusChanged;
        #endregion


        public virtual int Priority => throw new NotImplementedException();

        public virtual int NumTask => throw new NotImplementedException();

        public virtual int IdTask => throw new NotImplementedException();

        public virtual string TaskDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        /// <summary>
        /// returns the status of the task (1 - pending, 2 - in progress, 3 - operational part completed, 4 - completed)
        /// </summary>
        public int StatusTask 
        { 
            get => statusTask;
            set
            {
                if (statusTask != value)  // if changed, event is raised
                {
                    statusTask = value;

                    if (TaskStatusChanged != null)
                        TaskStatusChanged(this, new ChangedTaskStatusArgs(this, this.IdTask));
                }
            }            
        }

        public virtual int IdScenario 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }


        /// <summary>
        /// The parent task must be
        /// </summary>
        public ITask ParentTask
        {
            get => parentTask;
            set => parentTask = value;
        }       
        

        public virtual List<IParamTask> getTaskParams()
        {
            throw new NotImplementedException();
        }
    }
}
