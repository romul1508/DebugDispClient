using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.logging.Internal;
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace DebugOmgDispClient.pipe.stream
{
    /// <summary>
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 23.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov 
    /// </summary>
    public class StreamString: IStreamString
    {
        // private Stream ioStream;
        private NamedPipeServerStream serverStream;

        private UnicodeEncoding streamEncoding;
        private UTF8Encoding streamUTF8;

        private int number_processed_bytes_pipe_file = 0;   // The number of processed bytes of the pipe file

        private readonly object sync = new object();     // for lock

        public SimpleMultithreadSingLogger logger;
        
        public StreamString(NamedPipeServerStream pipeServer)
        {
            //this.ioStream = ioStream;
            logger = SimpleMultithreadSingLogger.Instance;
            this.serverStream = pipeServer;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString(int msg_size)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "StreamString class, ReadString method: ";

            logger.Write($"{Tag}; threadId = {threadId}; state: Started...\n");


            lock (sync)
            {
                try
                {
                    int len_buff = 90;
                    
                    byte[] inBuffer = new byte[len_buff];
                    
                    logger.Write($"{Tag}: threadId = {threadId}: Let's read a block of bytes from the stream and write the data to the specified buffer.\n");

                   
                    serverStream.Read(inBuffer, 0, msg_size); // the string in UTF-8 encoding

                    number_processed_bytes_pipe_file += len_buff;
                    
                    for (int i = 0; i < msg_size; i++)
                           logger.Write($"{Tag}: threadId = {threadId}: value inBuffer [{ i }] = { inBuffer[i] }  .");

                    Encoding utf16 = Encoding.GetEncoding("UTF-16");

                    byte[] inBuffFinale = Encoding.Convert(
                        Encoding.GetEncoding("UTF-8"), utf16, inBuffer);


                    // return streamEncoding.GetString(inBuffer);
                    string str_res = utf16.GetString(inBuffFinale);

                    logger.Write($"{Tag}: threadId = {threadId}: value str_res = { str_res } .");

                    return str_res.Trim();
                }
                catch(Exception e)
                {
                    logger.Write($"{Tag}: threadId = {threadId}: An error has occurred: Exception e  = { e.ToString() } .");
                    return "";
                }
            }
        }

        /// <summary>
        /// Writes a command message (request) to a pipe stream
        /// </summary>
        /// <param name="outString"></param>
        /// <returns>returns the size of the final buffer in bytes</returns>
        public int WriteString(string outString)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "StreamString class, WriteString method: ";

            logger.Write($"{Tag}; threadId = {threadId}; state: Started...\n");

            lock (sync)
            {
                try
                {
                    byte[] outBuffer = streamEncoding.GetBytes(outString);

                    int len = outBuffer.Length;
                    if (len > UInt16.MaxValue)
                    {
                        len = (int)UInt16.MaxValue;
                    }

                    for (int i = 0; i < outBuffer.Length; i++)
                        logger.Write($"{Tag}: threadId = {threadId}: value outBuffer [{ i }] = { outBuffer[i] } .");

                    Encoding utf8 = Encoding.GetEncoding("UTF-8");

                    byte[] utf8OutBytes = Encoding.Convert(
                        Encoding.GetEncoding("UTF-16"), utf8, outBuffer);

                    for (int i = 0; i < utf8OutBytes.Length; i++)
                        logger.Write($"{Tag}: threadId = {threadId}: value utf8OutBytes [{ i }] = { utf8OutBytes[i] } .");

                    serverStream.Write(utf8OutBytes, 0, utf8OutBytes.Length);

                    number_processed_bytes_pipe_file += utf8OutBytes.Length;

                    logger.Write($"{Tag}, threadId = {threadId}, vaiue number_processed_bytes_pipe_file = { number_processed_bytes_pipe_file } .");


                    serverStream.Flush();    // Clears the buffer for the current stream and causes any buffered data to be written to the underling device
                                             

                    number_processed_bytes_pipe_file = 0;
                                        
                    logger.Write($"{Tag}, threadId = {threadId}, return utf8OutBytes.Length = { utf8OutBytes.Length } .");
                    return utf8OutBytes.Length;
                }
                catch (Exception e)
                {
                    // an error occurred while writing to the pipe
                    logger.Write($"{Tag}: threadId = {threadId}: An error occurred while writing to the pipe: Exception e  = { e.ToString() } .");
                    return -1;
                }
            }
        }
    }
}
