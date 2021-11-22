using System;


namespace DebugOmgDispClient.Interfaces
{
    /// <summary>
    /// The interface describes the command parameter
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public interface IParamTask
    {
        /// <summary>
        /// Parameter identifier, corresponds to the NameParam property (may not be used)
        /// </summary>
        public int IdParam { get; }

        /// <summary>
        /// parameter name
        /// </summary>
        string NameParam { get; }

        /// <summary>
        /// parameter type
        /// </summary>
        Type TypeParam { get; }         

        /// <summary>
        /// type identifier (BYTE, SHOT, WCHAR, INT, etc.)
        /// </summary>
        int IdType { get; }

        /// <summary>
        /// returns parameter value (for standard types) 
        /// </summary>
        object Value { get; }

        /// <summary>
        /// returns the current object parameter if the parameter is an object
        /// </summary>
        IObject ObjectParameter { get; }

        /// <summary>
        /// Parameter brief description
        /// </summary>
        string ParameterDescription { get; }
    }
}
