using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SetBrightness
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            using (new System.Threading.Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    var tabForm = new TabForm();
                    Application.Run();
                }
                else
                {
                    MessageSender.SendMessageToProcess(TabForm.WindowName, MessageSender.Msg);
                    Application.Exit();
                }
            }

        }
    }
}