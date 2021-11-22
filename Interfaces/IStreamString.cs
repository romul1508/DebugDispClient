namespace DebugOmgDispClient.Interfaces
{
    /// <summary>
    /// Defines the data protocol for reading and writing strings on our stream
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 25.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public interface IStreamString
    {
        /// <summary>
        /// reads a line written to the pipe
        /// </summary>
        /// <returns> returns the resulting string </returns>
        public string ReadString(int msg_size);


        /// <summary>
        /// writes a message command (request) to the pipe
        /// </summary>
        /// <param name="outString"></param>
        /// <returns></returns>
        public int WriteString(string outString);
    }
}
