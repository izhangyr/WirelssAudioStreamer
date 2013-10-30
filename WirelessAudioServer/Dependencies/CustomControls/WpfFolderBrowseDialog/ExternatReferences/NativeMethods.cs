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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.ExternalReferences
{
    internal static class NativeMethods
    {
        private const uint ShgfiIcon = 0x100;
        private const uint ShgfiLargeicon = 0x0; // 'Large icon

        public static ImageSource GetLargeIcon(string file)
        {
            var shinfo = new SHFILEINFO();
            SHGetFileInfo(file, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), ShgfiIcon | ShgfiLargeicon);
            var imgSrc = ToImageSource(shinfo.hIcon);
            return imgSrc;
        }

        [SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0", Justification = "It mustn't be Unicode"), DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        private static ImageSource ToImageSource(IntPtr icon)
        {
            try
            {
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                return imageSource;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public readonly IntPtr hIcon;
            private readonly IntPtr iIcon;
            private readonly uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            private readonly string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            private readonly string szTypeName;
        }
    }
}
