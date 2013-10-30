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
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace WirelessAudioServer
{
    /// <summary>
    ///     ProtocolTypes
    /// </summary>
    public enum ProtocolTypes
    {
        LH
    }

    /// <summary>
    ///     Protocol
    /// </summary>
    public class Protocol
    {
        public delegate void DelegateDataComplete(Object sender, Byte[] data);

        public delegate void DelegateExceptionAppeared(Object sender, Exception ex);

        private const int m_MaxBufferLength = 10000;
        private readonly List<Byte> m_DataBuffer = new List<byte>();
        private Encoding m_Encoding = Encoding.Default;
        public Object m_LockerReceive = new object();
        private ProtocolTypes m_ProtocolType = ProtocolTypes.LH;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        /// <param name="type"></param>
        public Protocol(ProtocolTypes type, Encoding encoding)
        {
            m_ProtocolType = type;
            m_Encoding = encoding;
        }

        //Delegates bzw. Events

        public event DelegateDataComplete DataComplete;
        public event DelegateExceptionAppeared ExceptionAppeared;


        /// <summary>
        ///     ToBytes
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Byte[] ToBytes(Byte[] data)
        {
            try
            {
                //Bytes Länge
                var bytesLength = BitConverter.GetBytes(data.Length);

                //Alles zusammenfassen
                var allBytes = new Byte[bytesLength.Length + data.Length];
                Array.Copy(bytesLength, allBytes, bytesLength.Length);
                Array.Copy(data, 0, allBytes, bytesLength.Length, data.Length);

                //Fertig
                return allBytes;
            }
            catch (Exception ex)
            {
                ExceptionAppeared(null, ex);
            }

            //Fehler
            return data;
        }

        /// <summary>
        ///     Receive_LH_STX_ETX
        /// </summary>
        /// <param name="data"></param>
        public void Receive_LH(Object sender, Byte[] data)
        {
            lock (m_LockerReceive)
            {
                try
                {
                    //Daten an Puffer anhängen
                    m_DataBuffer.AddRange(data);

                    //Pufferüberlauf verhindern
                    if (m_DataBuffer.Count > m_MaxBufferLength)
                    {
                        m_DataBuffer.Clear();
                    }

                    //Bytes auslesen
                    var bytes = m_DataBuffer.Take(4).ToArray();
                    //Länge ermitteln
                    var length = BitConverter.ToInt32(bytes.ToArray(), 0);

                    //Maximale Länge sicherstellen
                    if (length > m_MaxBufferLength)
                    {
                        m_DataBuffer.Clear();
                    }

                    //So lange wie Daten vorhanden sind
                    while (m_DataBuffer.Count >= length + 4)
                    {
                        //Daten extrahieren
                        var message = m_DataBuffer.Skip(4).Take(length).ToArray();

                        //Benachrichtigung über vollständige Daten
                        if (DataComplete != null)
                        {
                            DataComplete(sender, message);
                        }
                        //Daten aus Puffer entfernen
                        m_DataBuffer.RemoveRange(0, length + 4);

                        //Wenn weitere Daten vorhanden
                        if (m_DataBuffer.Count > 4)
                        {
                            //Neue Länge berechnen
                            bytes = m_DataBuffer.Take(4).ToArray();
                            length = BitConverter.ToInt32(bytes.ToArray(), 0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Puffer leeren
                    m_DataBuffer.Clear();
                    ExceptionAppeared(null, ex);
                }
            }
        }
    }
}
