using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class CloseButtonBehavior : Behavior<Button>
    {
        private Window _parentWindow;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += (sender, args) => CloseWindow();
            _parentWindow = (Window)WpfElementsHelper.GetTopParent(AssociatedObject, typeof(Window));
        }

        private void CloseWindow()
        {
            _parentWindow.Close();
        }
    }
}