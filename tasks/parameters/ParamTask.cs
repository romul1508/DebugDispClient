using DebugOmgDispClient.Interfaces;
using System;

namespace DebugOmgDispClient.tasks.parameters
{
    /// <summary>
    /// The class implements the parameters of the transmitted commands in the system,
    /// implements the IParamTask interface
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class ParamTask : IParamTask
    {
        private int id_param = 0;                   // not set (default), unique numeric (integer) values corresponding to the string field (property) nameParam
        private string nameParam = "";              // parameter name, unique name
        private Type typeParam;                     // parameter type
        private int idType = -1;                    // type identifier
        private object valueParam;                  // parameter value (required field)
        private IObject paramObj;                   // object parameter
        private string parameterDescription = "";   // short description of the parameter

        public ParamTask(string nameParam, Type typeParam, object valueParam, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.typeParam = typeParam;
            this.valueParam = valueParam;
            this.parameterDescription = parameterDescription;
        }

        public ParamTask(int id_param, string nameParam, Type typeParam, object valueParam, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.typeParam = typeParam;
            this.valueParam = valueParam;
            this.parameterDescription = parameterDescription;
            this.id_param = id_param;
        }

        public ParamTask(string nameParam, int idType, object valueParam, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.idType = idType;
            this.valueParam = valueParam;
            this.parameterDescription = parameterDescription;
        }


        public ParamTask(int id_param, string nameParam, int idType, object valueParam, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.idType = idType;
            this.valueParam = valueParam;
            this.parameterDescription = parameterDescription;
            this.id_param = id_param;
        }


        public ParamTask(string nameParam, Type typeParam, IObject paramObj, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.typeParam = typeParam;
            this.paramObj = paramObj;
            this.parameterDescription = parameterDescription;
        }

        public ParamTask(int id_param, string nameParam, Type typeParam, IObject paramObj, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.typeParam = typeParam;
            this.paramObj = paramObj;
            this.parameterDescription = parameterDescription;
            this.id_param = id_param;
        }

        public ParamTask(string nameParam, int idType, IObject paramObj, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.idType = idType;
            this.paramObj = paramObj;
            this.parameterDescription = parameterDescription;
        }

        public ParamTask(int id_param, string nameParam, int idType, IObject paramObj, string parameterDescription)
        {
            this.nameParam = nameParam;
            this.idType = idType;
            this.paramObj = paramObj;
            this.parameterDescription = parameterDescription;
            this.id_param = id_param;
        }

        #region Properties

        /// <summary>
        /// not set (default), contains unique numeric (integer) values, 
        /// corresponding to the string field (property) NameParam
        /// </summary>
        public int IdParam
        {
            get { return id_param; }
        }


        /// <summary>
        /// parameter name (unique name)
        /// </summary>

        public string NameParam
        {
            get { return nameParam; }            
        }

        /// <summary>
        /// Parameter type
        /// </summary>

        public Type TypeParam
        {
            get { return typeParam; }            
        }

        /// <summary>
        /// Type identifier
        /// </summary>
        public int IdType   
        {
            get { return idType; }            
        }

        /// <summary>
        /// Parameter value (required field)
        /// </summary>
        public object Value 
        {
            get { return valueParam; }            
        }

        /// <summary>
        /// object parameter
        /// </summary>
        public IObject ObjectParameter
        {
            get { return paramObj; }            
        }

        /// <summary>
        /// short description of the command parameter
        /// </summary>
        public string ParameterDescription 
        {
            get { return parameterDescription; } 
        } 

        #endregion
    }
}
