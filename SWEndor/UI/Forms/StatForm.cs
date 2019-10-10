using SWEndor.Core;
using System;
using System.Windows.Forms;

namespace SWEndor.UI.Forms
{
  public partial class StatForm : Form
  {
    internal StatForm(Engine engine)
    {
      Engine = engine;
      InitializeComponent();
      TopMost = true;
      tmTick.Start();
    }

    private readonly Engine Engine;

    private void tmTick_Tick(object sender, EventArgs e)
    {
      lblFPS.Text = Engine.Game.CurrentFPS.ToString();
      lblGameTime.Text = Engine.Game.GameTime.ToString();
      lblRenderTime.Text = Engine.Game.TimeSinceRender.ToString();

      if (tabControl1.SelectedTab == tpObjects)
        ucStatObjects1.Update(Engine);
      else if (tabControl1.SelectedTab == tpSound)
        ucSound1.Update(Engine);
      else if(tabControl1.SelectedTab == tpPool)
        ucPoolCache1.Update(Engine);

    }

    private void StatForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      tmTick.Stop();
    }
  }
}
