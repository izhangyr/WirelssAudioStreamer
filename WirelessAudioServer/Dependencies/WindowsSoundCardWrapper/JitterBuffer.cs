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

#endregion

namespace WirelessAudioServer
{
    /// <summary>
    ///     JitterBuffer
    /// </summary>
    public class JitterBuffer
    {
        public delegate void DelegateDataAvailable(Object sender, RTPPacket packet);

        private readonly Queue<RTPPacket> m_Buffer = new Queue<RTPPacket>();

        private readonly uint m_MaxRTPPackets = 10;
        private readonly Object m_Sender;
        private readonly EventTimer m_Timer = new EventTimer();
        private readonly uint m_TimerIntervalInMilliseconds = 20;
        private RTPPacket m_LastRTPPacket = new RTPPacket();
        private bool m_Overflow;
        private bool m_Underflow = true;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        public JitterBuffer(Object sender, uint maxRTPPackets, uint timerIntervalInMilliseconds)
        {
            //Mindestanzahl einhalten
            if (maxRTPPackets < 2)
            {
                throw new Exception("Wrong Arguments. Minimum maxRTPPackets is 2");
            }

            m_Sender = sender;
            m_MaxRTPPackets = maxRTPPackets;
            m_TimerIntervalInMilliseconds = timerIntervalInMilliseconds;

            Init();
        }

        //Delegates bzw. Event

        /// <summary>
        ///     Anzahl Packete im Buffer
        /// </summary>
        public int Length
        {
            get { return m_Buffer.Count; }
        }

        /// <summary>
        ///     Maximale Anzahl an RTP Packeten
        /// </summary>
        public uint Maximum
        {
            get { return m_MaxRTPPackets; }
        }

        /// <summary>
        ///     IntervalInMilliseconds
        /// </summary>
        public uint IntervalInMilliseconds
        {
            get { return m_TimerIntervalInMilliseconds; }
        }

        public event DelegateDataAvailable DataAvailable;

        /// <summary>
        ///     Init
        /// </summary>
        private void Init()
        {
            InitTimer();
        }

        /// <summary>
        ///     InitTimer
        /// </summary>
        private void InitTimer()
        {
            m_Timer.TimerTick += OnTimerTick;
        }

        /// <summary>
        ///     Start
        /// </summary>
        public void Start()
        {
            m_Timer.Start(m_TimerIntervalInMilliseconds, 0);
            m_Underflow = true;
        }

        /// <summary>
        ///     Stop
        /// </summary>
        public void Stop()
        {
            m_Timer.Stop();
            m_Buffer.Clear();
        }

        /// <summary>
        ///     OnTimerTick
        /// </summary>
        private void OnTimerTick()
        {
            try
            {
                if (DataAvailable != null)
                {
                    //Wenn Daten vorhanden
                    if (m_Buffer.Count > 0)
                    {
                        //Wenn Überlauf
                        if (m_Overflow)
                        {
                            //Warten bis Buffer halb - leer ist
                            if (m_Buffer.Count <= m_MaxRTPPackets/2)
                            {
                                m_Overflow = false;
                            }
                        }

                        //Wenn Underflow
                        if (m_Underflow)
                        {
                            //Warten bis Buffer halb - voll ist
                            if (m_Buffer.Count < m_MaxRTPPackets/2)
                            {
                                return;
                            }
                            m_Underflow = false;
                        }

                        //Daten schicken
                        m_LastRTPPacket = m_Buffer.Dequeue();
                        DataAvailable(m_Sender, m_LastRTPPacket);
                    }
                    else
                    {
                        //Kein Overflow
                        m_Overflow = false;

                        //Wenn Buffer leer
                        if (m_LastRTPPacket != null && m_Underflow == false)
                        {
                            if (m_LastRTPPacket.Data != null)
                            {
                                //Underflow vorhanden
                                m_Underflow = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("JitterBuffer.cs | OnTimerTick() | {0}", ex.Message);
            }
        }

        /// <summary>
        ///     AddData
        /// </summary>
        /// <param name="data"></param>
        public void AddData(RTPPacket packet)
        {
            try
            {
                //Wenn kein Überlauf
                if (m_Overflow == false)
                {
                    //Maximalgrösse beachten
                    if (m_Buffer.Count <= m_MaxRTPPackets)
                    {
                        m_Buffer.Enqueue(packet);
                        //m_Buffer.OrderBy(x => x.SequenceNumber);
                    }
                    else
                    {
                        //Bufferüberlauf
                        m_Overflow = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("JitterBuffer.cs | AddData() | {0}", ex.Message);
            }
        }
    }
}
