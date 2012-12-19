using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Authentication;
using ENDA.Diagnostics;

namespace PLCNetLibDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(new Demo());
        }
        
        private static Logger log = new Logger("Application");
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception is InvalidCredentialException)
            {
                MessageBox.Show("Invalid password");
                return;
            }
            MessageBox.Show("An unhandled error occured: " + e.Exception.Message);
            log.Error("An unhandled error occured: " + e.Exception.Message + "\r\n" + e.Exception.StackTrace);
        }
    }
}
