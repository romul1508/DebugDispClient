using System;

namespace DebugOmgDispClient.Interfaces
{
    /// <summary>
    /// Describes parameter objects and their properties
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// Object parameter identifier 
        /// (or its properties, including for nested objects (properties) )
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// unique custom name
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Object type
        /// </summary>
        public Type TypeObject { get; }

        /// <summary>
        /// allows you to update the object (make changes to properties) 
        /// and / or perform some operations (actions)
        /// </summary>
        public void Update();

        /// <summary>
        /// performs some action before the current object is deleted
        /// </summary>
        public void Delete();

        /// <summary>
        /// reference to the parent object (set for complex parameter objects 
        /// to make it easier to find the required object in the tree)
        /// </summary>
        public IObject Parent { get; set; }

    }
}
