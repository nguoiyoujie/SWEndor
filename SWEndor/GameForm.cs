using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
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
      this.Width = Settings.ResolutionX;
      this.Height = Settings.ResolutionY;
    }

    private void GameForm_Load(object sender, EventArgs e)
    {
      Cursor.Hide();
      this.Show();
      this.DesktopLocation = new Point(0, 0);
      this.Width = Settings.ResolutionX;
      this.Height = Settings.ResolutionY;
      while (Game.Instance().IsRunning)
      {
        Application.DoEvents();
        Thread.Sleep(1000);
      }
      Cursor.Show();
      Close();
    }

    private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
    {
    }

    private void GameForm_MouseEnter(object sender, EventArgs e)
    {
      //Cursor.Hide();
    }

    private void GameForm_MouseLeave(object sender, EventArgs e)
    {
      //Cursor.Show();
    }
  }
}
