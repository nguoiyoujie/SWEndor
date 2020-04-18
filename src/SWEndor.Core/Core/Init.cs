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
        Logger.Log(Logger.DEBUG, LogType.SYS_INIT, Application.ProductName, Globals.Version);

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
        Logger.WriteErr(Logger.INITERROR, ex);
        MessageBox.Show(TextLocalization.Get(TextLocalKeys.SYSTEM_INIT_ERROR).F(Logger.INITERROR)
                      , TextLocalization.Get(TextLocalKeys.SYSTEM_TITLE_ERROR).F(Application.ProductName)
                      , MessageBoxButtons.OK);
      }
      finally
      {
        Logger.Log(Logger.DEBUG, LogType.SYS_CLOSE, Application.ProductName);
      }
    }
  }
}
