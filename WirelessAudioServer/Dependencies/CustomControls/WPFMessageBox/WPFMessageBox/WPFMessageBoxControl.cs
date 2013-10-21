




using System.Windows;
using System.Windows.Controls;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    public class WpfMessageBoxControl : Control
    {
        static WpfMessageBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfMessageBoxControl), new FrameworkPropertyMetadata(typeof(WpfMessageBoxControl)));
        }
    }
}
