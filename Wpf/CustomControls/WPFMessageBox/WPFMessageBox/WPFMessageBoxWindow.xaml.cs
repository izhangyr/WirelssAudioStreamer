




using System;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    public partial class WpfMessageBoxWindow : Window
    {
        [ThreadStatic]
        private static WpfMessageBoxWindow _messageBoxWindow;

        private MessageBoxViewModel _viewModel;

        public WpfMessageBoxWindow()
        {
            InitializeComponent();
        }
        
        public static MessageBoxResult Show(
            Action<Window> setOwner,
            string messageBoxText, 
            string caption,
            MessageBoxButtonOptions button, 
            MessageBoxImage icon, 
            MessageBoxResult defaultResult, 
            MessageBoxOptions options)
        {
            if ((options & MessageBoxOptions.DefaultDesktopOnly) == MessageBoxOptions.DefaultDesktopOnly)
            {
                throw new NotImplementedException();
            }

            if ((options & MessageBoxOptions.ServiceNotification) == MessageBoxOptions.ServiceNotification)
            {
                throw new NotImplementedException();
            }

            _messageBoxWindow = new WpfMessageBoxWindow();

            setOwner(_messageBoxWindow);

            PlayMessageBeep(icon);

            _messageBoxWindow._viewModel = new MessageBoxViewModel(_messageBoxWindow, caption, messageBoxText, button, icon, defaultResult, options);
            _messageBoxWindow.DataContext = _messageBoxWindow._viewModel;
            _messageBoxWindow.ShowDialog();
            return _messageBoxWindow._viewModel.Result;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // removes the application icon from the window top left corner
            // this is different than just hiding it
            WindowHelper.RemoveIcon(this);

            switch (_viewModel.Options)
            {
                case MessageBoxOptions.None:
                    break;

                case MessageBoxOptions.RightAlign:
                    WindowHelper.SetRightAligned(this);
                    break;

                case MessageBoxOptions.RtlReading:
                    break;

                case MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading:
                    break;
            }

            // disable close button if needed and remove resize menu items from the window system menu
            var systemMenuHelper = new SystemMenuHelper(this);

            if (_viewModel.ButtonOption == (MessageBoxButtonOptions.Yes | MessageBoxButtonOptions.No))
            {
                systemMenuHelper.DisableCloseButton = true;
            }

            systemMenuHelper.RemoveResizeMenu = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _viewModel.CloseCommand.Execute(null);
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _viewModel.EscapeCommand.Execute(null);
            }
        }

        private static void PlayMessageBeep(MessageBoxImage icon)
        {
            switch (icon)
            {
                //case MessageBoxImage.Hand:
                //case MessageBoxImage.Stop:
                case MessageBoxImage.Error:
                    SystemSounds.Hand.Play();
                    break;

                //case MessageBoxImage.Exclamation:
                case MessageBoxImage.Warning:
                    SystemSounds.Exclamation.Play();
                    break;

                case MessageBoxImage.Question:
                    SystemSounds.Question.Play();
                    break;

                //case MessageBoxImage.Asterisk:
                case MessageBoxImage.Information:
                    SystemSounds.Asterisk.Play();
                    break;

                default:
                    SystemSounds.Beep.Play();
                    break;
            }
        }
    }
}
