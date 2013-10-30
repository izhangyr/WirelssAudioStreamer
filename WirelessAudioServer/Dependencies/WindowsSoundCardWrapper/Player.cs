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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

#endregion

namespace WirelessAudioServer
{
    /// <summary>
    ///     Player
    /// </summary>
    public class Player
    {
        //Attribute
        public delegate void DelegateStopped();

        private readonly AutoResetEvent AutoResetEventDataPlayed = new AutoResetEvent(false);

        private readonly LockerClass Locker = new LockerClass();
        private readonly Win32.DelegateWaveOutProc delegateWaveOutProc;
        private int BitsPerSample = 16;
        private int BufferCount = 8;
        private int Channels = 1;
        private GCHandle[] GCWaveOutHandleBuffers;
        private GCHandle[] GCWaveOutHandleHeaders;
        private bool IsBlocking;
        private bool IsClosed;
        private bool IsPaused;
        private bool IsStarted;
        private bool IsThreadPlayWaveOutRunning;
        private bool IsWaveOutOpened;
        private LockerClass LockerCopy = new LockerClass();
        private int SamplesPerSecond = 8000;
        private Thread ThreadPlayWaveOut;
        private String WaveOutDeviceName = "";
        private Win32.WAVEHDR[] WaveOutHeaders;
        private IntPtr hWaveOut = IntPtr.Zero;

        /// <summary>
        ///     Konstruktor
        /// </summary>
        public Player()
        {
            delegateWaveOutProc = waveOutProc;
        }

        /// <summary>
        ///     Paused
        /// </summary>
        public bool Paused
        {
            get { return IsPaused; }
        }

        /// <summary>
        ///     Opened
        /// </summary>
        public bool Opened
        {
            get { return IsWaveOutOpened & IsClosed == false; }
        }

        /// <summary>
        ///     Playing
        /// </summary>
        public bool Playing
        {
            get
            {
                if (Opened && IsStarted)
                {
                    foreach (var header in WaveOutHeaders)
                    {
                        if (IsHeaderInqueue(header))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public event DelegateStopped PlayerClosed;
        public event DelegateStopped PlayerStopped;

        /// <summary>
        ///     CreateWaveOutHeaders
        /// </summary>
        /// <returns></returns>
        private bool CreateWaveOutHeaders()
        {
            //Buffer anlegen
            WaveOutHeaders = new Win32.WAVEHDR[BufferCount];
            GCWaveOutHandleBuffers = new GCHandle[BufferCount];
            GCWaveOutHandleHeaders = new GCHandle[BufferCount];
            var createdHeaders = 0;

            //Für jeden Buffer
            for (var i = 0; i < BufferCount; i++)
            {
                //Header erstellen
                WaveOutHeaders[i].dwLoops = 0;
                WaveOutHeaders[i].dwUser = IntPtr.Zero;
                WaveOutHeaders[i].lpNext = IntPtr.Zero;
                WaveOutHeaders[i].reserved = IntPtr.Zero;

                //Im Speicher verankern
                GCWaveOutHandleHeaders[i] = GCHandle.Alloc(WaveOutHeaders[i], GCHandleType.Pinned);

                //Wenn der Buffer vorbereitet werden konnte
                var hr = Win32.waveOutPrepareHeader(hWaveOut, ref WaveOutHeaders[i], Marshal.SizeOf(WaveOutHeaders[i]));
                if (hr == Win32.MMRESULT.MMSYSERR_NOERROR)
                {
                    createdHeaders++;
                }
            }

            //Fertig
            return (createdHeaders == BufferCount);
        }

        /// <summary>
        ///     FreeWaveInHeaders
        /// </summary>
        private void FreeWaveOutHeaders()
        {
            try
            {
                if (WaveOutHeaders != null)
                {
                    for (var i = 0; i < WaveOutHeaders.Length; i++)
                    {
                        var hr = Win32.waveOutUnprepareHeader(hWaveOut, ref WaveOutHeaders[i],
                            Marshal.SizeOf(WaveOutHeaders[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        /// <summary>
        ///     StartThreadRecording
        /// </summary>
        private void StartThreadPlayWaveOut()
        {
            if (IsThreadPlayWaveOutRunning == false)
            {
                ThreadPlayWaveOut = new Thread(OnThreadPlayWaveOut);
                IsThreadPlayWaveOutRunning = true;
                ThreadPlayWaveOut.Name = "PlayWaveOut";
                ThreadPlayWaveOut.Priority = ThreadPriority.Highest;
                ThreadPlayWaveOut.Start();
            }
        }

        /// <summary>
        ///     PlayBytes. Bytes in gleich grosse Stücke teilen und einzeln abspielen
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private bool PlayBytes(Byte[] bytes)
        {
            if (bytes.Length > 0)
            {
                //Grösse der Bytestücke 
                var byteSize = bytes.Length/BufferCount;
                var currentPos = 0;

                //Für jeden möglichen Buffer
                for (var count = 0; count < BufferCount; count++)
                {
                    //Nächsten freien Buffer ermitteln
                    var index = GetNextFreeWaveOutHeaderIndex();
                    if (index != -1)
                    {
                        try
                        {
                            //Teilstück kopieren
                            var partByte = new Byte[byteSize];
                            Array.Copy(bytes, currentPos, partByte, 0, byteSize);
                            currentPos += byteSize;

                            //Daten global kopieren
                            GCWaveOutHandleBuffers[index] = GCHandle.Alloc(partByte, GCHandleType.Pinned);
                            WaveOutHeaders[index].lpData = GCWaveOutHandleBuffers[index].AddrOfPinnedObject();
                            WaveOutHeaders[index].dwBufferLength = (uint) partByte.Length;
                            WaveOutHeaders[index].dwUser = (IntPtr) index;
                        }
                        catch (Exception ex)
                        {
                            //Fehler beim Kopieren
                            Debug.WriteLine(String.Format("CopyBytesToFreeWaveOutHeaders() | {0}", ex.Message));
                            AutoResetEventDataPlayed.Set();
                            return false;
                        }

                        //Wenn noch geöffnet
                        if (hWaveOut != null)
                        {
                            //Abspielen
                            var hr = Win32.waveOutWrite(hWaveOut, ref WaveOutHeaders[index],
                                Marshal.SizeOf(WaveOutHeaders[index]));
                            if (hr != Win32.MMRESULT.MMSYSERR_NOERROR)
                            {
                                //Fehler beim Abspielen
                                AutoResetEventDataPlayed.Set();
                                return false;
                            }
                        }
                        else
                        {
                            //WaveOut ungültig
                            return false;
                        }
                    }
                    else
                    {
                        //Nicht genügend freie Buffer vorhanden
                        return false;
                    }
                }
                return true;
            }
            //Keine Daten vorhanden
            return false;
        }

        /// <summary>
        ///     OpenWaveOuz
        /// </summary>
        /// <returns></returns>
        private bool OpenWaveOut()
        {
            if (hWaveOut == IntPtr.Zero)
            {
                //Wenn nicht schon offen
                if (IsWaveOutOpened == false)
                {
                    //Format bestimmen
                    var waveFormatEx = new Win32.WAVEFORMATEX();
                    waveFormatEx.wFormatTag = (ushort) Win32.WaveFormatFlags.WAVE_FORMAT_PCM;
                    waveFormatEx.nChannels = (ushort) Channels;
                    waveFormatEx.nSamplesPerSec = (ushort) SamplesPerSecond;
                    waveFormatEx.wBitsPerSample = (ushort) BitsPerSample;
                    waveFormatEx.nBlockAlign = (ushort) ((waveFormatEx.wBitsPerSample*waveFormatEx.nChannels) >> 3);
                    waveFormatEx.nAvgBytesPerSec = waveFormatEx.nBlockAlign*waveFormatEx.nSamplesPerSec;

                    //WaveOut Gerät ermitteln
                    var deviceId = WinSound.GetWaveOutDeviceIdByName(WaveOutDeviceName);
                    //WaveIn Gerät öffnen
                    var hr = Win32.waveOutOpen(ref hWaveOut, deviceId, ref waveFormatEx, delegateWaveOutProc, 0,
                        (int) Win32.WaveProcFlags.CALLBACK_FUNCTION);

                    //Wenn nicht erfolgreich
                    if (hr != Win32.MMRESULT.MMSYSERR_NOERROR)
                    {
                        IsWaveOutOpened = false;
                        return false;
                    }

                    //Handle sperren
                    GCHandle.Alloc(hWaveOut, GCHandleType.Pinned);
                }
            }

            IsWaveOutOpened = true;
            return true;
        }

        /// <summary>
        ///     Open
        /// </summary>
        /// <param name="waveInDeviceName"></param>
        /// <param name="waveOutDeviceName"></param>
        /// <param name="samplesPerSecond"></param>
        /// <param name="bitsPerSample"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        public bool Open(string waveOutDeviceName, int samplesPerSecond, int bitsPerSample, int channels,
            int bufferCount)
        {
            try
            {
                lock (Locker)
                {
                    //Wenn nicht schon geöffnet
                    if (Opened == false)
                    {
                        //Daten übernehmen
                        WaveOutDeviceName = waveOutDeviceName;
                        SamplesPerSecond = samplesPerSecond;
                        BitsPerSample = bitsPerSample;
                        Channels = channels;
                        BufferCount = Math.Max(bufferCount, 1);

                        //Wenn WaveOut geöffnet werden konnte
                        if (OpenWaveOut())
                        {
                            //Wenn alle Buffer erzeugt werden konnten
                            if (CreateWaveOutHeaders())
                            {
                                //Thread starten
                                StartThreadPlayWaveOut();
                                IsClosed = false;
                                return true;
                            }
                        }
                    }

                    //Schon geöffnet
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Start | {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        ///     PlayData
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="isBlocking"></param>
        /// <returns></returns>
        public bool PlayData(Byte[] datas, bool isBlocking)
        {
            try
            {
                if (Opened)
                {
                    var index = GetNextFreeWaveOutHeaderIndex();
                    if (index != -1)
                    {
                        //Werte übernehmen
                        IsBlocking = isBlocking;

                        //Daten kopieren
                        GCWaveOutHandleBuffers[index] = GCHandle.Alloc(datas, GCHandleType.Pinned);
                        WaveOutHeaders[index].lpData = GCWaveOutHandleBuffers[index].AddrOfPinnedObject();
                        WaveOutHeaders[index].dwBufferLength = (uint) datas.Length;
                        WaveOutHeaders[index].dwUser = (IntPtr) index;

                        //Abspielen
                        IsStarted = true;
                        var hr = Win32.waveOutWrite(hWaveOut, ref WaveOutHeaders[index],
                            Marshal.SizeOf(WaveOutHeaders[index]));
                        if (hr == Win32.MMRESULT.MMSYSERR_NOERROR)
                        {
                            //Wenn blockierend
                            if (isBlocking)
                            {
                                AutoResetEventDataPlayed.WaitOne();
                                AutoResetEventDataPlayed.Set();
                            }
                            return true;
                        }
                        //Fehler beim Abspielen
                        AutoResetEventDataPlayed.Set();
                        return false;
                    }
                    //Kein freier Ausgabebuffer vorhanden
                    Debug.WriteLine(String.Format("No free WaveOut Buffer found | {0}", DateTime.Now.ToLongTimeString()));
                    return false;
                }
                //Nicht geöffnet
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("PlayData | {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        ///     PlayFile (Wave Files)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool PlayFile(string fileName, string waveOutDeviceName)
        {
            lock (Locker)
            {
                try
                {
                    //WaveFile auslesen
                    var header = WaveFile.Read(fileName);

                    //Wenn Daten vorhanden
                    if (header.Payload.Length > 0)
                    {
                        //Wenn geöffnet
                        if (Open(waveOutDeviceName, (int) header.SamplesPerSecond, header.BitsPerSample, header.Channels,
                            8))
                        {
                            var index = GetNextFreeWaveOutHeaderIndex();
                            if (index != -1)
                            {
                                //Bytes Teilweise in Ausgabebuffer abspielen
                                IsStarted = true;
                                return PlayBytes(header.Payload);
                            }
                            //Kein freier Ausgabebuffer vorhanden
                            AutoResetEventDataPlayed.Set();
                            return false;
                        }
                        //Nicht geöffnet
                        AutoResetEventDataPlayed.Set();
                        return false;
                    }
                    //Fehlerhafte Datei
                    AutoResetEventDataPlayed.Set();
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("PlayFile | {0}", ex.Message));
                    AutoResetEventDataPlayed.Set();
                    return false;
                }
            }
        }

        /// <summary>
        ///     Close
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                lock (Locker)
                {
                    //Wenn geöffnet
                    if (Opened)
                    {
                        //Als manuel beendet setzen
                        IsClosed = true;
                        var hr = Win32.waveOutReset(hWaveOut);

                        //Warten bis alle Daten fertig abgespielt
                        var count = 0;
                        while (Win32.waveOutClose(hWaveOut) != Win32.MMRESULT.MMSYSERR_NOERROR && count <= 100)
                        {
                            Thread.Sleep(50);
                            count++;
                        }

                        //Variablen setzen
                        IsWaveOutOpened = false;
                        AutoResetEventDataPlayed.Set();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Close | {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        ///     Stop
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            try
            {
                lock (Locker)
                {
                    //Wenn geöffnet
                    if (Opened)
                    {
                        //Variablen setzen
                        IsPaused = false;
                        AutoResetEventDataPlayed.Set();

                        //WaveOut beenden
                        CloseWaveOut();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Stop | {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        ///     CloseWaveOut
        /// </summary>
        /// <returns></returns>
        private void CloseWaveOut()
        {
            //Anhalten
            var hr = Win32.waveOutReset(hWaveOut);
            //Header Handles freigeben
            FreeWaveOutHeaders();
            //Schliessen
            hr = Win32.waveOutClose(hWaveOut);
        }

        /// <summary>
        ///     StartPause
        /// </summary>
        /// <returns></returns>
        public bool StartPause()
        {
            try
            {
                lock (Locker)
                {
                    //Wenn geöffnet
                    if (Opened)
                    {
                        //Wenn nicht schon pausiert
                        if (IsPaused == false)
                        {
                            //Pausieren
                            var hr = Win32.waveOutPause(hWaveOut);
                            if (hr == Win32.MMRESULT.MMSYSERR_NOERROR)
                            {
                                //Speichern
                                IsPaused = true;
                                AutoResetEventDataPlayed.Set();
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("StartPause | {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        ///     EndPause
        /// </summary>
        /// <returns></returns>
        public bool EndPause()
        {
            try
            {
                lock (Locker)
                {
                    //Wenn geöffnet
                    if (Opened)
                    {
                        //Wenn pausiert
                        if (IsPaused)
                        {
                            //Pausieren
                            var hr = Win32.waveOutRestart(hWaveOut);
                            if (hr == Win32.MMRESULT.MMSYSERR_NOERROR)
                            {
                                //Speichern
                                IsPaused = false;
                                AutoResetEventDataPlayed.Set();
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("EndPause | {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        ///     GetNextFreeWaveOutHeaderIndex
        /// </summary>
        /// <returns></returns>
        private int GetNextFreeWaveOutHeaderIndex()
        {
            for (var i = 0; i < WaveOutHeaders.Length; i++)
            {
                if (IsHeaderPrepared(WaveOutHeaders[i]) && !IsHeaderInqueue(WaveOutHeaders[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        ///     IsHeaderPrepared
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        private bool IsHeaderPrepared(Win32.WAVEHDR header)
        {
            return (header.dwFlags & Win32.WaveHdrFlags.WHDR_PREPARED) > 0;
        }

        /// <summary>
        ///     IsHeaderInqueue
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        private bool IsHeaderInqueue(Win32.WAVEHDR header)
        {
            return (header.dwFlags & Win32.WaveHdrFlags.WHDR_INQUEUE) > 0;
        }

        /// <summary>
        ///     waveOutProc
        /// </summary>
        /// <param name="hWaveOut"></param>
        /// <param name="msg"></param>
        /// <param name="dwInstance"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        private void waveOutProc(IntPtr hWaveOut, Win32.WOM_Messages msg, IntPtr dwInstance,
            ref Win32.WAVEHDR waveHeader, IntPtr lParam)
        {
            try
            {
                switch (msg)
                {
                        //Open
                    case Win32.WOM_Messages.OPEN:
                        break;

                        //Done
                    case Win32.WOM_Messages.DONE:

                        //Vermerken das Daten ankommen
                        IsStarted = true;
                        //Speicher freigeben
                        var hBuffer = GCWaveOutHandleBuffers[(int) waveHeader.dwUser];
                        if (hBuffer.IsAllocated)
                        {
                            //Wenn wirklich belegt
                            if ((waveHeader.dwFlags & Win32.WaveHdrFlags.WHDR_DONE) > 0 &&
                                ((waveHeader.dwFlags & Win32.WaveHdrFlags.WHDR_PREPARED) > 0))
                            {
                                hBuffer.Free();
                            }
                        }
                        AutoResetEventDataPlayed.Set();
                        break;

                        //Close
                    case Win32.WOM_Messages.CLOSE:
                        IsStarted = false;
                        IsWaveOutOpened = false;
                        IsPaused = false;
                        IsClosed = true;
                        AutoResetEventDataPlayed.Set();
                        FreeWaveOutHeaders();
                        this.hWaveOut = IntPtr.Zero;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Player.cs | waveOutProc() | {0}", ex.Message));
                AutoResetEventDataPlayed.Set();
            }
        }

        /// <summary>
        ///     OnThreadRecording
        /// </summary>
        private void OnThreadPlayWaveOut()
        {
            while (Opened && !IsClosed)
            {
                //Warten bis Aufnahme beendet
                AutoResetEventDataPlayed.WaitOne();

                lock (Locker)
                {
                    if (Opened && !IsClosed)
                    {
                        //Variablen setzen
                        IsThreadPlayWaveOutRunning = true;

                        //Wenn keine Daten mehr abgespielt werden
                        if (!Playing)
                        {
                            //Wenn Daten abgespielt wurden
                            if (IsStarted)
                            {
                                IsStarted = false;
                                //Ereignis absenden
                                if (PlayerStopped != null)
                                {
                                    try
                                    {
                                        PlayerStopped();
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(String.Format("Player Stopped | {0}", ex.Message));
                                    }
                                    finally
                                    {
                                        AutoResetEventDataPlayed.Set();
                                    }
                                }
                            }
                        }
                    }
                }

                //Wenn blockierend
                if (IsBlocking)
                {
                    AutoResetEventDataPlayed.Set();
                }
            }

            lock (Locker)
            {
                //Variablen setzen
                IsThreadPlayWaveOutRunning = false;
            }

            //Ereignis aussenden
            if (PlayerClosed != null)
            {
                try
                {
                    PlayerClosed();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Player Closed | {0}", ex.Message));
                }
            }
        }
    }
}
