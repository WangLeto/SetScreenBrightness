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
                    Application.Run();
                }
                else
                {
                    MessageBox.Show("程序已经在运行");
                    Application.Exit();
                }
            }
        }
    }
}