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
using System.Runtime.InteropServices;

#endregion

namespace WirelessAudioServer
{
    /// <summary>
    ///     QueueTimer
    /// </summary>
    public class QueueTimer
    {
        //Attribute
        public delegate void DelegateTimerTick();

        private readonly Win32.DelegateTimerProc m_DelegateTimerProc;
        private GCHandle m_GCHandleTimer;
        private GCHandle m_GCHandleTimerQueue;
        private IntPtr m_HandleTimer = IntPtr.Zero;
        private IntPtr m_HandleTimerQueue;

        private bool m_IsRunning;
        private uint m_Milliseconds = 20;
        private uint m_ResolutionInMilliseconds;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        public QueueTimer()
        {
            m_DelegateTimerProc = OnTimer;
        }

        /// <summary>
        ///     IsRunning
        /// </summary>
        /// <returns></returns>
        public bool IsRunning
        {
            get { return m_IsRunning; }
        }

        /// <summary>
        ///     Milliseconds
        /// </summary>
        public uint Milliseconds
        {
            get { return m_Milliseconds; }
        }

        /// <summary>
        ///     ResolutionInMilliseconds
        /// </summary>
        public uint ResolutionInMilliseconds
        {
            get { return m_ResolutionInMilliseconds; }
        }

        public event DelegateTimerTick TimerTick;

        /// <summary>
        ///     SetBestResolution
        /// </summary>
        public static void SetBestResolution()
        {
            //QueueTimer Auflösung ermitteln
            var tc = new Win32.TimeCaps();
            Win32.TimeGetDevCaps(ref tc, (uint) Marshal.SizeOf(typeof (Win32.TimeCaps)));
            var resolution = Math.Max(tc.wPeriodMin, 0);

            //QueueTimer Resolution setzen
            Win32.TimeBeginPeriod(resolution);
        }

        /// <summary>
        ///     ResetResolution
        /// </summary>
        public static void ResetResolution()
        {
            //QueueTimer Auflösung ermitteln
            var tc = new Win32.TimeCaps();
            Win32.TimeGetDevCaps(ref tc, (uint) Marshal.SizeOf(typeof (Win32.TimeCaps)));
            var resolution = Math.Max(tc.wPeriodMin, 0);

            //QueueTimer Resolution deaktivieren
            Win32.TimeBeginPeriod(resolution);
        }

        /// <summary>
        ///     Start
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="dueTimeInMilliseconds"></param>
        public void Start(uint milliseconds, uint dueTimeInMilliseconds)
        {
            //Werte übernehmen
            m_Milliseconds = milliseconds;

            //QueueTimer Auflösung ermitteln
            var tc = new Win32.TimeCaps();
            Win32.TimeGetDevCaps(ref tc, (uint) Marshal.SizeOf(typeof (Win32.TimeCaps)));
            m_ResolutionInMilliseconds = Math.Max(tc.wPeriodMin, 0);

            //QueueTimer Resolution setzen
            Win32.TimeBeginPeriod(m_ResolutionInMilliseconds);

            //QueueTimer Queue erstellen
            m_HandleTimerQueue = Win32.CreateTimerQueue();
            m_GCHandleTimerQueue = GCHandle.Alloc(m_HandleTimerQueue);

            //Versuche QueueTimer zu starten
            var resultCreate = Win32.CreateTimerQueueTimer(out m_HandleTimer, m_HandleTimerQueue, m_DelegateTimerProc,
                IntPtr.Zero, dueTimeInMilliseconds, m_Milliseconds, Win32.WT_EXECUTEINTIMERTHREAD);
            if (resultCreate)
            {
                //Handle im Speicher halten
                m_GCHandleTimer = GCHandle.Alloc(m_HandleTimer, GCHandleType.Pinned);
                //QueueTimer ist gestartet
                m_IsRunning = true;
            }
        }

        /// <summary>
        ///     Stop
        /// </summary>
        public void Stop()
        {
            if (m_HandleTimer != IntPtr.Zero)
            {
                //QueueTimer beenden
                Win32.DeleteTimerQueueTimer(IntPtr.Zero, m_HandleTimer, IntPtr.Zero);
                //QueueTimer Resolution beenden
                Win32.TimeEndPeriod(m_ResolutionInMilliseconds);

                //QueueTimer Queue löschen
                if (m_HandleTimerQueue != IntPtr.Zero)
                {
                    Win32.DeleteTimerQueue(m_HandleTimerQueue);
                }

                //Handles freigeben
                if (m_GCHandleTimer.IsAllocated)
                {
                    m_GCHandleTimer.Free();
                }
                if (m_GCHandleTimerQueue.IsAllocated)
                {
                    m_GCHandleTimerQueue.Free();
                }

                //Variablen setzen
                m_HandleTimer = IntPtr.Zero;
                m_HandleTimerQueue = IntPtr.Zero;
                m_IsRunning = false;
            }
        }

        /// <summary>
        ///     OnTimer
        /// </summary>
        /// <param name="lpParameter"></param>
        /// <param name="TimerOrWaitFired"></param>
        private void OnTimer(IntPtr lpParameter, bool TimerOrWaitFired)
        {
            if (TimerTick != null)
            {
                TimerTick();
            }
        }
    }

    /// <summary>
    ///     QueueTimer
    /// </summary>
    public class EventTimer
    {
        //Attribute
        public delegate void DelegateTimerTick();

        private readonly Win32.TimerEventHandler m_DelegateTimeEvent;
        private GCHandle m_GCHandleTimer;

        private bool m_IsRunning;
        private uint m_Milliseconds = 20;
        private uint m_ResolutionInMilliseconds;
        private UInt32 m_TimerId;
        private UInt32 m_UserData;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        public EventTimer()
        {
            m_DelegateTimeEvent = OnTimer;
        }

        /// <summary>
        ///     IsRunning
        /// </summary>
        /// <returns></returns>
        public bool IsRunning
        {
            get { return m_IsRunning; }
        }

        /// <summary>
        ///     Milliseconds
        /// </summary>
        public uint Milliseconds
        {
            get { return m_Milliseconds; }
        }

        /// <summary>
        ///     ResolutionInMilliseconds
        /// </summary>
        public uint ResolutionInMilliseconds
        {
            get { return m_ResolutionInMilliseconds; }
        }

        public event DelegateTimerTick TimerTick;

        /// <summary>
        ///     SetBestResolution
        /// </summary>
        public static void SetBestResolution()
        {
            //QueueTimer Auflösung ermitteln
            var tc = new Win32.TimeCaps();
            Win32.TimeGetDevCaps(ref tc, (uint) Marshal.SizeOf(typeof (Win32.TimeCaps)));
            var resolution = Math.Max(tc.wPeriodMin, 0);

            //QueueTimer Resolution setzen
            Win32.TimeBeginPeriod(resolution);
        }

        /// <summary>
        ///     ResetResolution
        /// </summary>
        public static void ResetResolution()
        {
            //QueueTimer Auflösung ermitteln
            var tc = new Win32.TimeCaps();
            Win32.TimeGetDevCaps(ref tc, (uint) Marshal.SizeOf(typeof (Win32.TimeCaps)));
            var resolution = Math.Max(tc.wPeriodMin, 0);

            //QueueTimer Resolution deaktivieren
            Win32.TimeEndPeriod(resolution);
        }

        /// <summary>
        ///     Start
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <param name="dueTimeInMilliseconds"></param>
        public void Start(uint milliseconds, uint dueTimeInMilliseconds)
        {
            //Werte übernehmen
            m_Milliseconds = milliseconds;

            //Timer Auflösung ermitteln
            var tc = new Win32.TimeCaps();
            Win32.TimeGetDevCaps(ref tc, (uint) Marshal.SizeOf(typeof (Win32.TimeCaps)));
            m_ResolutionInMilliseconds = Math.Max(tc.wPeriodMin, 0);

            //Timer Resolution setzen
            Win32.TimeBeginPeriod(m_ResolutionInMilliseconds);

            //Versuche EventTimer zu starten
            m_TimerId = Win32.TimeSetEvent(m_Milliseconds, m_ResolutionInMilliseconds, m_DelegateTimeEvent,
                ref m_UserData, Win32.TIME_PERIODIC);
            if (m_TimerId > 0)
            {
                //Handle im Speicher halten
                m_GCHandleTimer = GCHandle.Alloc(m_TimerId, GCHandleType.Pinned);
                //QueueTimer ist gestartet
                m_IsRunning = true;
            }
        }

        /// <summary>
        ///     Stop
        /// </summary>
        public void Stop()
        {
            if (m_TimerId > 0)
            {
                //Timer beenden
                Win32.TimeKillEvent(m_TimerId);
                //Timer Resolution beenden
                Win32.TimeEndPeriod(m_ResolutionInMilliseconds);

                //Handles freigeben
                if (m_GCHandleTimer.IsAllocated)
                {
                    m_GCHandleTimer.Free();
                }

                //Variablen setzen
                m_TimerId = 0;
                m_IsRunning = false;
            }
        }

        /// <summary>
        ///     OnTimer
        /// </summary>
        /// <param name="lpParameter"></param>
        /// <param name="TimerOrWaitFired"></param>
        private void OnTimer(UInt32 id, UInt32 msg, ref UInt32 userCtx, UInt32 rsv1, UInt32 rsv2)
        {
            if (TimerTick != null)
            {
                TimerTick();
            }
        }
    }

    /// <summary>
    ///     Stopwatch
    /// </summary>
    public class Stopwatch
    {
        private readonly long m_Frequency;
        private long m_DurationTime;
        private long m_StartTime;

        /// <summary>
        ///     Stopwatch
        /// </summary>
        public Stopwatch()
        {
            //Prüfen
            if (Win32.QueryPerformanceFrequency(out m_Frequency) == false)
            {
                throw new Exception("High Performance counter not supported");
            }
        }

        /// <summary>
        ///     ElapsedMilliseconds
        /// </summary>
        public double ElapsedMilliseconds
        {
            get
            {
                Win32.QueryPerformanceCounter(out m_DurationTime);
                return (m_DurationTime - m_StartTime)/(double) m_Frequency*1000;
            }
        }

        /// <summary>
        ///     Start
        /// </summary>
        public void Start()
        {
            Win32.QueryPerformanceCounter(out m_StartTime);
            m_DurationTime = m_StartTime;
        }
    }
}
