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
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace WirelessAudioServer
{
    /// <summary>
    ///     LockerClass
    /// </summary>
    internal class LockerClass
    {
    }

    /// <summary>
    ///     WinSound
    /// </summary>
    public class WinSound
    {
        /// <summary>
        ///     Alle Abspielgeräte anzeigen
        /// </summary>
        /// <returns></returns>
        public static List<String> GetPlaybackNames()
        {
            //Ergebnis
            var list = new List<string>();
            var waveOutCap = new Win32.WAVEOUTCAPS();

            //Anzahl Devices
            var num = Win32.waveOutGetNumDevs();
            for (var i = 0; i < num; i++)
            {
                var hr = Win32.waveOutGetDevCaps(i, ref waveOutCap, Marshal.SizeOf(typeof (Win32.WAVEOUTCAPS)));
                if (hr == (int) Win32.HRESULT.S_OK)
                {
                    list.Add(waveOutCap.szPname);
                }
            }

            //Fertig
            return list;
        }

        /// <summary>
        ///     Alle Aufnahmegeräte anzeigen
        /// </summary>
        /// <returns></returns>
        public static List<String> GetRecordingNames()
        {
            //Ergebnis
            var list = new List<string>();
            var waveInCap = new Win32.WAVEINCAPS();

            //Anzahl Devices
            var num = Win32.waveInGetNumDevs();
            for (var i = 0; i < num; i++)
            {
                var hr = Win32.waveInGetDevCaps(i, ref waveInCap, Marshal.SizeOf(typeof (Win32.WAVEINCAPS)));
                if (hr == (int) Win32.HRESULT.S_OK)
                {
                    list.Add(waveInCap.szPname);
                }
            }

            //Fertig
            return list;
        }

        /// <summary>
        ///     GetWaveInDeviceIdByName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetWaveInDeviceIdByName(string name)
        {
            //Anzahl Devices
            var num = Win32.waveInGetNumDevs();

            //WaveIn Struktur
            var caps = new Win32.WAVEINCAPS();
            for (var i = 0; i < num; i++)
            {
                var hr = (Win32.HRESULT) Win32.waveInGetDevCaps(i, ref caps, Marshal.SizeOf(typeof (Win32.WAVEINCAPS)));
                if (hr == Win32.HRESULT.S_OK)
                {
                    if (caps.szPname == name)
                    {
                        //Gefunden
                        return i;
                    }
                }
            }

            //Nicht gefunden
            return Win32.WAVE_MAPPER;
        }

        /// <summary>
        ///     GetWaveOutDeviceIdByName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetWaveOutDeviceIdByName(string name)
        {
            //Anzahl Devices
            var num = Win32.waveOutGetNumDevs();

            //WaveIn Struktur
            var caps = new Win32.WAVEOUTCAPS();
            for (var i = 0; i < num; i++)
            {
                var hr =
                    (Win32.HRESULT) Win32.waveOutGetDevCaps(i, ref caps, Marshal.SizeOf(typeof (Win32.WAVEOUTCAPS)));
                if (hr == Win32.HRESULT.S_OK)
                {
                    if (caps.szPname == name)
                    {
                        //Gefunden
                        return i;
                    }
                }
            }

            //Nicht gefunden
            return Win32.WAVE_MAPPER;
        }

        /// <summary>
        ///     FlagToString
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static String FlagToString(Win32.WaveHdrFlags flag)
        {
            var sb = new StringBuilder();

            if ((flag & Win32.WaveHdrFlags.WHDR_PREPARED) > 0) sb.Append("PREPARED ");
            if ((flag & Win32.WaveHdrFlags.WHDR_BEGINLOOP) > 0) sb.Append("BEGINLOOP ");
            if ((flag & Win32.WaveHdrFlags.WHDR_ENDLOOP) > 0) sb.Append("ENDLOOP ");
            if ((flag & Win32.WaveHdrFlags.WHDR_INQUEUE) > 0) sb.Append("INQUEUE ");
            if ((flag & Win32.WaveHdrFlags.WHDR_DONE) > 0) sb.Append("DONE ");

            return sb.ToString();
        }
    }
}
