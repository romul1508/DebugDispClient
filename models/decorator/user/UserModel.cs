using DebugOmgDispClient.Interfaces;
using System.Collections.Generic;

namespace DebugOmgDispClient.models.decorator.user
{
    /// <summary>
    /// User Dispatch Model
    /// </summary>
    public record DispatcherUserModel
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// User priority
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Connection ID
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// User's IP address
        /// </summary>
        public string IpAddr { get; set; }
        /// <summary>
        /// Last time user was online
        /// </summary>
        public string LastOnline { get; set; }
        /// <summary>
        /// List of user role names
        /// </summary>
        public ICollection<string> UserRoles { get; set; } = new List<string>();
        /// <summary>
        /// List of user group names
        /// </summary>
        public ICollection<string> UserGroups { get; set; } = new List<string>();
    }


    /// <summary>
    /// Custom model
    /// </summary>
    public record UserModel: IUser
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Connection ID
        /// </summary>
        public string ConnectionId { get; set; }
        /// <summary>
        /// User priority
        /// </summary>
        public int Priority { get; set; }
    }    
}
