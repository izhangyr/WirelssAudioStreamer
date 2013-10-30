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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using WirelessAudioServer.Ui.Implementation;
using WirelessAudioServer.Wpf;

namespace WirelessAudioServer.Ui.ViewModel
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<ComboBoxItem> _resievers = new ObservableCollection<ComboBoxItem>();
        private readonly List<ListViewItemViewModel> _connectedClients = new List<ListViewItemViewModel>();
        private readonly Recorder _audioSoundbox = new Recorder();
        private readonly AudioDataSender _audioDataSender;
        private ComboBoxItem _selectedReciever;
        private int _port = 6800;
        private const int SampleRate = 11025;
        private const int ChanelsCount = 2;
        private const int PcmRate = 16;
        private const int BufferSize = 2400;
        private const int BufferCount = 8;

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerViewModel()
        {
            FillRecievers();
            _audioDataSender = new AudioDataSender(ConnectionStateChangedCallback);
            ErrorText = "No connection, waiting for connect signal";
            IsEditingEnabled = true;
            OnPropertyChanged("IsEditingEnabled");
            OnPropertyChanged("IsStopStreamingEnabled");
        }

        public ObservableCollection<ComboBoxItem> Recievers { get { return _resievers; } }

        public List<ListViewItemViewModel> ConnectedClients { get { return _connectedClients; } } 

        public string ServerIpAddress {get {return Dns.GetHostAddresses("").FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();}}

        public bool IsEditingEnabled { get; set; }

        public bool IsStopStreamingEnabled { get { return !IsEditingEnabled; } }

        public string Port
        {
            get
            {
                return _port.ToString(CultureInfo.InvariantCulture);
            }

            set
            {
                int tempInt;
                if (Int32.TryParse(value, out tempInt))
                {
                    _port = tempInt;
                }

                OnPropertyChanged("Port");
            }
            
        }

        public string ErrorText { get; set; }

        public ICommand StartStreaming { get { return new RelayCommand(StartStreamingCommand, () => _port != 0 && _selectedReciever != null); } }

        public ICommand StopStreaming { get {return new RelayCommand(StopStreamingCommand, () => !IsEditingEnabled);} }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FillRecievers()
        {
            var names = WinSound.GetRecordingNames();
            foreach (var item in names.Select(name => new ComboBoxItem
            {
                Content = name,
            }))
            {
                item.Selected += SelecterRecieverChanged;
                Recievers.Add(item);
            }

            OnPropertyChanged("Recievers");
        }

        private void StopStreamingCommand()
        {
            _audioSoundbox.DataRecorded -= SendAudioData;
            _audioSoundbox.Stop();
            _audioDataSender.Close();
            ErrorText = "No connection, waiting for connect signal";
            IsEditingEnabled = true;
            OnPropertyChanged("IsEditingEnabled");
            OnPropertyChanged("IsStopStreamingEnabled");
            OnPropertyChanged("ErrorText");
        }

        private void StartStreamingCommand()
        {
            IsEditingEnabled = false;
            OnPropertyChanged("IsEditingEnabled");
            OnPropertyChanged("IsStopStreamingEnabled");
            _audioSoundbox.Start(_selectedReciever.Content.ToString(), SampleRate, PcmRate, ChanelsCount, BufferCount, BufferSize);
            _audioDataSender.OpenConnection(_port);
            _audioSoundbox.DataRecorded += SendAudioData;
        }
        
        private void SelecterRecieverChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            _selectedReciever = (ComboBoxItem)sender;
        }

        private void SendAudioData(byte[] bytes)
        {
            if (_audioDataSender.ConnectionEstablished)
            {
                _audioDataSender.SendBytes(bytes);

                if (string.IsNullOrEmpty(ErrorText))
                {
                    return;
                }

                ErrorText = string.Empty;
                OnPropertyChanged("ErrorText");
                return;
            }

            ErrorText = "No connection, waiting for connect signal";
            OnPropertyChanged("ErrorText");
        }

        private void ConnectionStateChangedCallback(IEnumerable<string> connectedClients)
        {
            ConnectedClients.Clear();
            foreach (var connectedClient in connectedClients)
            {
                ConnectedClients.Add(new ListViewItemViewModel {Text = string.Format("Client connected from IP: {0}", connectedClient)});
            }

            OnPropertyChanged("ConnectedClients");
        }
    }
}
