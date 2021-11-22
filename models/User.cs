using DebugOmgDispClient.Interfaces;

namespace DebugOmgDispClient.models
{
    /// <summary>
    /// User model
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 21.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class User: IUser
    {        
        private int id = -1;
        private string userName = "";
        private string connectionId = "";
        private int priority = -1;


        public User(int id)
        {
            this.id = id;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userName">Username</param>
        /// <param name="connectionId">Connection ID</param>
        /// <param name="priority">User priority</param>
        public User(int id, string userName, string connectionId, int priority)
        {
            this.id = id;
            this.UserName = userName;
            this.connectionId = connectionId;
            this.priority = priority;
        }
        
        public int Id 
        {
            get { return id; }

            set { id = value; } 
        }

        public string UserName 
        {
            get { return userName; }

            set { userName = value; } 
        }


        public string ConnectionId
        {
            get { return connectionId; }

            set { connectionId = value; }
        }

        public int Priority
        {
            get { return priority; }

            set { priority = value; }
        }
    }
}
