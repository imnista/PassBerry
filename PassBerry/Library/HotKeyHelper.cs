namespace PassBerry.Library
{
    #region using directives

    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    #endregion using directives

    internal class HotKeyHelper
    {
        [Flags]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            Others = 6,
            WindowsKey = 8
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, Keys vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,
            int id
            );
    }
}