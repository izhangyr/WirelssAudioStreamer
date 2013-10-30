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
