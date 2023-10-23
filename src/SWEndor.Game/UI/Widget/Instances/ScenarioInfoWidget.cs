using MTV3D65;
using SWEndor.Game.Actors;

namespace SWEndor.Game.UI.Widgets
{
  public class ScenarioInfoWidget : Widget
  {
    // Left Info
    private readonly float leftinfo_left = 20;
    private readonly float leftinfo_stagetop = 25;
    private readonly float leftinfo_stageheight = 20;
    private readonly float leftinfo_stagewidth = 60;

    // Middle Info
    private readonly float infomiddlegap = 15;
    private readonly float infowidth_left = 160;
    private readonly float infowidth_right = 160;
    private readonly float infoheight = 20;
    private readonly float infotop = 50;

    public ScenarioInfoWidget(Screen2D owner) : base(owner, "sceneinfo") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left - 5
                                    , infotop - 5
                                    , Engine.ScreenWidth / 2 - infomiddlegap + 5
                                    , infotop + infoheight * 4 + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      int icolor = pcolor.Value;

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(LookUpString.GetMetricDisplay(PlayerInfo.Lives, PlayerInfo.Score.Score, PlayerInfo.Score.Kills, PlayerInfo.Score.Hits)
      , Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left
      , infotop
      , icolor
      , FontFactory.Get(Font.T12).ID
      );
      TVScreen2DText.Action_EndText();


      if (GameScenarioManager.Scenario != null)
      {
        TVScreen2DImmediate.Action_Begin2D();
        TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                      , leftinfo_stagetop - 2
                                      , leftinfo_left + leftinfo_stagewidth + 5
                                      , leftinfo_stagetop + leftinfo_stageheight + 2
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

        TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 + infomiddlegap - 5
                                      , infotop - 5
                                      , Engine.ScreenWidth / 2 + -infomiddlegap + infowidth_right + 5
                                      , infotop + infoheight * 4 + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        TVScreen2DImmediate.Action_End2D();

        TVScreen2DText.Action_BeginText();
        // Scenario Title, Difficulty
        TVScreen2DText.TextureFont_DrawText(LookUpString.GetTimeDisplay(Engine.Game.GameTime)
          , leftinfo_left
          , leftinfo_stagetop
          , icolor
          , FontFactory.Get(Font.T12).ID
          );

        // StageNumber
        TVScreen2DText.TextureFont_DrawText(LookUpString.GetStageDisplay(GameScenarioManager.Scenario.State.StageNumber)
          , Engine.ScreenWidth / 2 + infomiddlegap
          , infotop
          , icolor
          , FontFactory.Get(Font.T12).ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Line1.Text
          , Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight
          , Owner.Line1.Color.Value
          , FontFactory.Get(Font.T12).ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Line2.Text
          , Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 2
          , Owner.Line2.Color.Value
          , FontFactory.Get(Font.T12).ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Line3.Text
          , Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 3
          , Owner.Line3.Color.Value
          , FontFactory.Get(Font.T12).ID
          );

        TVScreen2DText.Action_EndText();
      }
    }
  }
}
