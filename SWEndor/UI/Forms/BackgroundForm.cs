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
      gf.Owner = this;
      gf.Show();
    }

    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams cp = base.CreateParams;
        // turn on WS_EX_TOOLWINDOW style bit, this hides the window from ALT-TAB 
        // https://stackoverflow.com/questions/27561133/prevent-window-from-showing-in-alt-tab?noredirect=1&lq=1
        cp.ExStyle |= 0x80;
        return cp;
      }
    }
  }
}
