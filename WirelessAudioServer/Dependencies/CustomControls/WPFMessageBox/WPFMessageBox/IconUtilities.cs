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
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    internal static class IconUtilities 
    {
        public static ImageSource ToImageSource(this Icon icon) 
        { 
            var bitmap = icon.ToBitmap(); 
            var bitmapPointer = bitmap.GetHbitmap(); 
            var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(bitmapPointer, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); 
            if (!NativeMethods.DeleteObject(bitmapPointer)) 
            { 
                throw new Win32Exception(); 
            } 

            return wpfBitmap; 
        }
    } 
}
