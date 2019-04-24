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

    public ScenarioInfo() : base("sceneinfo") { }

    public override bool Visible
    {
      get
      {
        return (!Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.PlayerInfo.Actor != null
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Globals.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Globals.Engine.Screen2D.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = Globals.Engine.PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left - 5
                                    , infotop - 5
                                    , Globals.Engine.ScreenWidth / 2 - infomiddlegap + 5
                                    , infotop + infoheight * 4 + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Action_End2D();

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Globals.Engine.TVScreen2DText.Action_BeginText();
      Globals.Engine.TVScreen2DText.TextureFont_DrawText(string.Format("LIVES: {0,8:0}\nSCORE: {1,8:00000000}\nKILLS: {2,8:0}\nHITS:  {3,8:0}"
      , Globals.Engine.PlayerInfo.Lives
      , Globals.Engine.PlayerInfo.Score.Score
      , Globals.Engine.PlayerInfo.Score.Kills
      , Globals.Engine.PlayerInfo.Score.Hits
      )
      , Globals.Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left
      , infotop
      , pcolor.GetIntColor()
      , Font.Factory.Get("Text_12").ID
      );
      Globals.Engine.TVScreen2DText.Action_EndText();


      if (Globals.Engine.GameScenarioManager.Scenario != null)
      {
        Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
        Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(leftinfo_left - 5
                                      , leftinfo_stagetop - 5
                                      , leftinfo_left + leftinfo_stagewidth + 5
                                      , leftinfo_stageheight + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

        Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 + infomiddlegap - 5
                                      , infotop - 5
                                      , Globals.Engine.ScreenWidth / 2 + -infomiddlegap + infowidth_right + 5
                                      , infotop + infoheight * 4 + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        Globals.Engine.TVScreen2DImmediate.Action_End2D();

        Globals.Engine.TVScreen2DText.Action_BeginText();
        // Scenario Title, Difficulty
        Globals.Engine.TVScreen2DText.TextureFont_DrawText(string.Format("{0}: {1}"
          , Globals.Engine.GameScenarioManager.Scenario.Name
          , Globals.Engine.GameScenarioManager.Scenario.Difficulty
          )
          , leftinfo_left
          , leftinfo_stagetop
          , pcolor.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        // StageNumber
        Globals.Engine.TVScreen2DText.TextureFont_DrawText(string.Format("STAGE: {0}"
          , Globals.Engine.GameScenarioManager.Scenario.StageNumber
          )
          , Globals.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop
          , pcolor.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        Globals.Engine.TVScreen2DText.TextureFont_DrawText(Globals.Engine.GameScenarioManager.Line1Text
          , Globals.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight
          , Globals.Engine.GameScenarioManager.Line1Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        Globals.Engine.TVScreen2DText.TextureFont_DrawText(Globals.Engine.GameScenarioManager.Line2Text
          , Globals.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 2
          , Globals.Engine.GameScenarioManager.Line2Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        Globals.Engine.TVScreen2DText.TextureFont_DrawText(Globals.Engine.GameScenarioManager.Line3Text
          , Globals.Engine.ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 3
          , Globals.Engine.GameScenarioManager.Line3Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        Globals.Engine.TVScreen2DText.Action_EndText();
      }
    }
  }
}
