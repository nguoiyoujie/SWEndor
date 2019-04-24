using MTV3D65;
using SWEndor.Primitives;
using SWEndor.UI;
using SWEndor.UI.Menu;
using SWEndor.UI.Widgets;
using System.Collections.Generic;

namespace SWEndor
{
  public class Screen2D
  {
    // Show toggles
    public bool ShowUI = true;
    public bool ShowStatus = true;
    public bool ShowRadar = true;
    public bool ShowScore = true;

    // PreRender Text
    public List<string> LoadingTextLines = new List<string>(); 

    // Page
    public bool ShowPage = false;
    public Page CurrentPage = null;

    // Message Text
    public TextInfo PrimaryText = new TextInfo();
    public TextInfo SecondaryText = new TextInfo();

    // Radar
    public bool OverrideTargetingRadar = false;
    public string TargetingRadar_text = "";

    // Draw 3D Box info
    public bool Box3D_Enable = false;
    public TV_3DVECTOR Box3D_min = new TV_3DVECTOR();
    public TV_3DVECTOR Box3D_max = new TV_3DVECTOR();
    public TV_COLOR Box3D_color = new TV_COLOR(1, 1, 1, 1);

    ThreadSafeList<Widget> m_Widgets = new ThreadSafeList<Widget>();

    internal Screen2D()
    {
      m_Widgets.Add(new SideBars());
      m_Widgets.Add(new HitBar());

      m_Widgets.Add(new Radar());

      //m_Widgets.Add(new UIWidget_Score()); // Disabled
      m_Widgets.Add(new AIInfo());
      m_Widgets.Add(new ScenarioInfo());
      m_Widgets.Add(new WidgetWeaponInfo());
      m_Widgets.Add(new Steering());
      m_Widgets.Add(new CrossHair());
      m_Widgets.Add(new Target());

      m_Widgets.Add(new Box3D());
      //m_Widgets.Add(new Debug_GeneralInfo());
      //m_Widgets.Add(new Debug_ObjectInfo());
      //m_Widgets.Add(new Debug_SelectInfo());

      m_Widgets.Add(new MessageText());

      m_Widgets.Add(new WidgetPage());
      m_Widgets.Add(new PerfText());
      m_Widgets.Add(new WidgetTerminal());

      m_Widgets.Add(new MouseLocation());
    }

    public void Draw()
    {
      foreach (Widget w in m_Widgets.GetList())
      {
        if (w != null && w.Visible)
          using (new PerfElement("render_" + w.Name))
            w.Draw();
      }
    }

    public void MessageText(string text, float expiretime, TV_COLOR color, int priority = 0)
    {
      if (PrimaryText.Priority <= priority || PrimaryText.ExpireTime < Globals.Engine.Game.GameTime)
      {
        PrimaryText.Priority = priority;
        PrimaryText.Text = text;
        PrimaryText.ExpireTime = Globals.Engine.Game.GameTime + expiretime;
        PrimaryText.Color = color;
      }
    }

    public void MessageSecondaryText(string text, float expiretime, TV_COLOR color, int priority = 0)
    {
      if (SecondaryText.Priority <= priority || SecondaryText.ExpireTime < Globals.Engine.Game.GameTime)
      {
        SecondaryText.Priority = priority;
        SecondaryText.Text = text;
        SecondaryText.ExpireTime = Globals.Engine.Game.GameTime + expiretime;
        SecondaryText.Color = color;
      }
    }

    public void ClearText()
    {
      PrimaryText.Text = "";
      SecondaryText.Text = "";
    }
  }
}
