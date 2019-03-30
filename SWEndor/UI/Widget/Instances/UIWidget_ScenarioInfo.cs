using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using SWEndor.Scenarios;

namespace SWEndor.UI
{
  public class UIWidget_ScenarioInfo : UIWidget
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

    public UIWidget_ScenarioInfo() : base("sceneinfo") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - infomiddlegap - infowidth_left - 5
                                    , infotop - 5
                                    , Engine.Instance().ScreenWidth / 2 - infomiddlegap + 5
                                    , infotop + infoheight * 4 + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("LIVES: {0,8:0}\nSCORE: {1,8:00000000}\nKILLS: {2,8:0}\nHITS:  {3,8:0}"
      , PlayerInfo.Instance().Lives
      , PlayerInfo.Instance().Score.Score
      , PlayerInfo.Instance().Score.Kills
      , PlayerInfo.Instance().Score.Hits
      )
      , Engine.Instance().ScreenWidth / 2 - infomiddlegap - infowidth_left
      , infotop
      , pcolor.GetIntColor()
      , Font.GetFont("Text_12").ID
      );
      Engine.Instance().TVScreen2DText.Action_EndText();


      if (GameScenarioManager.Instance().Scenario != null)
      {
        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                      , leftinfo_stagetop - 5
                                      , leftinfo_left + leftinfo_stagewidth + 5
                                      , leftinfo_stageheight + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 + infomiddlegap - 5
                                      , infotop - 5
                                      , Engine.Instance().ScreenWidth / 2 + -infomiddlegap + infowidth_right + 5
                                      , infotop + infoheight * 4 + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Action_End2D();

        Engine.Instance().TVScreen2DText.Action_BeginText();
        // Scenario Title, Difficulty
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("{0}: {1}"
          , GameScenarioManager.Instance().Scenario.Name
          , GameScenarioManager.Instance().Scenario.Difficulty
          )
          , leftinfo_left
          , leftinfo_stagetop
          , pcolor.GetIntColor()
          , Font.GetFont("Text_12").ID
          );

        // StageNumber
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("STAGE: {0}"
          , GameScenarioManager.Instance().Scenario.StageNumber
          )
          , Engine.Instance().ScreenWidth / 2 + infomiddlegap
          , infotop
          , pcolor.GetIntColor()
          , Font.GetFont("Text_12").ID
          );

        Engine.Instance().TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Instance().Line1Text
          , Engine.Instance().ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight
          , GameScenarioManager.Instance().Line1Color.GetIntColor()
          , Font.GetFont("Text_12").ID
          );

        Engine.Instance().TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Instance().Line2Text
          , Engine.Instance().ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 2
          , GameScenarioManager.Instance().Line2Color.GetIntColor()
          , Font.GetFont("Text_12").ID
          );

        Engine.Instance().TVScreen2DText.TextureFont_DrawText(GameScenarioManager.Instance().Line3Text
          , Engine.Instance().ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 3
          , GameScenarioManager.Instance().Line3Color.GetIntColor()
          , Font.GetFont("Text_12").ID
          );

        Engine.Instance().TVScreen2DText.Action_EndText();
      }
    }
  }
}
