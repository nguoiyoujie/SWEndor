using Primrose;
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
        Globals.PreInit();

        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.SYS_INIT), Application.ProductName, Globals.Version);
        Engine engine = Globals.InitEngine();

        InputFunction.Registry.GenerateDefault();
        engine.Settings.LoadSettings(engine);
        engine.Settings.SaveSettings(engine);

        Application.Run(new UI.Forms.BackgroundForm(engine));

        engine.Dispose();
      }
      catch (Exception ex)
      {
        Log.Fatal(Globals.LogChannel, ex);
        MessageBox.Show(TextLocalization.Get(TextLocalKeys.SYSTEM_INIT_ERROR).F(Globals.LogChannel)
                      , TextLocalization.Get(TextLocalKeys.SYSTEM_TITLE_ERROR).F(Application.ProductName)
                      , MessageBoxButtons.OK);
      }
      finally
      {
        Log.Info(Globals.LogChannel, LogDecorator.GetFormat(LogType.SYS_CLOSE), Application.ProductName);
      }
    }
  }
}
