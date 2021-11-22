
namespace DebugOmgDispClient.common
{
    /// <summary>
    /// current status of the user console interface
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 20.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public enum StatusCurrGUI
    {
        /// <summary>
        /// interact-method at work
        /// </summary>
        AT_WORK = 1,

        /// <summary>
        /// Stopped
        /// </summary>
        STOPPED = 2
    }

    //-------------------
    public enum ParamTypeID
    {
        /// <summary>
        /// matches System.Byte
        /// </summary>
        BYTE = 1,

        /// <summary>
        /// matches char, used to store double-byte unicode text characters
        /// </summary>
        WCHAR = 2,

        /// <summary>
        /// matches Int16, the word is a signed two-byte integer
        /// </summary>
        SHORT = 3,

        /// <summary>
        /// matches UInt16, the word is an unsigned two-byte integer
        /// </summary>
        USHORT = 4,

        /// <summary>
        /// matches Int32, a signed four-byte integer
        /// </summary>
        INT = 5,

        /// <summary>
        /// matches UInt32, an unsigned four-byte integer
        /// </summary>
        UINT = 6,

        /// <summary>
        /// matches float (4 bytes)
        /// </summary>
        FLOAT = 7,

        /// <summary>
        /// Int64, eight-byte signed integer
        /// </summary>
        LONG = 8,

        /// <summary>
        /// uInt64, 8-byte unsigned integer
        /// </summary>
        ULONG = 9,

        /// <summary>
        /// double (eight bytes)
        /// </summary>
        DOUBLE = 10,

        /// <summary>
        /// matches System.String
        /// </summary>
        STRING = 11,

        /// <summary>
        /// matches System.Object  (assumes the presence of attributes, i.e. nested parameters)
        /// </summary>
        OBJECT = 12,

        /// <summary>
        /// array of standard variables
        /// </summary>
        ARRAY = 13,

        /// <summary>
        /// array of objects
        /// </summary>
        OBJECT_ARRAY = 14
    }

    //-------------------
    /// <summary>
    /// Task priority
    /// </summary>
    public enum PriorityTask
    {

        /// <summary>
        /// low priority
        /// </summary>
        // LOW = 0,                         

        /// <summary>
        /// medium priority
        /// </summary>
        MIDDLE = 1,

        /// <summary>
        /// high priority
        /// </summary>
        HIGH = 2                            
    }

    //-------------------
    /// <summary>
    /// Task status
    /// </summary>
    public enum StatusTASK
    {
        /// <summary>
        /// waiting for execution to start
        /// </summary>
        AWAIT_EXECUTION = 1,

        /// <summary>
        /// the task is in progress, after performing the initial check operations, the task is assigned the TASK_AT_WORK status,
        /// this leads to the generation of the TaskAtWorkEvent event (system components and testers are notified about the current status of the task, whether there are any errors, if everything is going as it should, for example, 
        /// the task can go to the TASK_AT_WORK status only after the completion [or change of the status] of another task)
        /// an event is generated in an object of the OperationalTask class, a handler in an object of the EventSink class, a listener (subscriber) of DispModuleClient
        /// </summary>
        TASK_AT_WORK = 2,

        /// <summary>
        /// the operational part is executed when all functional operations of the task are completed, but the subtasks (child tasks) are executed,
        /// accompanied by the AfterCompletionOperatingPartHandler event, the handler of which requires appropriate implementation,
        /// an event is generated in an object of the DispModuleClient class, a handler in an object of the BackendServiceManager class
        /// </summary>
        OPER_PART_IS_COMPLETED = 3,

        /// <summary>
        /// the task is completed, all child tasks have completed their work,
        /// after execution, the TaskCompletionEvent event is generated (an event is generated in the object of the DispModuleClient class, the handler is in the object of the BackendServiceManager class)
        /// </summary>
        TASK_COMPLETED = 4,

        /// <summary>
        /// The task could not be completed, the TaskCouldNotBeCompletedEvent event is raised,
        /// an event is raised in an object of the DispModuleClient class, a handler in an object of the BackendServiceManager class
        /// </summary>
        TASK_COULD_NOT_BE_COMPLETED = 5,

        /// <summary>
        /// Task canceled, essentially like TASK_COMPLETED or TASK_COULD_NOT_BE_COMPLETED means closing the task and all subtasks,
        /// a TaskCanceledEvent event is generated, an event is generated in an object of the DispModuleClient class, a handler in an object of the BackendServiceManager class
        /// </summary>
        TASK_CANCELED = 6
    }
    //-------------------
    /// <summary>
    /// An enumeration of scenarios with which tasks can be associated
    /// </summary>
    public enum Scenario
    {
        /// <summary>
        /// Scenario not set, task is not associated with any scenario
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Providing transmission of audio messages from user devices of the system to the dispatch console in the direction (Android -> Linux)
        /// </summary>
        START_AUDIO_PROXY_TX_RX = 1,

        /// <summary>
        /// Providing transmission of audio messages from the system dispatch console to users' devices in the direction (Linux -> Android)
        /// </summary>
        START_AUDIO_PROXY_RX_TX = 2
    }
    //-------------------
    /// <summary>
    /// User priority
    /// </summary>
    public enum PriorityUser
    {
        /// <summary>
        /// low priority
        /// </summary>
        LOW = 3,

        /// <summary>
        /// medium priority
        /// </summary>
        MIDDLE = 2,

        /// <summary>
        /// high priority
        /// </summary>
        HIGH = 1                            
    }
    //-------------------
    /// <summary>
    /// possible states of a network connection (or opening a pipe)
    /// </summary>
    public enum PipeConnStatus
    {
        /// <summary>
        /// Connection status undefined
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Connection established
        /// </summary>
        ESTABLISHED = 1,


        /// <summary>
        /// Connection is established
        /// </summary>
        IS_BEING_ESTABLISHED = 2,

        /// <summary>
        /// Connection not established
        /// </summary>
        NOT_ESTABLISHED = 3
    }
    //-------------------
    /// <summary>
    /// User property identifier
    /// </summary>
    public enum IdUserProperty
    {
        /// <summary>
        ///User ID
        /// </summary>
        ID_USER = 1,

        /// <summary>
        /// Username
        /// </summary>
        USER_NAME = 2,

        /// <summary>
        /// Network connection identifier
        /// </summary>
        CONN_ID = 3,

        /// <summary>
        /// Device ID
        /// </summary>
        // ID_DEV = 3,                          


        /// <summary>
        /// User priority
        /// </summary>
        USER_PRIORITY = 4,


        /// <summary>
        /// IP address and port
        /// </summary>
        IP_ADDR_PORT = 5,


        /// <summary>
        /// Last time user was online
        /// </summary>
        LAST_ONLINE = 6,

        /// <summary>
        /// List of user role names
        /// </summary>
        USER_ROLES = 7,


        /// <summary>
        /// List of user group names
        /// </summary>
        USER_GROUPS = 8                         
    }
    //-------------------
    /// <summary>
    /// possible operating modes of the repeater (in the active phase, in the standby state)
    /// </summary>
    public enum RetrStatus
    {
        /// <summary>
        /// Repeater status undefined
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Repeater in active mode of operation (when there are current tasks)
        /// </summary>
        IN_ACTIVE_MODE = 1,                         


        /// <summary>
        /// Repeater in standby state
        /// </summary>
        IN_STANDBY_STATE = 2                        
    }
    //------------------
    /// <summary>
    /// enum CmdTYPE - describes the types of transmit-receive commands (classification))
    /// </summary>

    public enum CmdTYPE
    {
        /// <summary>
        /// The subscriber requested to open a channel for the transmission of voice messages, 
        /// confirm readiness
        /// </summary>
        OPEN_VOICE_MSG_CHANNEL__CONFIRM_READINESS = 1,  

        /// <summary>
        /// Get background information
        /// </summary>
        GET_BACKGROUND_INFO = 2,

        /// <summary>
        /// Completed, submit next task
        /// </summary>
        COMPL_SEND_NEXT_TASK = 3,

        /// <summary>
        /// Submit a new task
        /// </summary>
        SEND_NEW_TASK = 4,

        /// <summary>
        /// Close client connection
        /// </summary>
        CLOSE_CLIENT_CONN = 5,

        /// <summary>
        /// Close all connections to the server
        /// </summary>
        CLOSE_ALL_CONNS_TO_SERVER = 6,

        /// <summary>
        /// Request accepted, await response
        /// </summary>
        REQUEST_ACCEPT__AWAIT_RESPONSE = 7,

        /// <summary>
        /// Start sending voice messages
        /// </summary>
        START_SEND_VOICE_MSG = 8,

        /// <summary>
        /// The communication channel is busy, call back later
        /// </summary>
        CHANNEL_IS_BUSY__CALL_BACK_LATER = 9,

        /// <summary>
        /// I confirm my readiness, send the initial data
        /// </summary>
        CONFIRM_MY_READINESS__SEND_INIT_DATA = 10,  

        /// <summary>
        /// End communication session
        /// </summary>
        END_COMMUNIC_SESSION = 11,          

        /// <summary>
        /// Communication session ended, switch to standby mode
        /// </summary>
        COMMUNICATION_SESSION_ENDED__AWAIT_FURTHER_INSTRUCTIONS = 12,   

        /// <summary>
        /// No tasks, send further instructions
        /// </summary>
        NO_TASKS__SEND_FURTHER_INSTRUCTIONS = 13,   

        /// <summary>
        /// No new tasks, await further instructions
        /// </summary>
        NO_NEW_TASKS__AWAIT_FURTHER_INSTRUCTIONS = 14,

        /// <summary>
        /// No new tasks, you are on standby
        /// </summary>
        NO_NEW_TASKS__STAY_IDLE = 15,   // было 15

        /// <summary>
        /// The communication channel is busy, terminate the current session
        /// </summary>
        COMMUNIC_CHANNEL_IS_BUSY__TERMINATE_CURRENT_SESSION = 16,  

        /// <summary>
        /// Communication session is over, confirm readiness to start a new session
        /// </summary>
        COMMUNIC_SESSION_ENDED__CONFIRM_READINESS_TO_OPEN_NEW_SESSION = 17,

        /// <summary>
        /// Accept rtp package
        /// </summary>
        ACCEPT_RTP_PACKAGE = 18,

        #region intrnalCommand
        /// <summary>
        /// Accept the authority to conduct further negotiations with the dispatch console to establish the tx-rx voice channel
        /// </summary>
        AGREE_WITH_DISP_CONSOLE_TO_ESTABL_TXRX_VOICE_CHANNEL = 19,   

        /// <summary>
        /// Accept the authority to conduct further negotiations with the dispatch console to establish the rx-tx voice channel
        /// </summary>
        AGREE_WITH_DISP_CONSOLE_TO_ESTABL_RXTX_VOICE_CHANNEL = 20   
        #endregion
    }
    //--------------------------
    /// <summary>
    /// the result of the readiness to receive an individual voice call
    /// </summary>
    public enum ReadinessAcceptVoiceCall
    {
        /// <summary>
        /// the client (dispatch console) is ready to receive an audio call
        /// </summary>
        READY_TO_RECEIVE_AUDIO_CALL = 1,

        /// <summary>
        /// the client is talking (the connection is busy)
        /// </summary>
        CLIENT_IS_TALKING = 2,

        /// <summary>
        /// error while establishing a network connection (no connection)
        /// </summary>
        NO_NETWORK_CONNECTION = 3,

        /// <summary>
        /// connection error [for command exchange], the channel name may be incorrectly specified
        /// </summary>
        CONNECT_ERROR_COMMAND = 4,

        /// <summary>
        /// connection error [for audio transmission], the channel name may be incorrectly set
        /// </summary>
        CONNECT_ERROR_VOICE = 5,

        /// <summary>
        /// data has just been updated
        /// </summary>
        DATA_UPDATED = 6,

        /// <summary>
        /// the update status is not confirmed, the check has not been carried out for a long time (it is necessary to update the information)
        /// </summary>
        UPDATE_STATUS_NOT_CONFIRMED = 7
    }
    //--------------------------
    public class Common
    {

    }
}
