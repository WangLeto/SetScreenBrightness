using System;
using System.Runtime.InteropServices;

namespace SetBrightness
{
    class MessageSender
    {
        private const int WmCopydata = 0x004A;
        public static string Msg = "max window";

        public struct CopyDataStruct
        {
            public IntPtr DwData;
            public int CbData;
            [MarshalAs(UnmanagedType.LPStr)] public string LpData;
        }

        #region Dll Import

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
            int hWnd, // handle to destination window
            int msg, // message
            int wParam, // first message parameter
            ref CopyDataStruct lParam // second message parameter
        );

        #endregion

        #region public methods

        public static void SendMessageToProcess(string processWindowName, string message)
        {
            var windowHandler = FindWindow(null, processWindowName);
            if ((int) windowHandler == 0)
            {
                return;
            }

            //send message
            var sarr = System.Text.Encoding.Default.GetBytes(message);
            var len = sarr.Length;
            CopyDataStruct cds;
            cds.DwData = (IntPtr) 100;
            cds.LpData = message;
            cds.CbData = len + 1;
            SendMessage((int) windowHandler, WmCopydata, 0, ref cds);
        }

        public static void SendMessageToProcess(IntPtr windowHandler, string message)
        {
            //send message
            var sarr = System.Text.Encoding.Default.GetBytes(message);
            var len = sarr.Length;
            CopyDataStruct cds;
            cds.DwData = (IntPtr)100;
            cds.LpData = message;
            cds.CbData = len + 1;
            SendMessage((int)windowHandler, WmCopydata, 0, ref cds);
        }

        #endregion
    }
}