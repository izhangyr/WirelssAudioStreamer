




using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Interop;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    public static class WindowHelper
    {
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Replay.Common.Wpf.CustomControls.WpfMessageBox.NativeMethods.SetWindowLong(System.IntPtr,System.Int32,System.Int32)", Justification = "I don't need result.")]
        public static void RemoveIcon(Window window)
        {
            // Get this window's handle
            var hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            var extendedStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.ExtendedStyle);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.ExtendedStyle, extendedStyle | NativeMethods.DialogModalFrame);

            // Update the window's non-client area to reflect the changes
            NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.NoMove | NativeMethods.NoSize | NativeMethods.NoZorder | NativeMethods.FrameChanged);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Replay.Common.Wpf.CustomControls.WpfMessageBox.NativeMethods.SetWindowLong(System.IntPtr,System.Int32,System.Int32)", Justification = "I don't need result.")]
        public static void SetRightAligned(Window window)
        {
            // Get this window's handle
            var hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            var extendedStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.ExtendedStyle);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.ExtendedStyle, extendedStyle | NativeMethods.Right);

            // Update the window's non-client area to reflect the changes
            NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.NoMove | NativeMethods.NoSize | NativeMethods.NoZorder | NativeMethods.FrameChanged);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Replay.Common.Wpf.CustomControls.WpfMessageBox.NativeMethods.SetWindowLong(System.IntPtr,System.Int32,System.Int32)", Justification = "I don't need result.")]
        public static void SetReading(Window window)
        {
            // Get this window's handle
            var hwnd = new WindowInteropHelper(window).Handle;

            // Change the extended window style to not show a window icon
            var extendedStyle = NativeMethods.GetWindowLong(hwnd, NativeMethods.ExtendedStyle);
            NativeMethods.SetWindowLong(hwnd, NativeMethods.ExtendedStyle, extendedStyle | NativeMethods.Reading);

            // Update the window's non-client area to reflect the changes
            NativeMethods.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.NoMove | NativeMethods.NoSize | NativeMethods.NoZorder | NativeMethods.FrameChanged);
        }
    }
}
