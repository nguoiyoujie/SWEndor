using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.Core;
using SWEndor.Player;
using SWEndor.Primitives.Extensions;
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
    public readonly TV_2DVECTOR ScreenSize;
    public readonly TV_2DVECTOR ScreenCenter;

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

    Widget[] m_Widgets;


    internal Screen2D(Engine engine)
    {
      Engine = engine;
      ScreenSize = new TV_2DVECTOR(Engine.ScreenWidth, Engine.ScreenHeight);
      ScreenCenter = ScreenSize / 2;
      m_Widgets = new Widget[]
        {
          new SideBars(this),
          //new HitBar(this),

          new Radar(this),
          new LargeShipSystems(this),

          //new UIWidget_Score(this), // Disabled
          new AIInfo(this),
          new ScenarioInfo(this),
          new WidgetWeaponInfo(this),
          new Steering(this),
          new CrossHair(this),
          new Target(this),
          new TargetInfo(this),
          new Squad(this),
          new SystemIndicator(this),

          new Box3D(this),
          //new Debug_GeneralInfo(this),
          //new Debug_ObjectInfo(this),
          //new Debug_SelectInfo(this),

          new MessageText(this),
          new WarningText(this),

          new WidgetPage(this),
          new PerfText(this),
          new WidgetTerminal(this),

          new MouseLocation(this)
        };
    }

    public void Draw()
    {
      foreach (Widget w in m_Widgets)
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
        ActorInfo p = Globals.Engine.PlayerInfo.Actor;
        if (p != null && (p.GetStatus(SystemPart.COMLINK) == SystemState.DISABLED || p.GetStatus(SystemPart.COMLINK) == SystemState.DESTROYED))
          text = text.Scramble();

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
        ActorInfo p = Globals.Engine.PlayerInfo.Actor;
        if (p != null && (p.GetStatus(SystemPart.COMLINK) == SystemState.DISABLED || p.GetStatus(SystemPart.COMLINK) == SystemState.DESTROYED))
          text = text.Scramble();

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
        ActorInfo p = Globals.Engine.PlayerInfo.Actor;
        if (p != null && (p.GetStatus(SystemPart.COMLINK) == SystemState.DISABLED || p.GetStatus(SystemPart.COMLINK) == SystemState.DESTROYED))
          text = text.Scramble();

        SystemsText.Text = text;
        SystemsText.ExpireTime = Engine.Game.GameTime + expiretime;
        SystemsText.Color = color;
      }
    }

    public void ClearText()
    {
      PrimaryText.Text = "";
      SecondaryText.Text = "";
      SystemsText.Text = "";
      PrimaryText.ExpireTime = 0;
      SecondaryText.ExpireTime = 0;
      SystemsText.ExpireTime = 0;
    }
  }
}
