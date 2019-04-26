using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

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
            && this.GetEngine().PlayerInfo.Actor != null
            && this.GetEngine().PlayerInfo.Actor.ActorState != ActorState.DEAD
            && this.GetEngine().PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Owner.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = this.GetEngine().PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(Owner.Engine.ScreenWidth / 2 - infomiddlegap - infowidth_left - 5
                                    , infotop - 5
                                    , this.GetEngine().ScreenWidth / 2 - infomiddlegap + 5
                                    , infotop + infoheight * 4 + 5
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(string.Format("LIVES: {0,8:0}\nSCORE: {1,8:00000000}\nKILLS: {2,8:0}\nHITS:  {3,8:0}"
      , this.GetEngine().PlayerInfo.Lives
      , this.GetEngine().PlayerInfo.Score.Score
      , this.GetEngine().PlayerInfo.Score.Kills
      , this.GetEngine().PlayerInfo.Score.Hits
      )
      , this.GetEngine().ScreenWidth / 2 - infomiddlegap - infowidth_left
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
                                      , this.GetEngine().ScreenWidth / 2 + -infomiddlegap + infowidth_right + 5
                                      , infotop + infoheight * 4 + 5
                                      , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
        TVScreen2DImmediate.Action_End2D();

        TVScreen2DText.Action_BeginText();
        // Scenario Title, Difficulty
        TVScreen2DText.TextureFont_DrawText(string.Format("{0}: {1}"
          , this.GetEngine().GameScenarioManager.Scenario.Name
          , this.GetEngine().GameScenarioManager.Scenario.Difficulty
          )
          , leftinfo_left
          , leftinfo_stagetop
          , pcolor.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        // StageNumber
        TVScreen2DText.TextureFont_DrawText(string.Format("STAGE: {0}"
          , this.GetEngine().GameScenarioManager.Scenario.StageNumber
          )
          , this.GetEngine().ScreenWidth / 2 + infomiddlegap
          , infotop
          , pcolor.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Engine.GameScenarioManager.Line1Text
          , this.GetEngine().ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight
          , this.GetEngine().GameScenarioManager.Line1Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Engine.GameScenarioManager.Line2Text
          , this.GetEngine().ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 2
          , this.GetEngine().GameScenarioManager.Line2Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.TextureFont_DrawText(Owner.Engine.GameScenarioManager.Line3Text
          , this.GetEngine().ScreenWidth / 2 + infomiddlegap
          , infotop + infoheight * 3
          , this.GetEngine().GameScenarioManager.Line3Color.GetIntColor()
          , Font.Factory.Get("Text_12").ID
          );

        TVScreen2DText.Action_EndText();
      }
    }
  }
}
