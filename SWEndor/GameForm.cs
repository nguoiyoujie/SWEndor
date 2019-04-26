using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWEndor
{
  public partial class GameForm : Form
  {
    internal GameForm(Engine engine)
    {
      Engine = engine;
      InitializeComponent();
      lblVersion.Text = Globals.Version;
    }

    private readonly Engine Engine;

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
      Engine.Game.StartLoad();
      Engine.LinkForm(this);
      Engine.LinkHandle(pbGame.Handle);
      Engine.InitTrueVision();
      Engine.Game.Run();

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
