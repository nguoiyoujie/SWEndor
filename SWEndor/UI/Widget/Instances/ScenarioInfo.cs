using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.Scenarios;

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
        return (!Owner.ShowPage
            && Owner.Engine.PlayerInfo.Actor != null
            && Owner.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Owner.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Owner.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = Owner.Engine.PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(Owner.Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left - 5
                                    , infotop - 5
                                    , Owner.Engine.ScreenWidth / 2 - infomiddlegap + 5
                                    , infotop + infoheight * 4 + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(string.Format("LIVES: {0,8:0}\nSCORE: {1,8:00000000}\nKILLS: {2,8:0}\nHITS:  {3,8:0}"
      , Owner.Engine.PlayerInfo.Lives
      , Owner.Engine.PlayerInfo.Score.Score
      , Owner.Engine.PlayerInfo.Score.Kills
      , Owner.Engine.PlayerInfo.Score.Hits
      )
      , Owner.Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left
      , infotop
      , pcolor.GetIntColor()
      , Font.Factory.Get("Text_12").ID
      );
      TVScreen2DText.Action_EndText();


      if (Owner.Engine.GameScenarioManager.Scenario != null)
      {
        TVScreen2DImmediate.Action_Begin2D();
        TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                      , leftinfo_stagetop - 5
                                      , leftinfo_left + leftinfo_stagewidth + 5
                                      , leftinfo_stageheight + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

        TVScreen2DImmediate.Draw_FilledBox(Owner.Engine.ScreenWidth / 2 + infomiddlegap - 5
                                      , infotop - 5
                                      , Owner.Engine.ScreenWidth / 2 + -infomiddlegap + infowidth_right + 5
                                      , infotop + infoheight * 4 + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        TVScreen2DImmediate.Action_End2D();

        TVScreen2DText.Action_BeginText();
        // Scenario Title, Difficulty
        TVScreen2DText.TextureFont_DrawText(string.Format("{0}: {1}"
          , Owner.Engine.GameScenarioManager.Scenario.Name
          , Owner.Engine.GameScenarioManager.Scenario.Difficulty
          )
          , leftinfo_left
          , leftinfo_stagetop
          , pcolor.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        // StageNumber
        TVScreen2DText.TextureFont_DrawText(string.Format("STAGE: {0}"
          , Owner.Engine.GameScenarioManager.Scenario.StageNumber
          )
          , Owner.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop
          , pcolor.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Engine.GameScenarioManager.Line1Text
          , Owner.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight
          , Owner.Engine.GameScenarioManager.Line1Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Engine.GameScenarioManager.Line2Text
          , Owner.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 2
          , Owner.Engine.GameScenarioManager.Line2Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Engine.GameScenarioManager.Line3Text
          , Owner.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 3
          , Owner.Engine.GameScenarioManager.Line3Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.Action_EndText();
      }
    }
  }
}
