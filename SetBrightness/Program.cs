using System;
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
                    Form form = new Form1();
                    Application.Run(new TabForm());
                }
                else
                {
                    MessageSender.SendMessageToProcess(Form1.WindowName, MessageSender.Msg);
                    Application.Exit();
                }
            }
        }
    }
}