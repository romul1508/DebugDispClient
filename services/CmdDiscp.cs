namespace DebugOmgDispClient.services
{
    /// <summary>
    /// Contains information about the messages of the commands to be executed
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>

    public class CmdDiscp
    {
        private int cmd_num = -1;       //   the ordinal number of the command, the values of the class fields have not yet been
        private int type_cmd = -1;      //   identifier (type) of the command
        private string cmd_msg = "";    //   team name
        private int priority = -1;      //   command priority

        CmdDiscp(int cmd_num, int type_cmd, string cmd_msg, int priority)
        {
            this.cmd_num = cmd_num;
            this.type_cmd = type_cmd;
            this.cmd_msg = cmd_msg;
            this.priority = priority;
        }

        public int CmdNum { get { return cmd_num; } set { cmd_num = value; } }

        public int Type_Cmd { get { return type_cmd; } set { type_cmd = value; } }

        public string CmdMsg { get { return cmd_msg; } set { cmd_msg = value; } }

        public int Priority { get { return priority; } set { priority = value; } }
    }

}
