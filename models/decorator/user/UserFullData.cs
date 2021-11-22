using DebugOmgDispClient.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace DebugOmgDispClient.models.decorator.user
{
    /// <summary>
    /// Has the required number of properties to ensure a full-fledged information exchange
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 28.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class UserFullData: UserDecorator
    {
        protected string ipAddr = "";                                        // Subscriber ip-address
        protected string lastOnline = "";                                    // Last time user was online  

        protected ICollection<string> _userRoles = new List<string>();       // List of user role names

        protected ICollection<string> _userGroups = new List<string>();      // List of user groups

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="id_user">User ID</param>
        public UserFullData(int id_user)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "UserFullData class, Constructor(id_user): ";

            user = new User(id_user);
            Id = id_user;

            
            logger.Write($"\n { Tag }: threadId = {threadId}:  Id = { Id }");
        }

        /// <summary>
        /// Parameterized constructor (saves Users data)
        /// </summary>
        /// <param name="id_user">User ID</param>
        /// <param name="user_name">User name</param>
        /// <param name="сonnectionId">Connection ID</param>
        /// <param name="user_priority">User priority</param>
        public UserFullData(int id_user, string user_name, string сonnection_id, int user_priority)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "UserFullData class, Constructor(id_user, user_name, сonnection_id, user_priority): ";

            user = new User(id_user);
            Id = id_user;

            UserName = user_name;

            ConnectionId = сonnection_id;

            Priority = user_priority;

            logger.Write($"\n { Tag }: threadId = {threadId}:  user.Id = { user.Id }, user.UserName = {user.UserName}, user.ConnectionId = {user.ConnectionId}, user.Priority = {user.Priority}");
        }


        /// <summary>
        /// Constructor with five parameters
        /// </summary>
        /// <param name="id_user">User ID</param>
        /// <param name="user_name">User name</param>
        /// <param name="сonnectionId">Connection ID</param>
        /// <param name="user_priority">User priority</param>
        /// <param name="ipAddr">User ip address</param>
        public UserFullData(int id_user, string user_name, string сonnectionId, int user_priority, string ipAddr)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "UserFullData class, Constructor(id_user, user_name, сonnection_id, user_priority, ipAddr): ";

            user = new User(id_user);
            Id = id_user;

            UserName = user_name;

            ConnectionId = сonnectionId;

            Priority = user_priority;
            
            this.ipAddr = ipAddr;

            logger.Write($"\n { Tag }: threadId = {threadId}:  user.Id = { user.Id }, user.UserName = {user.UserName}, user.ConnectionId = {user.ConnectionId}, user.Priority = {user.Priority}, ipAddr = {ipAddr}");
        }

        /// <summary>
        /// Constructor with six parameters
        /// </summary>
        /// <param name="id_user">User ID</param>
        /// <param name="user_name">User name</param>
        /// <param name="сonnectionId">Connection ID</param>
        /// <param name="user_priority">User priority</param>
        /// <param name="ipAddr">User ip address</param>
        /// <param name="lastOnline">Last time user was online</param>
        public UserFullData(int id_user, string user_name, string сonnectionId, int user_priority, string ipAddr, string lastOnline)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "UserFullData class, Constructor(id_user, user_name, сonnection_id, user_priority, ipAddr, lastOnline): ";


            // this.user.Id = id_user;
            user = new User(id_user);

            UserName = user_name;

            ConnectionId = сonnectionId;

            Priority = user_priority;

            this.ipAddr = ipAddr;

            this.lastOnline = lastOnline;

            logger.Write($"\n { Tag }: threadId = {threadId}:  user.Id = { user.Id }, user.UserName = {user.UserName}, user.ConnectionId = {user.ConnectionId}, user.Priority = {user.Priority}, ipAddr = {ipAddr}, lastOnline = {lastOnline}");
        }


        /// <summary>
        /// The most complete constructor, allows you to set values for all fields of the class
        /// </summary>
        /// <param name="id_user">User ID</param>
        /// <param name="user_name">User name</param>
        /// <param name="сonnectionId">Connection ID</param>
        /// <param name="user_priority">User priority</param>
        /// <param name="ipAddr">User ip address</param>
        /// <param name="lastOnline">Last time user was online</param>
        /// <param name="userRoles">List of user role names</param>
        /// <param name="UserGroups">List of user group names</param>
        public UserFullData(int id_user, string user_name, string сonnectionId, int user_priority, string ipAddr, string lastOnline, ICollection<string> userRoles, ICollection<string> userGroups)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "UserFullData class, Constructor(id_user, user_name, сonnection_id, user_priority, ipAddr, lastOnline, userRoles, userGroups): ";


            // this.user.Id = id_user;
            user = new User(id_user);

            user.UserName = user_name;

            ConnectionId = сonnectionId;

            user.Priority = user_priority;

            this.ipAddr = ipAddr;

            this.lastOnline = lastOnline;

            foreach(string userRole in userRoles) 
            {
                string temp = new string(userRole);
                _userRoles.Add(temp);
            }

            foreach (string userGroup in userGroups)
            {
                string temp = new string(userGroup);
                _userGroups.Add(temp);
            }


            logger.Write($"\n { Tag }: threadId = {threadId}:  user.Id = { user.Id }, user.UserName = {user.UserName}, user.ConnectionId = {user.ConnectionId}, user.Priority = {user.Priority}, ipAddr = {ipAddr}, lastOnline = {lastOnline}");

        }


        /// <summary>
        /// User's IP address
        /// </summary>
        public string IpAddr
        {
            get
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "UserFullData class, IpAddr get property: ";

               logger.Write($"\n { Tag }: threadId = {threadId}:  return IpAddr = { ipAddr }");

                return ipAddr;
            }

            set
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "UserFullData class, IpAddr set property: ";

                ipAddr = value;

                logger.Write($"\n { Tag }: threadId = {threadId}:  ipAddr = { ipAddr }");

            }
        }


        /// <summary>
        /// Last time user was online
        /// </summary>
        public string LastOnline
        {
            get
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "UserFullData class, LastOnline get property: ";

               logger.Write($"\n { Tag }: threadId = {threadId}:  return lastOnline = { lastOnline }");

                return ipAddr;
            }

            set
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                string Tag = "UserFullData class, LastOnline set property: ";

                lastOnline = value;

                logger.Write($"\n { Tag }: threadId = {threadId}:  lastOnline = { lastOnline }");

            }
        }


        /// <summary>
        /// User in the format used on the server
        /// </summary>
        public IUser USER
        {
            get 
            {
                //int threadId = Thread.CurrentThread.ManagedThreadId;

                //string Tag = "UserFullData class, USER get property: ";

                //logger.Write($"\n { Tag }: threadId = {threadId}:  return user .");
                
                return user; 
            }
            
        }


        public ICollection<string> UserRoles
        {
            get
            {
                //int threadId = Thread.CurrentThread.ManagedThreadId;

                //string Tag = "UserFullData class, UserRoles get property: ";

                //logger.Write($"\n { Tag }: threadId = {threadId}:  return _userRoles .");

                return _userRoles;
            }
        }

        public ICollection<string> UserGroups
        {
            get
            {
                //int threadId = Thread.CurrentThread.ManagedThreadId;

                //string Tag = "UserFullData class, UserGroups get property: ";

                //logger.Write($"\n { Tag }: threadId = {threadId}:  return _userGroups .");

                return _userGroups;
            }
        }
    }
}
