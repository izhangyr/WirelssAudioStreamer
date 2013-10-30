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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Replay.Common.Wpf.Behaviors;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    internal class MessageBoxViewModel : INotifyPropertyChanged
    {
        private readonly WpfMessageBoxWindow _view;

        private bool _isOkDefault;
        private bool _isYesDefault;
        private bool _isNoDefault;
        private bool _isCancelDefault;

        private string _title;
        private string _message;
        private MessageBoxButtonOptions _buttonOption;
        private MessageBoxOptions _options;
        
        private Visibility _yesNoVisibility;
        private Visibility _retryVisibility;
        private Visibility _cancelVisibility;
        private Visibility _okayVisibility;

        private HorizontalAlignment _contentTextAlignment;
        private FlowDirection _contentFlowDirection;
        private FlowDirection _titleFlowDirection;

        private ICommand _yesCommand;
        private ICommand _rejectCommand;
        private ICommand _cancelCommand;
        private ICommand _okayCommand;
        private ICommand _escapeCommand;
        private ICommand _closeCommand;
        private ImageSource _messageImageSource;

        public MessageBoxViewModel(
            WpfMessageBoxWindow view,
            string title,
            string message,
            MessageBoxButtonOptions buttonOption,
            MessageBoxImage image,
            MessageBoxResult defaultResult,
            MessageBoxOptions options)
        {
            Title = title;
            Message = message;
            ButtonOption = buttonOption;
            Options = options;

            SetDirections(options);
            SetButtonVisibility(buttonOption);
            SetImageSource(image);
            SetButtonDefault(defaultResult);
            _view = view;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MessageBoxResult Result { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public MessageBoxButtonOptions ButtonOption
        {
            get
            {
                return _buttonOption;
            }

            set
            {
                if (_buttonOption != value)
                {
                    _buttonOption = value;
                    NotifyPropertyChange("ButtonOption");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public MessageBoxOptions Options
        {
            get
            {
                return _options;
            } 

            set
            {
                if (_options != value)
                {
                    _options = value;
                    NotifyPropertyChange("Options");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyPropertyChange("Title");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyPropertyChange("Message");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public ImageSource MessageImageSource
        {
            get
            {
                return _messageImageSource;
            }

            set
            {
                _messageImageSource = value;
                NotifyPropertyChange("MessageImageSource");
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public Visibility YesNoVisibility
        {
            get
            {
                return _yesNoVisibility;
            }

            set
            {
                if (_yesNoVisibility != value)
                {
                    _yesNoVisibility = value;
                    NotifyPropertyChange("YesNoVisibility");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public Visibility RetryVisibility
        {
            get
            {
                return _retryVisibility;
            }

            set
            {
                if (_retryVisibility != value)
                {
                    _retryVisibility = value;
                    NotifyPropertyChange("RetryVisibility");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public Visibility CancelVisibility
        {
            get
            {
                return _cancelVisibility;
            }

            set
            {
                if (_cancelVisibility != value)
                {
                    _cancelVisibility = value;
                    NotifyPropertyChange("CancelVisibility");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public Visibility OkVisibility
        {
            get
            {
                return _okayVisibility;
            }

            set
            {
                if (_okayVisibility != value)
                {
                    _okayVisibility = value;
                    NotifyPropertyChange("OkVisibility");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public HorizontalAlignment ContentTextAlignment
        {
            get
            {
                return _contentTextAlignment;
            }

            set
            {
                if (_contentTextAlignment != value)
                {
                    _contentTextAlignment = value;
                    NotifyPropertyChange("ContentTextAlignment");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public FlowDirection ContentFlowDirection
        {
            get
            {
                return _contentFlowDirection;
            }

            set
            {
                if (_contentFlowDirection != value)
                {
                    _contentFlowDirection = value;
                    NotifyPropertyChange("ContentFlowDirection");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public FlowDirection TitleFlowDirection
        {
            get
            {
                return _titleFlowDirection;
            }

            set
            {
                if (_titleFlowDirection != value)
                {
                    _titleFlowDirection = value;
                    NotifyPropertyChange("TitleFlowDirection");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public bool IsOkDefault
        {
            get
            {
                return _isOkDefault;
            }

            set 
            {
                if (_isOkDefault != value)
                {
                    _isOkDefault = value;
                    NotifyPropertyChange("IsOkDefault");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public bool IsYesDefault
        {
            get
            {
                return _isYesDefault;
            }

            set
            {
                if (_isYesDefault != value)
                {
                    _isYesDefault = value;
                    NotifyPropertyChange("IsYesDefault");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public bool IsNoDefault
        {
            get
            {
                return _isNoDefault;
            }

            set
            {
                if (_isNoDefault != value)
                {
                    _isNoDefault = value;
                    NotifyPropertyChange("IsNoDefault");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public bool IsCancelDefault
        {
            get
            {
                return _isCancelDefault;
            }

            set
            {
                if (_isCancelDefault != value)
                {
                    _isCancelDefault = value;
                    NotifyPropertyChange("IsCancelDefault");
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public ICommand YesCommand
        {
            get
            {
                return _yesCommand ?? (_yesCommand = new RelayCommand(() =>
                    {
                        Result = MessageBoxResult.Yes;
                        _view.Close();
                    }));
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public ICommand NoCommand
        {
            get
            {
                return _rejectCommand ?? (_rejectCommand = new RelayCommand(() =>
                    {
                        Result = MessageBoxResult.No;
                        _view.Close();
                    }));
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(() =>
                    {
                        Result = MessageBoxResult.Cancel;
                        _view.Close();
                    }));
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "It's binded from view.")]
        public ICommand OkCommand
        {
            get
            {
                return _okayCommand ?? (_okayCommand = new RelayCommand(() =>
                    {
                        Result = MessageBoxResult.OK;
                        _view.Close();
                    }));
            }
        }

        public ICommand EscapeCommand
        {
            get
            {
                return _escapeCommand ?? (_escapeCommand = new RelayCommand(() =>
                    {
                        switch (ButtonOption)
                        {
                            case MessageBoxButtonOptions.Ok:
                                Result = MessageBoxResult.OK;
                                _view.Close();
                                break;

                            case
                                MessageBoxButtonOptions.Yes | MessageBoxButtonOptions.No |
                                MessageBoxButtonOptions.Cancel:
                            case MessageBoxButtonOptions.Ok | MessageBoxButtonOptions.Cancel:
                            case MessageBoxButtonOptions.Retry | MessageBoxButtonOptions.Cancel:
                                Result = MessageBoxResult.Cancel;
                                _view.Close();
                                break;

                            case MessageBoxButtonOptions.Yes | MessageBoxButtonOptions.No:
                                break;
                        }
                    }));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RelayCommand(() =>
                    {
                        if (Result != MessageBoxResult.None)
                        {
                            return;
                        }

                        switch (ButtonOption)
                        {
                            case MessageBoxButtonOptions.Ok:
                                Result = MessageBoxResult.OK;
                                break;

                            case
                                MessageBoxButtonOptions.Yes | MessageBoxButtonOptions.No |
                                MessageBoxButtonOptions.Cancel:
                            case MessageBoxButtonOptions.Ok | MessageBoxButtonOptions.Cancel:
                            case MessageBoxButtonOptions.Retry | MessageBoxButtonOptions.Cancel:
                                Result = MessageBoxResult.Cancel;
                                break;

                            case MessageBoxButtonOptions.Yes | MessageBoxButtonOptions.No:
                                break;
                        }
                    }));
            }
        }

        private void SetDirections(MessageBoxOptions options)
        {
            switch (options)
            { 
                case MessageBoxOptions.None:
                    ContentTextAlignment = HorizontalAlignment.Left;
                    ContentFlowDirection = FlowDirection.LeftToRight;
                    TitleFlowDirection = FlowDirection.LeftToRight;
                    break;

                case MessageBoxOptions.RightAlign:
                    ContentTextAlignment = HorizontalAlignment.Right;
                    ContentFlowDirection = FlowDirection.LeftToRight;
                    TitleFlowDirection = FlowDirection.LeftToRight;
                    break;

                case MessageBoxOptions.RtlReading:
                    ContentTextAlignment = HorizontalAlignment.Right;
                    ContentFlowDirection = FlowDirection.RightToLeft;
                    TitleFlowDirection = FlowDirection.RightToLeft;
                    break;

                case MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading:
                    ContentTextAlignment = HorizontalAlignment.Left;
                    ContentFlowDirection = FlowDirection.RightToLeft;
                    TitleFlowDirection = FlowDirection.RightToLeft;
                    break;
            }
        }

        private void NotifyPropertyChange(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void SetButtonDefault(MessageBoxResult defaultResult)
        {
            switch (defaultResult)
            { 
                case MessageBoxResult.OK:
                    IsOkDefault = true;
                    break;

                case MessageBoxResult.Yes:
                    IsYesDefault = true;
                    break;

                case MessageBoxResult.No:
                    IsNoDefault = true;
                    break;

                case MessageBoxResult.Cancel:
                    IsCancelDefault = true;
                    break;
            }
        }

        private void SetButtonVisibility(MessageBoxButtonOptions buttonOption)
        {
            switch (buttonOption)
            {
                case MessageBoxButtonOptions.Yes | MessageBoxButtonOptions.No:
                    OkVisibility = CancelVisibility = RetryVisibility = Visibility.Collapsed;
                    break;

                case MessageBoxButtonOptions.Yes | MessageBoxButtonOptions.No | MessageBoxButtonOptions.Cancel:
                    OkVisibility = RetryVisibility = Visibility.Collapsed;
                    break;

                case MessageBoxButtonOptions.Ok:
                    YesNoVisibility = RetryVisibility = CancelVisibility = Visibility.Collapsed;
                    break;

                case MessageBoxButtonOptions.Ok | MessageBoxButtonOptions.Cancel:
                    YesNoVisibility = RetryVisibility = Visibility.Collapsed;
                    break;

                case MessageBoxButtonOptions.Retry | MessageBoxButtonOptions.Cancel:
                    YesNoVisibility = OkVisibility = Visibility.Collapsed;
                    break;

                default:
                    OkVisibility = CancelVisibility = RetryVisibility = YesNoVisibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SetImageSource(MessageBoxImage image)
        {
            switch (image)
            {
                //case MessageBoxImage.Hand:
                //case MessageBoxImage.Stop:
                case MessageBoxImage.Error:
                    MessageImageSource = SystemIcons.Error.ToImageSource();
                    break;

                //case MessageBoxImage.Exclamation:
                case MessageBoxImage.Warning:
                    MessageImageSource = SystemIcons.Warning.ToImageSource();
                    break;

                case MessageBoxImage.Question:
                    MessageImageSource = SystemIcons.Question.ToImageSource();
                    break;

                //case MessageBoxImage.Asterisk:
                case MessageBoxImage.Information:
                    MessageImageSource = SystemIcons.Information.ToImageSource();
                    break;

                default:
                    MessageImageSource = null;
                    break;
            }
        }
    }
}
