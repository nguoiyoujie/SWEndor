using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWEndor
{
  public partial class GameForm : Form
  {
    private static GameForm _instance;
    public static GameForm Instance()
    {
      if (_instance == null) { _instance = new GameForm(); }
      return _instance;
    }

    private GameForm()
    {
      InitializeComponent();
      lblVersion.Text = Globals.Version;
    }

    private void GameForm_Load(object sender, EventArgs e)
    {
      DesktopLocation = new Point(0, 0);
      Width = Screen.PrimaryScreen.Bounds.Width;
      Height = Screen.PrimaryScreen.Bounds.Height;

      float w_m = Screen.PrimaryScreen.Bounds.Width * 1.0f / Settings.ResolutionX;
      float h_m = Screen.PrimaryScreen.Bounds.Height * 1.0f / Settings.ResolutionY;

      int m = (int)((w_m > h_m) ? h_m : w_m); // use integer for pixel doubling, if any

      pbGame.Width = Settings.ResolutionX;
      pbGame.Height = Settings.ResolutionY;

      // game
      Globals.Engine.Game.StartLoad();
      Globals.Engine.LinkHandle(pbGame.Handle);
      Globals.Engine.InitTrueVision();
      Globals.Engine.Game.Run();

      pbGame.Width = (int)(Settings.ResolutionX * m);
      pbGame.Height = (int)(Settings.ResolutionY * m);
      pbGame.Location = new Point((Width - pbGame.Width) / 2, (Height - pbGame.Height) / 2);
    }

    public void Exit()
    {
      Cursor.Show();
      Invoke(new Action(Close));
    }

    private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Globals.Engine.Game.Close();
    }

    private void pbGame_MouseLeave(object sender, EventArgs e)
    {
      Cursor.Show();
    }

    private void pbGame_MouseEnter(object sender, EventArgs e)
    {
      Cursor.Hide();
    }
  }
}
