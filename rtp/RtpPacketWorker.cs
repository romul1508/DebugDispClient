using System;


namespace DebugOmgDispClient.rtp
{
    /// <summary>
    /// RTP package, provides properties for working with header fields and Payload
    /// </summary>
    public class RtpPacketWorker
    {
        /// <summary>
        /// RTP packet buffer containing both the RTP header and payload
        /// </summary>
        private byte[] packet;

        /// <summary>
        /// RTP packet length
        /// </summary>
        private int packetLen;

        /// <summary>
        /// RTP - package
        /// </summary>
        public byte[] Packet
        {
            get { return packet; }

            set
            {
                byte[] temp_packet = value;
                Array.Copy(value, packet, temp_packet.Length);
            }
        }

        /// <summary>
        /// RTP packet length
        /// </summary>
        public int PacketLength
        {
            get { return packetLen; }            
        }

        /// <summary>
        /// RTP header length
        /// </summary>
        public int HeaderLength
        {
            get
            {
                if (PacketLength >= 12)
                    return 12 + 4 * CscrCount;
                else
                    return PacketLength; // broken packet
            }
        }

        /// <summary>
        /// Payload RTP packet length
        /// </summary>
        public int PayloadLength
        {
            get
            {
                if (PacketLength >= 12)
                    return PacketLength - HeaderLength;
                else
                    return 0; // broken packet
            }

            set
            {
                packetLen = HeaderLength + value;
            }
        }     

        /// <summary>
        /// Gets the CSCR count (CC)
        /// </summary>
        public int CscrCount
        {
            get
            {
                if (packetLen >= 12)
                    return (packet[0] & 0x0F);
                else
                    return 0; // broken packet
            }
        }  
        

        /// <summary>
        /// the timestamp
        /// </summary>
        public long Timestamp
        {
            get
            { 
                if (packetLen >= 12)
                    return getLong(packet, 4, 8);
                else
                    return 0; // broken packet
            }

            set
            {
                if (packetLen >= 12)
                    setLong(value, packet, 4, 8);
            }
        }


        /// <summary>
        /// the SSCR
        /// </summary>        
        public long Sscr
        {
            get
            {
                if (packetLen >= 12)
                    return getLong(packet, 8, 12);
                else
                    return 0; // broken packet
            }

            set
            {
                if (packetLen >= 12)
                    setLong(value, packet, 8, 12);
            }
        }


        /// <summary>
        /// Get the CSCR list
        /// </summary>
        public long[] getCscrList()
        {
            int cc = CscrCount;
            long[] cscr = new long[cc];
            for (int i = 0; i < cc; i++)
                cscr[i] = getLong(packet, 12 + 4 * i, 16 + 4 * i);
            return cscr;
        }

        
        /// <summary>
        /// Sets the CSCR list
        /// </summary>
        public void setCscrList(long[] cscr)
        {
            if (packetLen >= 12)
            {
                int cc = cscr.Length;
                if (cc > 15)
                    cc = 15;
                packet[0] = (byte)(((packet[0] >> 4) << 4) + cc);
                cscr = new long[cc];
                for (int i = 0; i < cc; i++)
                    setLong(cscr[i], packet, 12 + 4 * i, 16 + 4 * i);
                // header_len=12+4*cc;
            }
        }

        
        /// <summary>
        /// Sets the payload
        /// </summary>
        public void setPayload(byte[] payload, int len)
        {
            if (packetLen >= 12)
            {
                int header_len = HeaderLength;
                Array.Copy(payload, 0, packet, header_len, len);
                packetLen = header_len + len;
            }
        }

        /// <summary>
        /// Gets the payload
        /// </summary>
        public byte[] getPayload()
        {
            int header_len = HeaderLength;
            int len = packetLen - header_len;
            byte[] payload = new byte[len];
            Array.Copy(packet, header_len, payload, 0, len);
            return payload;
        }


        /// <summary>
        /// Creates a new RTP packet
        /// </summary>
        public RtpPacketWorker(byte[] buffer, int packet_length)
        {
            packet = buffer;
            packetLen = packet_length;
            if (packetLen < 12)
                packetLen = 12;            
        }

        /// <summary>
        /// Clone this packet to another
        /// </summary>        
        public void clone(RtpPacketWorker rtpPacket)
        {
            Array.Copy(packet, 0, rtpPacket.packet, 0, packetLen);
            rtpPacket.packetLen = packetLen;
        }


        /// <summary>
        /// init the RTP packet header (TimeStamp, SSCR)
        /// </summary> 
        public void init(long timestamp, long sscr)
        {
            // SequenceNumber = seqn;
            Timestamp = timestamp;
            Sscr = sscr;
        }

        //-------------------------------------

        #region Private and Static

              
        /// <summary>
        ///  Gets long value
        /// </summary> 
        private static long getLong(byte[] data, int begin, int end)
        {
            long n = 0;
            for (; begin < end; begin++)
            {
                n <<= 8;
                n += data[begin] & 0xFF;
            }
            return n;
        }

        /// <summary>
        ///  Sets long value
        /// </summary> 
        private static void setLong(long n, byte[] data, int begin, int end)
        {
            for (end--; end >= begin; end--)
            {
                data[end] = (byte)(n % 256);
                n >>= 8;
            }
        }   

        #endregion
    }
}
