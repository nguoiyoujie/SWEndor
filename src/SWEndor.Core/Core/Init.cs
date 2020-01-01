using Primrose.Primitives.Extensions;
using SWEndor.Input.Functions;
using System;
using System.Windows.Forms;

namespace SWEndor.Core
{
  public static class Init
  {
    public static void Execute()
    {
      try
      {
        Log.Write(Log.DEBUG, LogType.SYS_INIT, Application.ProductName, Globals.Version);

        Globals.PreInit();
        Engine engine = Globals.InitEngine();

        InputFunction.Registry.GenerateDefault();
        Settings.LoadSettings(engine);
        Settings.SaveSettings(engine);

        Application.Run(new UI.Forms.BackgroundForm(engine));

        engine.Dispose();
      }
      catch (Exception ex)
      {
        Log.WriteErr(Log.INITERROR, ex);
        MessageBox.Show(TextLocalization.Get(TextLocalKeys.SYSTEM_INIT_ERROR).F(Log.INITERROR)
                      , TextLocalization.Get(TextLocalKeys.SYSTEM_TITLE_ERROR).F(Application.ProductName)
                      , MessageBoxButtons.OK);
      }
      finally
      {
        Log.Write(Log.DEBUG, LogType.SYS_CLOSE, Application.ProductName);
      }
    }
  }
}
