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
using System.Windows;
using System.Windows.Interop;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    public class SystemMenuHelper
    {
        private HwndSource _hwndSource;

        public SystemMenuHelper(Window window)
        {
            AddHook(window);
        }

        public bool DisableCloseButton { get; set; }

        public bool RemoveResizeMenu { get; set; }

        private void AddHook(Window window)
        {
            if (_hwndSource == null)
            {
                _hwndSource = PresentationSource.FromVisual(window) as HwndSource;
                if (_hwndSource != null)
                {
                    _hwndSource.AddHook(HwndSourceHook);
                }
            }
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr param, IntPtr longParam, ref bool handled)
        {
            if (msg == NativeMethods.ShowWindow)
            {
                IntPtr menu = NativeMethods.GetSystemMenu(hwnd, false);
                if (menu != IntPtr.Zero)
                {
                    // handle disabling the close button and system menu item
                    if (DisableCloseButton)
                    {
                        NativeMethods.EnableMenuItem(menu, NativeMethods.Close, NativeMethods.ByCommand | NativeMethods.Grayed);
                    }

                    // handles removing the resize items from the system menu
                    if (RemoveResizeMenu)
                    {
                        NativeMethods.RemoveMenu(menu, NativeMethods.Restore, NativeMethods.ByCommand);
                        NativeMethods.RemoveMenu(menu, NativeMethods.Size, NativeMethods.ByCommand);
                        NativeMethods.RemoveMenu(menu, NativeMethods.Minimize, NativeMethods.ByCommand);
                        NativeMethods.RemoveMenu(menu, NativeMethods.Maximize, NativeMethods.ByCommand);
                    }
                }
            }

            return IntPtr.Zero;
        }
    }
}
