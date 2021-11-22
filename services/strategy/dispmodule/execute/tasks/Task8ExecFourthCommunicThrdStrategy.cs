using DebugOmgDispClient.Interfaces;
using DebugOmgDispClient.pipe.stream;
using DebugOmgDispClient.rtp;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DebugOmgDispClient.services.strategy.dispmodule.execute.tasks
{
    /// <summary>
    /// Implements the START_SEND_VOICE_MSG task as the fourth worker thread
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 24.10.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// </summary>
    public class Task8ExecFourthCommunicThrdStrategy : TaskExecCommunicThrdStrategy
    {
        private bool startAudioCall = false;        // true - the audio stream is sent to the Qt client
        private bool clientSideError = false;       // client side error

        public Task8ExecFourthCommunicThrdStrategy(NamedPipeServerStream pipeServer)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task8ExecFourthCommunicThrdStrategy class, Constructor: ";

            logger.Write($" {Tag}; threadId = {threadId}; state: Started...\n");

            //---------------------------------
            this.pipeServer = pipeServer;
            
        }

        /// <summary>
        /// Ensures the execution of task # 8 (transfer of RTP packets to the Dispatcher Console)
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public override int TaskExecute(ITask task)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task8ExecFourthCommunicThrdStrategy class, TaskExecute method: ";

            logger.Write($"{Tag}; threadId = {threadId}; state: Started...\n");


            lock (sync)
            {
                int resultTask = 1;

                string strIpAdd = GlobalConstants.IpRTPServer;

                int serverUDPPort = GlobalConstants.ServerUDPPort;

                int localUDPPort = GlobalConstants.LocalUDPPort;


                UdpClient receiver = null;
                IPEndPoint remoteIp = null;
                                
                //-----------------------
                try
                {
                    logger.Write($"\n { Tag }: threadId = {threadId}:  localUDPPort = { localUDPPort }");
                    
                    receiver = new UdpClient(localUDPPort);

                    int whileStartAudioCall = 0;

                    logger.Write($"\n { Tag }: threadId = {threadId}:  whileStartAudioCall = { whileStartAudioCall }");

                    startAudioCall = true;

                    // int whileStartAudio = 0;
                    // myLogger.Write($"\n {Tag}: whileStartAudio = { whileStartAudio } .");

                    while (startAudioCall)
                    {
                        byte[] rtp_packet = receiver.Receive(ref remoteIp);

                        logger.Write($"\n { Tag }: threadId = {threadId}:  Rtp packege received!!");
                        //-------------------
                        if ( AudioDataTransfer(rtp_packet) != 1 )
                        {
                            logger.Write($"\n { Tag }:  threadId = {threadId}: Error (AudioDataTransfer)");
                            startAudioCall = false;
                            return -1;
                        }
                        else
                            logger.Write($"\n { Tag }: threadId = {threadId}:  Rtp packege sended to Dispetcher Console!!");
                        //-------------------
                        whileStartAudioCall++;
                        logger.Write($"\n { Tag }: threadId = {threadId}:  whileStartAudioCall = { whileStartAudioCall }");
                    }
                }
                catch (Exception e)
                {
                    resultTask = -1;
                    startAudioCall = false;
                    logger.Write($"\n { Tag }: threadId = {threadId}: Error (UdpClient): Exception e = { e.ToString() }");
                }

                logger.Write($"\n { Tag }: threadId = {threadId}:  resultTask = { resultTask }");

                return resultTask;
            }
        }
        //-------------------------
        /// <summary>
        /// Provides the transfer of audio data to the Qt client (to the dispatch console)
        /// </summary>
        /// <param name="rtp_packet">данные RTP-пакета</param>
        /// <returns></returns>
        private int AudioDataTransfer(byte[] rtp_packet)
        {
            int res = 1;

            int threadId = Thread.CurrentThread.ManagedThreadId;

            string Tag = "Task8ExecFourthCommunicThrdStrategy class, AudioDataTransfer method: ";

            logger.Write($"{Tag}; threadId = {threadId}; state: Started...\n");

            UnicodeEncoding streamEncoding = new UnicodeEncoding();
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            
            RtpPacketWorker packet = new RtpPacketWorker(rtp_packet, rtp_packet.Length);

            logger.Write($"\n {Tag}: threadId = {threadId}: rtp packet created successfully!");

            try
            {
                IStreamString streamString = new StreamString(pipeServer);

                logger.Write($"\n {Tag}: threadId = {threadId}: StreamString streamString created successfully!");

               
                StringBuilder strBuildCmd18 = new StringBuilder("id_cmd=ACCEPT_RTP_PACKAGE;");

                int packetLen = rtp_packet.Length;
                int payloadLen = packet.PayloadLength;

                strBuildCmd18.Append(payloadLen);

                strBuildCmd18.Append(";");  // delimiter

                //---------------------------
                byte[] outBufferStrCmd18UTF16 = streamEncoding.GetBytes(strBuildCmd18.ToString() );
                //---------------------------

                for (int i = 0; i < outBufferStrCmd18UTF16.Length; i++)
                    logger.Write($"\n {Tag}: threadId = {threadId}: value outBuffer [{i}] = {outBufferStrCmd18UTF16[i]} .");

                // int lenOutBufferStrCmd18UTF16 = outBufferStrCmd18UTF16.Length;

                byte[] utf8OutBytes = Encoding.Convert(Encoding.GetEncoding("UTF-16"), utf8, outBufferStrCmd18UTF16);   // здесь команда в UTF-8

                for (int i = 0; i < utf8OutBytes.Length; i++)
                    logger.Write($"\n {Tag}: threadId = {threadId}: AudioDataTransfer method: value utf8OutBytes [{i}] = {utf8OutBytes[i]} .");

                //----------------------------
                // int packetLen = rtp_packet.Length;           // RTP packet length
                logger.Write($"\n {Tag}: threadId = {threadId}: RTP packet length = {packetLen} .");

                int RTPHeaderLen = packet.HeaderLength;         // RTP header length
                logger.Write($"\n {Tag}: threadId = {threadId}: RTP header length = {RTPHeaderLen} .");

                // int payloadLen = packet.PayloadLength;           // Payload length
                logger.Write($"\n {Tag}: threadId = {threadId}: RTP payload length = {payloadLen} .");

                //---------------------------------------
                // add payload
                byte[] payload = packet.getPayload();   //utf8OutBytes

                int size_msg = utf8OutBytes.Length + payload.Length;

                byte[] buff_msg_cmd_18 = new byte[size_msg];

                Buffer.BlockCopy(utf8OutBytes, 0, buff_msg_cmd_18, 0, utf8OutBytes.Length);
                Buffer.BlockCopy(payload, 0, buff_msg_cmd_18, utf8OutBytes.Length, payloadLen);


                for (int i = 0; i < buff_msg_cmd_18.Length; i++)
                    logger.Write($"{Tag} : threadId = {threadId}, vaiue buff_msg_cmd_18 (finally): [{i}] = {buff_msg_cmd_18[i]}.");


                pipeServer.Write(buff_msg_cmd_18, 0, buff_msg_cmd_18.Length);

                //---------------
                int len = 5;
                //-----------------------------------
                string gotFromFileData = streamString.ReadString(len);

                logger.Write($"{Tag} : threadId = {threadId}, gotFromFileData = {gotFromFileData}.");
                
                //--------------------
                StringBuilder strRes = new StringBuilder();

                int count = 0;
                char simb = '\0';
                while (count < len)
                {
                    simb = gotFromFileData.ElementAt(count);

                    logger.Write($"\n {Tag}: threadId = {threadId}: count = {count}, simb = {simb}");

                    if (simb == ' ' || simb == '\0')
                        break;
                    else
                    {
                        strRes.Append(simb);
                        logger.Write($"\n {Tag}: threadId = {threadId}: simb added!");
                        count++;
                    }
                    
                }

                string result = strRes.ToString();
                logger.Write($"\n {Tag}: threadId = {threadId}: result = {result}");

                
                if (result == "ok")
                {
                    logger.Write($"{Tag} : threadId = {threadId}, res = 1 .");
                    res = 1;
                }
                else if (gotFromFileData == "error")
                {
                    logger.Write($"{Tag} : threadId = {threadId}, res = 0 .");
                    res = 0;
                }
                else
                {
                    logger.Write($"\n {Tag}, threadId = {threadId}: ERROR = incorrect data format [Exchange with the dispatch console] .");
                    res = 0;
                }                           

            }
            catch (Exception e)
            {
                res = -1;
                logger.Write($"\n {Tag}: threadId = {threadId}: Error:  Exception e = { e.ToString() }");
            }

            return res;
        }
        //-------------------------
        /// <summary>
        /// Audio call (and audio stream) status
        /// </summary>
        public bool StartAudioCall
        {
            get { return startAudioCall; }

            set { startAudioCall = value; }
        }
    }
}
