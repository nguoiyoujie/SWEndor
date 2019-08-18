using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;
using SWEndor.Primitives;

namespace SWEndor.UI.Widgets
{
  public class ScenarioInfo : Widget
  {
    // Left Info
    private float leftinfo_left = 20;
    private float leftinfo_stagetop = 15;
    private float leftinfo_stageheight = 30;
    private float leftinfo_stagewidth = 260;

    // Middle Info
    private float infomiddlegap = 15;
    private float infowidth_left = 160;
    private float infowidth_right = 160;
    private float infoheight = 20;
    private float infotop = 50;

    public ScenarioInfo(Screen2D owner) : base(owner, "sceneinfo") { }

    public override bool Visible
    {
      get
      {
        using (var v = ActorFactory.Get(PlayerInfo.ActorID))
        {
          if (v == null)
            return false;

          ActorInfo p = v.Value;
          return (!Owner.ShowPage
          && !p.StateModel.IsDyingOrDead
          && Owner.ShowUI);
        }
      }
    }

    public override void Draw()
    {
      using (var v = ActorFactory.Get(PlayerInfo.ActorID))
      {
        if (v == null)
          return;

        ActorInfo p = v.Value;
        if (!p.Active)
          return;

        TVScreen2DImmediate.Action_Begin2D();
        TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left - 5
                                      , infotop - 5
                                      , Engine.ScreenWidth / 2 - infomiddlegap + 5
                                      , infotop + infoheight * 4 + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        TVScreen2DImmediate.Action_End2D();

        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        TVScreen2DText.Action_BeginText();
        TVScreen2DText.TextureFont_DrawText(string.Format("LIVES: {0,8:0}\nSCORE: {1,8:00000000}\nKILLS: {2,8:0}\nHITS:  {3,8:0}"
        , PlayerInfo.Lives
        , PlayerInfo.Score.Score
        , PlayerInfo.Score.Kills
        , PlayerInfo.Score.Hits
        )
        , Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left
        , infotop
        , pcolor.GetIntColor()
        , FontFactory.Get(Font.T12).ID
        );
        TVScreen2DText.Action_EndText();


        if (GameScenarioManager.Scenario != null)
        {
          TVScreen2DImmediate.Action_Begin2D();
          TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                        , leftinfo_stagetop - 5
                                        , leftinfo_left + leftinfo_stagewidth + 5
                                        , leftinfo_stageheight + 5
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

          TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 + infomiddlegap - 5
                                        , infotop - 5
                                        , Engine.ScreenWidth / 2 + -infomiddlegap + infowidth_right + 5
                                        , infotop + infoheight * 4 + 5
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
          TVScreen2DImmediate.Action_End2D();

          TVScreen2DText.Action_BeginText();
          // Scenario Title, Difficulty
          TVScreen2DText.TextureFont_DrawText("{0}: {1}".F(GameScenarioManager.Scenario.Name, GameScenarioManager.Scenario.Difficulty)
            , leftinfo_left
            , leftinfo_stagetop
            , pcolor.GetIntColor()
            , FontFactory.Get(Font.T12).ID
            );

          // StageNumber
          TVScreen2DText.TextureFont_DrawText("STAGE: {0}".F(GameScenarioManager.Scenario.StageNumber)
            , Engine.ScreenWidth / 2 + infomiddlegap
            , infotop
            , pcolor.GetIntColor()
            , FontFactory.Get(Font.T12).ID
            );

          TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Line1Text
            , Engine.ScreenWidth / 2 + infomiddlegap
            , infotop + infoheight
            , GameScenarioManager.Line1Color.GetIntColor()
            , FontFactory.Get(Font.T12).ID
            );

          TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Line2Text
            , Engine.ScreenWidth / 2 + infomiddlegap
            , infotop + infoheight * 2
            , GameScenarioManager.Line2Color.GetIntColor()
            , FontFactory.Get(Font.T12).ID
            );

          TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Line3Text
            , Engine.ScreenWidth / 2 + infomiddlegap
            , infotop + infoheight * 3
            , GameScenarioManager.Line3Color.GetIntColor()
            , FontFactory.Get(Font.T12).ID
            );

          TVScreen2DText.Action_EndText();
        }
      }
    }
  }
}
