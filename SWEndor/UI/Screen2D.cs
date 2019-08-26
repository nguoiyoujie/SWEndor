using MTV3D65;
using SWEndor.UI;
using SWEndor.UI.Menu;
using SWEndor.UI.Widgets;

namespace SWEndor
{
  public class Screen2D
  {
    public enum ShowSquadMode
    {
      NONE = 0,
      SQUAD = 1,
      ALL = 2
    }

    public readonly Engine Engine;

    // Show toggles
    public bool ShowUI = true;
    public ShowSquadMode ShowSquad = ShowSquadMode.NONE;
    public bool ShowStatus = true;
    public bool ShowRadar = true;
    public bool ShowScore = true;

    // PreRender Text
    public System.Collections.Generic.List<string> LoadingTextLines = new System.Collections.Generic.List<string>(); 

    // Page
    public bool ShowPage = false;
    public Page CurrentPage = null;

    // Message Text
    public TextInfo PrimaryText = new TextInfo();
    public TextInfo SecondaryText = new TextInfo();
    public TextInfo SystemsText = new TextInfo();

    // Radar
    public bool OverrideTargetingRadar = false;
    public string TargetingRadar_text = "";

    // Draw 3D Box info
    public bool Box3D_Enable = false;
    public TV_3DVECTOR Box3D_min = new TV_3DVECTOR();
    public TV_3DVECTOR Box3D_max = new TV_3DVECTOR();
    public TV_COLOR Box3D_color = new TV_COLOR(1, 1, 1, 1);

    Primitives.ThreadSafeList<Widget> m_Widgets = new Primitives.ThreadSafeList<Widget>();

    internal Screen2D(Engine engine)
    {
      Engine = engine;
      m_Widgets.Add(new SideBars(this));
      m_Widgets.Add(new HitBar(this));

      m_Widgets.Add(new Radar(this));
      m_Widgets.Add(new LargeShipSystems(this));

      //m_Widgets.Add(new UIWidget_Score(this)); // Disabled
      m_Widgets.Add(new AIInfo(this));
      m_Widgets.Add(new ScenarioInfo(this));
      m_Widgets.Add(new WidgetWeaponInfo(this));
      m_Widgets.Add(new Steering(this));
      m_Widgets.Add(new CrossHair(this));
      m_Widgets.Add(new Target(this));
      m_Widgets.Add(new Squad(this));

      m_Widgets.Add(new Box3D(this));
      //m_Widgets.Add(new Debug_GeneralInfo(this));
      //m_Widgets.Add(new Debug_ObjectInfo(this));
      //m_Widgets.Add(new Debug_SelectInfo(this));

      m_Widgets.Add(new MessageText(this));
      m_Widgets.Add(new WarningText(this));

      m_Widgets.Add(new WidgetPage(this));
      m_Widgets.Add(new PerfText(this));
      m_Widgets.Add(new WidgetTerminal(this));

      m_Widgets.Add(new MouseLocation(this));
    }

    public void Draw()
    {
      foreach (Widget w in m_Widgets.GetList())
      {
        if (w != null && w.Visible)
            w.Draw();
      }
    }

    public void MessageText(string text, float expiretime, TV_COLOR color, int priority = 0)
    {
      if (PrimaryText.Priority <= priority || PrimaryText.ExpireTime < Engine.Game.GameTime)
      {
        PrimaryText.Priority = priority;
        PrimaryText.Text = text;
        PrimaryText.ExpireTime = Engine.Game.GameTime + expiretime;
        PrimaryText.Color = color;
      }
    }

    public void MessageSecondaryText(string text, float expiretime, TV_COLOR color, int priority = 0)
    {
      if (SecondaryText.Priority <= priority || SecondaryText.ExpireTime < Engine.Game.GameTime)
      {
        SecondaryText.Priority = priority;
        SecondaryText.Text = text;
        SecondaryText.ExpireTime = Engine.Game.GameTime + expiretime;
        SecondaryText.Color = color;
      }
    }

    public void MessageSystemsText(string text, float expiretime, TV_COLOR color, int priority = 0)
    {
      if (SystemsText.Priority <= priority || SystemsText.ExpireTime < Engine.Game.GameTime)
      {
        SystemsText.Priority = priority;
        SystemsText.Text = text;
        SystemsText.ExpireTime = Engine.Game.GameTime + expiretime;
        SystemsText.Color = color;
      }
    }

    public void ClearText()
    {
      PrimaryText.Text = "";
      SecondaryText.Text = "";
    }
  }
}
