




using System.ComponentModel;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ViewModel
{
    public class InputFolderWindowViewModel : INotifyPropertyChanged
    {
        private string _newFolderName = string.Empty;
        private string _error = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NewFolderName 
        { 
            get
            {
                return _newFolderName;
            } 

            set 
            { 
                _newFolderName = value;
                OnPropertyChanged("NewFolderName");
            }
        }

        public string Error
        {
            get
            {
                return _error;
            }

            set
            {
                _error = value;
                OnPropertyChanged("Error");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}