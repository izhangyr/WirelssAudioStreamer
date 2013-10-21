




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
