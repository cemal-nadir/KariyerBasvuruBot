using App.Forms;
using System;
using System.Windows.Forms;

namespace App
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //  Application.ThreadException += new ThreadExceptionEventHandler(MyCommonExceptionHandlingMethod);

          

            Application.Run(new FrmMain());
        }
  //      private static void MyCommonExceptionHandlingMethod(object sender, ThreadExceptionEventArgs t)
  //      {
            //Exception handling...
  //      }
    }
}
