using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Game.UI;
using SWEndor.Game.UI.Menu;
using SWEndor.Game.UI.Widgets;
using SWEndor.Game.Scenarios;
using SWEndor.Game.Actors.Components;
using SWEndor.Game;

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
    public readonly Textures Textures;

    // Show toggles
    public bool ShowUI = true;
    public ShowSquadMode ShowSquad = ShowSquadMode.NONE;
    public bool ShowStatus = true;
    public bool ShowRadar = true;
    public bool ShowScore = true;

    // PreRender Text
    public readonly LockStringBuilder LoadingText = new LockStringBuilder();

    // Page
    public bool ShowPage = false;
    public Page CurrentPage = null;

    // Message Text
    public TextInfo PrimaryText = TextInfo.Default;
    public TextInfo SecondaryText = TextInfo.Default;
    public TextInfo SystemsText = TextInfo.Default;

    public TopTextInfo Line1 = TopTextInfo.Default;
    public TopTextInfo Line2 = TopTextInfo.Default;
    public TopTextInfo Line3 = TopTextInfo.Default;

    // Radar
    public bool OverrideTargetingRadar = false;
    public string TargetingRadar_text = "";

    // Draw 3D Box info
    public bool Box3D_Enable = false;
    public TV_3DVECTOR Box3D_min = new TV_3DVECTOR();
    public TV_3DVECTOR Box3D_max = new TV_3DVECTOR();
    public COLOR Box3D_color = ColorLocalization.Get(ColorLocalKeys.WHITE);

    private readonly Widget[] m_Widgets;


    internal Screen2D(Engine engine)
    {
      Engine = engine;
      ScreenSize = new TV_2DVECTOR(Engine.ScreenWidth, Engine.ScreenHeight);
      ScreenCenter = ScreenSize / 2;
      Textures = new Textures(this);

      m_Widgets = new Widget[]
        {
          new SideBarsWidget (this),

          new RadarWidget(this),
          new LargeShipSystemsWidget(this),

          //new UIWidget_Score(this), // Disabled
          new AIWidget(this),
          new ScenarioInfoWidget(this),
          new WeaponInfoWidget(this),
          new SteeringWidget (this),
          new CrossHairWidget(this),
          new TargetWidget(this),
          new TargetScannerWidget(this),
          new SquadWidget(this),
          new SystemIndicatorWidget(this),

          new Box3DWidget(this),
          //new Debug_GeneralInfo(this),
          //new Debug_ObjectInfo(this),
          //new Debug_SelectInfo(this),

          new MessageTextWidget(this),
          new WarningTextWidget(this),

          new PageWidget(this),
          new PerfTextWidget(this),
          new TerminalWidget(this),

          new MouseLocationWidget(this),
          new FadeRectangleWidget(this)
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

    public void AppendLoadingText(string text)
    {
      LoadingText.AppendLine(text);
    }

    public void ReplaceLoadingText(string text)
    {
      LoadingText.Clear();
      LoadingText.AppendLine(text);
    }

    public void MessageText(string text, float expiretime, COLOR color, int priority = 0, bool includeInLogs = true)
    {
      MessageText(ref PrimaryText, ref text, expiretime, color, priority);
      if (includeInLogs)
      {
        Engine.GameScenarioManager.Scenario.State.MessageLogs.Add(new MessageLog(Engine.Game.GameTime + expiretime, text, color));
        Engine.PlayerInfo.Score.AddEntry(new ScoreInfo.Entry(Engine.Game.GameTime, ScoreInfo.EntryType.MESSAGE_LOG, text, null, 0, color.Value));
      }
    }

    public void MessageSecondaryText(string text, float expiretime, COLOR color, int priority = 0)
    {
      MessageText(ref SecondaryText, ref text, expiretime, color, priority);
    }

    public void MessageSystemsText(string text, float expiretime, COLOR color, int priority = 0)
    {
      MessageText(ref SystemsText, ref text, expiretime, color, priority);
    }

    private void MessageText(ref TextInfo tinfo, ref string text, float expiretime, COLOR color, int priority)
    {
      if (tinfo.Priority <= priority || tinfo.ExpireTime < Engine.Game.GameTime)
      {
        tinfo.Priority = priority;
        ActorInfo p = Engine.PlayerInfo.Actor;
        if (p != null && !p.IsSystemOperational(SystemPart.COMLINK))
          text = text.Scramble(Engine.Random);

        tinfo.Text = text;
        tinfo.ExpireTime = Engine.Game.GameTime + expiretime;
        tinfo.Color = color;
      }
    }


    public void ClearText()
    {
      PrimaryText = TextInfo.Default;
      SecondaryText = TextInfo.Default;
      SystemsText = TextInfo.Default;
    }
  }
}
