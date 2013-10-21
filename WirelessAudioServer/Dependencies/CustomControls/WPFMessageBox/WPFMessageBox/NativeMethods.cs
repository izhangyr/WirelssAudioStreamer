




using System;
using System.Runtime.InteropServices;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    internal static class NativeMethods
    {
        public const int ExtendedStyle = -20;
        public const int DialogModalFrame = 0x0001;
        public const int Right = 0x00001000;
        public const int Reading = 0x00002000;
        public const int NoSize = 0x0001;
        public const int NoMove = 0x0002;
        public const int NoZorder = 0x0004;
        public const int FrameChanged = 0x0020;
        public const uint SetIcon = 0x0080;
        public const uint ByCommand = 0x00000000;
        public const uint Grayed = 0x00000001;
        public const uint Enabled = 0x00000000;
        public const uint Size = 0xF000;
        public const uint Restore = 0xF120;
        public const uint Minimize = 0xF020;
        public const uint Maximize = 0xF030;
        public const uint Close = 0xF060;
        public const int ShowWindow = 0x00000018;
        public const int CloseWindow = 0x10;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, [In, MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}