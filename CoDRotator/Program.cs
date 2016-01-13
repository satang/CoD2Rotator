using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
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

            var args = Environment.GetCommandLineArgs();

            bool isServer = false;

            if (args != null && args.Length >= 2 && args[1] == "-s")
            {
                isServer = true;
            }

            Application.Run(new Form1(isServer));
        }
    }
}
