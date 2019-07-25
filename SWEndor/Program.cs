using SWEndor.Input.Functions;
using SWEndor.Primitives;
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
        Log.Init();
        Globals.PreInit();
        Engine engine = Globals.InitEngine();

        InputFunction.Registry.GenerateDefault();
        Settings.LoadSettings(engine);
        Settings.SaveSettings(engine);

        Application.Run(new UI.Forms.BackgroundForm(engine));
      }
      catch (Exception ex)
      {
        Log.WriteErr(Log.INITERROR, ex);
        MessageBox.Show(TextLocalization.Get(TextLocalKeys.SYSTEM_INIT_ERROR).F(Log.INITERROR)
                      , TextLocalization.Get(TextLocalKeys.SYSTEM_TITLE_ERROR).F(Application.ProductName)
                      , MessageBoxButtons.OK);
        return;
      }
    }
  }
}
