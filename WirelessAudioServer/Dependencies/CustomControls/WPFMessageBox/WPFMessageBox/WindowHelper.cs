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
