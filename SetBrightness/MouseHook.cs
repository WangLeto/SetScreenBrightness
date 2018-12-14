using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace SetBrightness
{
    // 来自：https://github.com/rvknth043/Global-Low-Level-Key-Board-And-Mouse-Hook

    /// <summary>
    /// Class for intercepting low level Windows mouse hooks.
    /// </summary>
    internal class MouseHook
    {
        /// <summary>
        /// Internal callback processing function
        /// </summary>
        private delegate int MouseHookHandler(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// Function to be called when defined even occurs
        /// </summary>
        /// <param name="mouseStruct">MSLLHOOKSTRUCT mouse structure</param>
        public delegate void MouseHookCallback(Msllhookstruct mouseStruct, out bool goOn);

        public event MouseHookCallback MouseWheel;
        public event MouseHookCallback MouseLDown;
        public event MouseHookCallback MouseRDown;

        private IntPtr _hookId = IntPtr.Zero;

        private MouseHookHandler _mouseHookHandler;

        public void Install()
        {
            _mouseHookHandler = HookFunc;
            _hookId = SetHook(_mouseHookHandler);
        }

        /// <summary>
        /// Remove low level mouse hook
        /// </summary>
        public void Uninstall()
        {
            if (_hookId == IntPtr.Zero)
                return;

            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        /// <summary>
        /// Destructor. Unhook current hook
        /// </summary>
        ~MouseHook()
        {
            Uninstall();
        }

        /// <summary>
        /// Sets hook and assigns its ID for tracking
        /// </summary>
        /// <param name="proc">Internal callback function</param>
        /// <returns>Hook ID</returns>
        private static IntPtr SetHook(MouseHookHandler proc)
        {
            using (var module = Process.GetCurrentProcess().MainModule)
                return SetWindowsHookEx(WhMouseLl, proc, GetModuleHandle(module.ModuleName), 0);
        }

        /// <summary>
        /// Callback function
        /// </summary>
        private int HookFunc(int nCode, int wParam, IntPtr lParam)
        {
            // parse system messages
            if (nCode < 0)
            {
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
            }

            var msllhookstruct =
                (Msllhookstruct) Marshal.PtrToStructure(lParam, typeof(Msllhookstruct));
            switch ((MouseMessages) wParam)
            {
                case MouseMessages.WmMouseWheel:
                    if (!MsgHandle(MouseWheel, msllhookstruct))
                    {
                        return 1;
                    }

                    break;
                case MouseMessages.WmLbuttonDown:
                    if (!MsgHandle(MouseLDown, msllhookstruct))
                    {
                        return 1;
                    }

                    break;
                case MouseMessages.WmRbuttonDown:
                    if (!MsgHandle(MouseRDown, msllhookstruct))
                    {
                        return 1;
                    }

                    break;
                case MouseMessages.WmMbuttonDown:
                    break;
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static bool MsgHandle(MouseHookCallback callback, Msllhookstruct msllhookstruct)
        {
            bool @continue;
            callback(msllhookstruct, out @continue);
            return @continue;
        }

        #region WinAPI

        private const int WhMouseLl = 14;

        private enum MouseMessages
        {
            WmMouseWheel = 0x020A,
            WmLbuttonDown = 0x0201,
            WmRbuttonDown = 0x0204,
            WmMbuttonDown = 0x0207
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Msllhookstruct
        {
            public Point pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, MouseHookHandler lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}