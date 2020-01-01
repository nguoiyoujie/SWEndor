using SWEndor.Core;
using System;
using System.Windows.Forms;

namespace SWEndor
{
  static class Program
  {
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Init.Execute();
    }
  }
}
