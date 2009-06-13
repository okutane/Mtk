using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UI
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Win32.AllocConsole();
            Application.Run(new UI2());
        }
    }
}
