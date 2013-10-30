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
    public class Recorder
    {
        //Attribute
        public delegate void DelegateDataRecorded(Byte[] bytes);

        public delegate void DelegateStopped();

        private readonly AutoResetEvent _autoResetEventDataRecorded = new AutoResetEvent(false);

        private readonly LockerClass _locker = new LockerClass();
        private readonly Win32.DelegateWaveInProc _delegateWaveInProc;
        private int _bitsPerSample = 16;
        private int _bufferCount = 8;
        private int _bufferSize = 1024;
        private int _channels = 1;
        private Win32.WAVEHDR _currentRecordedHeader;
        private GCHandle[] _gcWaveInHandleBuffers;
        private GCHandle[] _gcWaveInHandleHeaders;
        private bool _isDataIncomming;
        private bool _isThreadRecordingRunning;
        private bool _isWaveInOpened;
        private bool _isWaveInStarted;
        private int _samplesPerSecond = 8000;
        private bool _stopped;
        private Thread _threadRecording;
        private String _waveInDeviceName = "";
        private Win32.WAVEHDR[] _waveInHeaders;
        private IntPtr _hWaveIn = IntPtr.Zero;

        public Recorder()
        {
            _delegateWaveInProc = WaveInProc;
        }

        public bool Started
        {
            get { return _isWaveInStarted && _isWaveInOpened && _isThreadRecordingRunning; }
        }

        public event DelegateStopped RecordingStopped;
        public event DelegateDataRecorded DataRecorded;

        private bool CreateWaveInHeaders()
        {
            _waveInHeaders = new Win32.WAVEHDR[_bufferCount];
            _gcWaveInHandleBuffers = new GCHandle[_bufferCount];
            _gcWaveInHandleHeaders = new GCHandle[_bufferCount];
            var createdHeaders = 0;

            for (var i = 0; i < _bufferCount; i++)
            {
                var bytes = new Byte[_bufferSize];
                _gcWaveInHandleBuffers[i] = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                _waveInHeaders[i].lpData = _gcWaveInHandleBuffers[i].AddrOfPinnedObject();
                _waveInHeaders[i].dwBufferLength = (uint) _bufferSize;
                _waveInHeaders[i].dwLoops = 0;
                _waveInHeaders[i].dwUser = IntPtr.Zero;
                _waveInHeaders[i].lpNext = IntPtr.Zero;
                _waveInHeaders[i].reserved = IntPtr.Zero;

                _gcWaveInHandleHeaders[i] = GCHandle.Alloc(_waveInHeaders[i], GCHandleType.Pinned);

                var hr = Win32.waveInPrepareHeader(_hWaveIn, ref _waveInHeaders[i], Marshal.SizeOf(_waveInHeaders[i]));
                if (hr == Win32.MMRESULT.MMSYSERR_NOERROR)
                {
                    if (i == 0)
                    {
                        Win32.waveInAddBuffer(_hWaveIn, ref _waveInHeaders[i], Marshal.SizeOf(_waveInHeaders[i]));
                    }

                    createdHeaders++;
                }
            }

            return (createdHeaders == _bufferCount);
        }

        private void FreeWaveInHeaders()
        {
            try
            {
                if (_waveInHeaders == null)
                {
                    return;
                }

                for (var i = 0; i < _waveInHeaders.Length; i++)
                {
                    Win32.waveInUnprepareHeader(_hWaveIn, ref _waveInHeaders[i], Marshal.SizeOf(_waveInHeaders[i]));
                    if (_gcWaveInHandleBuffers[i].IsAllocated)
                    {
                        _gcWaveInHandleBuffers[i].Free();
                    }

                    if (_gcWaveInHandleHeaders[i].IsAllocated)
                    {
                        _gcWaveInHandleHeaders[i].Free();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }

        private void StartThreadRecording()
        {
            if (Started)
            {
                return;
            }

            _threadRecording = new Thread(OnThreadRecording);
            _isThreadRecordingRunning = true;
            _threadRecording.Name = "Recording";
            _threadRecording.Priority = ThreadPriority.Highest;
            _threadRecording.Start();
        }

        private bool OpenWaveIn()
        {
            if (_hWaveIn == IntPtr.Zero)
            {
                if (_isWaveInOpened == false)
                {
                    var waveFormatEx = new Win32.WAVEFORMATEX
                    {
                        wFormatTag = (ushort) Win32.WaveFormatFlags.WAVE_FORMAT_PCM,
                        nChannels = (ushort) _channels,
                        nSamplesPerSec = (ushort) _samplesPerSecond,
                        wBitsPerSample = (ushort) _bitsPerSample
                    };

                    waveFormatEx.nBlockAlign = (ushort) ((waveFormatEx.wBitsPerSample*waveFormatEx.nChannels) >> 3);
                    waveFormatEx.nAvgBytesPerSec = waveFormatEx.nBlockAlign*waveFormatEx.nSamplesPerSec;

                    var deviceId = WinSound.GetWaveInDeviceIdByName(_waveInDeviceName);
                    Win32.waveInOpen(ref _hWaveIn, deviceId, ref waveFormatEx, _delegateWaveInProc, 0, (int) Win32.WaveProcFlags.CALLBACK_FUNCTION);
                    if (_hWaveIn == IntPtr.Zero)
                    {
                        return false;
                    }

                    GCHandle.Alloc(_hWaveIn, GCHandleType.Pinned);
                }
            }

            _isWaveInOpened = true;
            return true;
        }

        public bool Start(string waveInDeviceName, int samplesPerSecond, int bitsPerSample, int channels, int bufferCount, int bufferSize)
        {
            try
            {
                lock (_locker)
                {
                    if (Started)
                    {
                        return false;
                    }

                    _waveInDeviceName = waveInDeviceName;
                    _samplesPerSecond = samplesPerSecond;
                    _bitsPerSample = bitsPerSample;
                    _channels = channels;
                    _bufferCount = bufferCount;
                    _bufferSize = bufferSize;
                    if (!OpenWaveIn())
                    {
                        return false;
                    }

                    if (!CreateWaveInHeaders())
                    {
                        return false;
                    }

                    var hr = Win32.waveInStart(_hWaveIn);
                    if (hr != Win32.MMRESULT.MMSYSERR_NOERROR)
                    {
                        return false;
                    }

                    _isWaveInStarted = true;
                    StartThreadRecording();
                    _stopped = false;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Start | {0}", ex.Message));
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                lock (_locker)
                {
                    if (!Started)
                    {
                        return false;
                    }

                    _stopped = true;
                    _isThreadRecordingRunning = false;
                    CloseWaveIn();
                    _autoResetEventDataRecorded.Set();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Stop | {0}", ex.Message));
                return false;
            }
        }

        private void CloseWaveIn()
        {
            Win32.waveInStop(_hWaveIn);
            var resetCount = 0;
            while (IsAnyWaveInHeaderInState(Win32.WaveHdrFlags.WHDR_INQUEUE) & resetCount < 20)
            {
                Win32.waveInReset(_hWaveIn);
                Thread.Sleep(50);
                resetCount++;
            }

            FreeWaveInHeaders();
            Win32.waveInClose(_hWaveIn);
        }

        private bool IsAnyWaveInHeaderInState(Win32.WaveHdrFlags state)
        {
            for (var i = 0; i < _waveInHeaders.Length; i++)
            {
                if ((_waveInHeaders[i].dwFlags & state) == state)
                {
                    return true;
                }
            }

            return false;
        }

        private void WaveInProc(IntPtr hWaveIn, Win32.WIM_Messages msg, IntPtr dwInstance, ref Win32.WAVEHDR waveHeader, IntPtr lParam)
        {
            switch (msg)
            {
                case Win32.WIM_Messages.OPEN:
                    break;
                case Win32.WIM_Messages.DATA:
                    _isDataIncomming = true;
                    _currentRecordedHeader = waveHeader;
                    _autoResetEventDataRecorded.Set();
                    break;
                case Win32.WIM_Messages.CLOSE:
                    _isDataIncomming = false;
                    _isWaveInOpened = false;
                    _autoResetEventDataRecorded.Set();
                    FreeWaveInHeaders();
                    _hWaveIn = IntPtr.Zero;
                    break;
            }
        }

        private void OnThreadRecording()
        {
            while (Started && !_stopped)
            {
                _autoResetEventDataRecorded.WaitOne();

                try
                {
                    if (_currentRecordedHeader.dwBytesRecorded <= 0)
                    {
                        continue;
                    }

                    if (DataRecorded != null && _isDataIncomming)
                    {
                        try
                        {
                            var bytes = new Byte[_currentRecordedHeader.dwBytesRecorded];
                            Marshal.Copy(_currentRecordedHeader.lpData, bytes, 0, (int) _currentRecordedHeader.dwBytesRecorded);
                            DataRecorded(bytes);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(String.Format("Recorder.cs | OnThreadRecording() | {0}", ex.Message));
                        }
                    }

                    for (var i = 0; i < _waveInHeaders.Length; i++)
                    {
                        if ((_waveInHeaders[i].dwFlags & Win32.WaveHdrFlags.WHDR_INQUEUE) == 0)
                        {
                            Win32.waveInAddBuffer(_hWaveIn, ref _waveInHeaders[i], Marshal.SizeOf(_waveInHeaders[i]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }


            lock (_locker)
            {
                _isWaveInStarted = false;
                _isThreadRecordingRunning = false;
            }

            if (RecordingStopped == null)
            {
                return;
            }

            try
            {
                RecordingStopped();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Recording Stopped | {0}", ex.Message));
            }
        }
    }
}
