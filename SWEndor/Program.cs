using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MTV3D65;

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

      Globals.CheckDirectories();

      try
      {
        InputManager.GenerateKeyMap();
        Settings.LoadSettings();
        Settings.SaveSettings();

        Game.Instance().StartLoad();
        Engine.Instance().LinkForm(GameForm.Instance());
        Engine.Instance().Initialize();
        Engine.Instance().InitializeComponents();
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Error initializing Game: \n{0}\n\n{1}", ex.Message, ex.StackTrace));
        return;
      }

      Game.Instance().Run();
      GameForm.Instance().Show();
    }
  }
}
