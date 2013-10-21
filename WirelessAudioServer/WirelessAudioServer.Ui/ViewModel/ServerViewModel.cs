using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WirelessAudioServer.Ui.Implementation;
using WirelessAudioServer.Wpf;

namespace WirelessAudioServer.Ui.ViewModel
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<ComboBoxItem> _resievers = new ObservableCollection<ComboBoxItem>();
        private readonly Recorder _audioSoundbox = new Recorder();
        private AudioDataSender _audioDataSender;
        private ComboBoxItem _selectedReciever;
        private int _port = 6800;
        private const int SampleRate = 44100;
        private const int ChanelsCount = 2;
        private const int PcmRate = 16;
        private const int BufferSize = 9600;
        private const int BufferCount = 8;

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerViewModel()
        {
            FillRecievers();
            ErrorText = "No connection, waiting for connect signal";
            IsEditingEnabled = true;
            OnPropertyChanged("IsEditingEnabled");
            OnPropertyChanged("IsStopStreamingEnabled");
        }

        public ObservableCollection<ComboBoxItem> Recievers { get { return _resievers; }}

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
            _audioDataSender = new AudioDataSender(_port);
            _audioSoundbox.Start(_selectedReciever.Content.ToString(), SampleRate, PcmRate, ChanelsCount, BufferCount, BufferSize);
            _audioDataSender.OpenConnection();
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
    }
}