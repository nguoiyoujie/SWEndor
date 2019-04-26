using SWEndor.Input.Functions;
using SWEndor.Log;
using System;
using System.Windows.Forms;

namespace SWEndor
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
      Init();
    }

    static void Init()
    { 
      try
      {
        Globals.PreInit();
        Engine engine = Globals.InitEngine();

        InputFunction.Registry.GenerateDefault();
        Settings.LoadSettings();
        Settings.SaveSettings();

        Application.Run(new GameForm(engine));
      }
      catch (Exception ex)
      {
        string errorfilename = @"initerror.txt";
        Logger.GenerateErrLog(ex, errorfilename);
        MessageBox.Show(string.Format("Fatal Error occurred during initialization. Please see " + errorfilename + " in the /Log folder for the error message."
                      , Application.ProductName + " - Error Encountered!"
                      , MessageBoxButtons.OK));
        return;
      }

      Globals.UnloadDlls();
    }
  }
}
