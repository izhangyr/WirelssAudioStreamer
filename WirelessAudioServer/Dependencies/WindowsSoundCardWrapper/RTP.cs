/*
    Copyright 2013 Roman Fortunatov

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

#region Usings

using System;

#endregion

namespace WirelessAudioServer
{
    /// <summary>
    ///     RTPPacket
    /// </summary>
    public class RTPPacket
    {
        //Attribute
        public static int MinHeaderLength = 12;
        public int CSRCCount = 0;
        public Byte[] Data;
        public bool Extension = false;
        public int HeaderLength = MinHeaderLength;
        public bool Marker = false;
        public bool Padding = false;
        public int PayloadType = 0;
        public UInt16 SequenceNumber = 0;
        public uint SourceId = 0;
        public uint Timestamp = 0;
        public int Version = 0;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        public RTPPacket()
        {
        }

        /// <summary>
        ///     Konstuktor
        /// </summary>
        /// <param name="_data"></param>
        public RTPPacket(byte[] data)
        {
            Parse(data);
        }

        /// <summary>
        ///     Parse
        /// </summary>
        /// <param name="linearData"></param>
        private void Parse(Byte[] data)
        {
            if (data.Length >= MinHeaderLength)
            {
                Version = ValueFromByte(data[0], 6, 2);
                Padding = Convert.ToBoolean(ValueFromByte(data[0], 5, 1));
                Extension = Convert.ToBoolean(ValueFromByte(data[0], 4, 1));
                CSRCCount = ValueFromByte(data[0], 0, 4);
                Marker = Convert.ToBoolean(ValueFromByte(data[1], 7, 1));
                PayloadType = ValueFromByte(data[1], 0, 7);
                HeaderLength = MinHeaderLength + (CSRCCount*4);

                //Sequence Nummer
                var seqNum = new Byte[2];
                seqNum[0] = data[3];
                seqNum[1] = data[2];
                SequenceNumber = BitConverter.ToUInt16(seqNum, 0);

                //TimeStamp
                var timeStmp = new Byte[4];
                timeStmp[0] = data[7];
                timeStmp[1] = data[6];
                timeStmp[2] = data[5];
                timeStmp[3] = data[4];
                Timestamp = BitConverter.ToUInt32(timeStmp, 0);

                //SourceId
                var srcId = new Byte[4];
                srcId[0] = data[8];
                srcId[1] = data[9];
                srcId[2] = data[10];
                srcId[3] = data[11];
                SourceId = BitConverter.ToUInt32(srcId, 0);

                //Daten kopieren
                Data = new Byte[data.Length - HeaderLength];
                Array.Copy(data, HeaderLength, Data, 0, data.Length - HeaderLength);
            }
        }

        /// <summary>
        ///     GetValueFromByte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startPos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private Int32 ValueFromByte(Byte value, int startPos, int length)
        {
            Byte mask = 0;
            //Maske erstellen
            for (var i = 0; i < length; i++)
            {
                mask = (Byte) (mask | 0x1 << startPos + i);
            }

            //Ergebnis
            var result = (Byte) ((value & mask) >> startPos);
            //Fertig
            return Convert.ToInt32(result);
        }

        /// <summary>
        ///     ToBytes
        /// </summary>
        /// <returns></returns>
        public Byte[] ToBytes()
        {
            //Ergebnis
            var bytes = new Byte[HeaderLength + Data.Length];

            //Byte 0
            bytes[0] = (Byte) (Version << 6);
            bytes[0] |= (Byte) (Convert.ToInt32(Padding) << 5);
            bytes[0] |= (Byte) (Convert.ToInt32(Extension) << 4);
            bytes[0] |= (Byte) (Convert.ToInt32(CSRCCount));

            //Byte 1
            bytes[1] = (Byte) (Convert.ToInt32(Marker) << 7);
            bytes[1] |= (Byte) (Convert.ToInt32(PayloadType));

            //Byte 2 + 3
            var bytesSequenceNumber = BitConverter.GetBytes(SequenceNumber);
            bytes[2] = bytesSequenceNumber[1];
            bytes[3] = bytesSequenceNumber[0];

            //Byte 4 bis 7
            var bytesTimeStamp = BitConverter.GetBytes(Timestamp);
            bytes[4] = bytesTimeStamp[3];
            bytes[5] = bytesTimeStamp[2];
            bytes[6] = bytesTimeStamp[1];
            bytes[7] = bytesTimeStamp[0];

            //Byte 8 bis 11
            var bytesSourceId = BitConverter.GetBytes(SourceId);
            bytes[8] = bytesSourceId[3];
            bytes[9] = bytesSourceId[2];
            bytes[10] = bytesSourceId[1];
            bytes[11] = bytesSourceId[0];

            //Daten
            Array.Copy(Data, 0, bytes, HeaderLength, Data.Length);

            //Fertig
            return bytes;
        }
    }
}
