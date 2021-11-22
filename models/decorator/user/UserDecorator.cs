using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using System.Threading;


namespace DebugOmgDispClient.models.decorator.user
{
    /// <summary>
    /// An abstract decorator class that implements the Interfaces.IUser interface
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 28.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public abstract class UserDecorator : IUser
    {
        protected IUser user;

        public static SimpleMultithreadSingLogger logger = SimpleMultithreadSingLogger.Instance;

        public void SetUser(IUser user)
        {
            this.user = user;
        }        

        public int Id
        {
            get
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (user != null)
                    return user.Id;
                else
                {
                    logger.Write($"abstract class UserDecorator: property Id: threadId = {threadId}: no user data");
                    return -1;
                }
            }
            set
            {
                user.Id = value;
            }
        }


        public string UserName
        {
            get
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                if (user != null)
                    return user.UserName;
                else
                {
                    logger.Write($"abstract class UserDecorator: property UserName: threadId = {threadId}: no user data");
                    return "no_information";
                }
            }

            set
            {
                user.UserName = value;
            }
        }


        public string ConnectionId
        {
            get
            {
                if (user != null)
                    return user.ConnectionId;
                else
                {
                    logger.Write($"abstract class UserDecorator: property ConnectionId: no user data");
                    return "not set";
                }
            }
            set
            {
                user.ConnectionId = value;
            }
        }

        public int Priority
        {
            get
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;

                if (user != null)
                    return user.Priority;
                else
                {
                    logger.Write($"abstract class UserDecorator: property Priority: threadId = {threadId}: no user data");
                    return -1;
                }
            }
            set
            {
                user.Priority = value;
            }
        }
        
    }
}
