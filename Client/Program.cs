using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new LogIn());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LogIn loginForm = new LogIn();

            Application.Run(loginForm);
            if (loginForm.DialogResult == DialogResult.OK)
            {
                MainForm mainForm = new MainForm();
                mainForm.clientSocket = loginForm.clientSocket;
                mainForm.strName = loginForm.strName;

                mainForm.ShowDialog();
            }
        }
    }
}
