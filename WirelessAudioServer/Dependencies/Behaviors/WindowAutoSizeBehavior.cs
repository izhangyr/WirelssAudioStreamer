using System;
using System.Windows;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class WindowAutoSizeBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += FitWindow;
        }

        private static void FitWindow(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window == null)
            {
                return;
            }

            window.SizeToContent = SizeToContent.WidthAndHeight;
            var enlargeProportion = window.ActualHeight / window.MinHeight;
            window.SizeToContent = SizeToContent.Manual;
            window.Height = window.MinHeight * enlargeProportion;
            window.Width = window.MinWidth * enlargeProportion;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = ((SystemParameters.WorkArea.Width - window.ActualWidth) / 2) + SystemParameters.WorkArea.Left;
            window.Top = ((SystemParameters.WorkArea.Height - window.ActualHeight) / 2) + SystemParameters.WorkArea.Top;
            window.UpdateLayout();
            window.Activate();
            window.Opacity = 100;
            window.Topmost = true;
            window.Topmost = false;
            window.Focus();
        }
    }
}