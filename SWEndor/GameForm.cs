using MTV3D65;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
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
    }

    private void GameForm_Load(object sender, EventArgs e)
    {
      DesktopLocation = new Point(0, 0);
      Width = Settings.ResolutionX;
      Height = Settings.ResolutionY;

      // game
      Game.Instance().StartLoad();
      Engine.Instance().LinkHandle(pbGame.Handle);
      Engine.Instance().Initialize();
      Engine.Instance().InitializeComponents();
      Game.Instance().Run();
    }

    public TV_2DVECTOR GetDisplaySize()
    {
      return new TV_2DVECTOR(pbGame.Width, pbGame.Height);
    }

    public void SetDisplaySize()
    {
      Engine.Instance().SetSize();
    }

    public void Exit()
    {
      Cursor.Show();
      Invoke(new Action(Close));
    }

    private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Game.Instance().Close();
    }

    private void pbGame_MouseLeave(object sender, EventArgs e)
    {
      Cursor.Show();
    }

    private void pbGame_SizeChanged(object sender, EventArgs e)
    {
      SetDisplaySize(); // new TV_2DVECTOR(pbGame.Width, pbGame.Height));
    }

    private void pbGame_MouseEnter(object sender, EventArgs e)
    {
      Cursor.Hide();
    }
  }
}
