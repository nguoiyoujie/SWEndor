using System;
using System.Drawing;
using System.Windows.Forms;

namespace SWEndor.UI.Forms
{
  public partial class BackgroundForm : Form
  {
    public BackgroundForm(Engine engine)
    {
      Engine = engine;
      InitializeComponent();
      lblVersion.Text = Globals.Version;
    }

    private readonly Engine Engine;

    private void BackgroundForm_Load(object sender, EventArgs e)
    {
      DesktopLocation = new Point(0, 0);
      Width = Screen.PrimaryScreen.Bounds.Width;
      Height = Screen.PrimaryScreen.Bounds.Height;
    }

    private void gameTimer_Tick(object sender, EventArgs e)
    {
      gameTimer.Enabled = false;
      GameForm gf = new GameForm(Engine);
      gf.ShowDialog();

      Close();
    }
  }
}
