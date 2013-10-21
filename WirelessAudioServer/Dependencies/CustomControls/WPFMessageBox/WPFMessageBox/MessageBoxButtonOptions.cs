




using System;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    [Flags]
    public enum MessageBoxButtonOptions
    {
        Ok = 0x1,
        Yes = 0x2,
        No = 0x4,
        Cancel = 0x8,
        Retry = 0x10
    }
}