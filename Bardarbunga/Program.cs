using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bardarbunga
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += currentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception eObj = (Exception)e.ExceptionObject;

            Console.WriteLine("Error Message: " + eObj.Message);
            MessageBox.Show("Unhandled Error Occured. Program terminating.\nAn error report has been sent to the developer\n\nMessage: " + eObj.Message);

            try
            {
                WebRequest request = WebRequest.Create("http://ilikeducks.com/LampSim/bugReport.php?eMessage=" + eObj.Message);
                WebResponse response = request.GetResponse();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
