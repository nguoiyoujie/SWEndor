using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWEndor.UI.Forms
{
  public partial class GameForm : Form
  {
    internal GameForm(Engine engine)
    {
      Engine = engine;
      InitializeComponent();
    }

    private readonly Engine Engine;

    private void GameForm_Load(object sender, EventArgs e)
    {
      Width = Settings.ResolutionX;
      Height = Settings.ResolutionY;
      CenterToScreen();

      // game
      Engine.Game.StartLoad();
      Engine.LinkForm(this);
      Engine.LinkHandle(pbGame.Handle);
      Engine.InitTrueVision();
      Engine.Game.Run();
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
