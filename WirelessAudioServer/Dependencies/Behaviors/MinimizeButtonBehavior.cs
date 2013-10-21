using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class MinimizeButtonBehavior : Behavior<Button>
    {
        private Window _parentWindow;
        private bool _isMinimized;
        
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += (sender, args) => MinimizeWindow();
            _parentWindow = (Window)WpfElementsHelper.GetTopParent(AssociatedObject, typeof(Window));
        }

        private void MinimizeWindow()
        {
            _parentWindow.WindowState = WindowState.Minimized;
        }
    }
}