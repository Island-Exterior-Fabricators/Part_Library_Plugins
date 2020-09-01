using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Part_Library_App_Vault
{
    static class Program
    {
        public static IntPtr app_hwnd;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // the Revit plugin's Window handle is passed here in the command line argument
                //app_hwnd = new IntPtr(int.Parse(args[0]));

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                DialogResult result;
                using (LoginForm lf = new LoginForm())
                    result = lf.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Application.Run(new LibraryForm());
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
