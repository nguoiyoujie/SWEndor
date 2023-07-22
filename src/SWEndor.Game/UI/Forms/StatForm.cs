using SWEndor.Game.Core;
using System;
using System.Windows.Forms;

namespace SWEndor.Game.UI.Forms
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
      lblGameFrame.Text = Engine.Game.GameFrame.ToString();
      lblCollisionFrame.Text = Engine.Game.CollisionTickCount.ToString();
      lblAIFrame.Text = Engine.Game.AITickCount.ToString();


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

    private void StatForm_Load(object sender, EventArgs e)
    {
      ucPoolCache1.Init(Engine);
    }
  }
}
