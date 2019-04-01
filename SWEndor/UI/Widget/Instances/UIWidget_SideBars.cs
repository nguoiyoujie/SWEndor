using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Scenarios;
using System;

namespace SWEndor.UI
{
  public class UIWidget_SideBars: UIWidget
  {
    // Right Info
    private TV_2DVECTOR bar_topleft = new TV_2DVECTOR(Engine.Instance().ScreenWidth * 0.85f - 5, 25); //new TV_2DVECTOR(0, -150);
    private float bar_length = Engine.Instance().ScreenWidth * 0.15f;
    private float bar_height = 16;
    private float bar_barheight = 6;

    public UIWidget_SideBars() : base("sidebar") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI
            && Screen2D.Instance().ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        //Health Bar
        DrawSingleBar(0
                      , string.Format("HP [{0}%]", Math.Ceiling(PlayerInfo.Instance().StrengthFrac * 100))
                      , PlayerInfo.Instance().StrengthFrac
                      , PlayerInfo.Instance().HealthColor
                      );

        //Speed Bar
        DrawSingleBar(1
              , string.Format("SPEED ")
              , p.MovementInfo.Speed / p.MovementInfo.MaxSpeed
              , new TV_COLOR(0.7f, 0.8f, 0.4f, 1)
              );

        int barnumber = 2;

        //Allies
        foreach (ActorInfo a in GameScenarioManager.Instance().CriticalAllies.Values)
        {
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , a.StrengthFrac
              , new TV_COLOR(0, 0.8f, 0.6f, 1)
              );
          barnumber++;
        }

        //Enemies
        foreach (ActorInfo a in GameScenarioManager.Instance().CriticalEnemies.Values)
        {
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , a.StrengthFrac
              , new TV_COLOR(1f, 0, 0, 1)
              );
          barnumber++;
        }

        /*
        Engine.Instance().TVScreen2DText.Action_BeginText();
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("TOGGLE: {0} "
            , ((CONST_TV_KEY)InputKeyMap.GetFnKey("g_ui_status_toggle")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
            )
          , bar_topleft.x - 90
          , bar_topleft.y - 15
          , pcolor.GetIntColor()
          , FontID08
          );
        Engine.Instance().TVScreen2DText.Action_EndText();
        */
      }
    }

    private void DrawSingleBar(int barnumber, string text, float barlengthfrac, TV_COLOR color)
    {
      float h = barnumber * 1.2f;
      if (barlengthfrac < 0)
        barlengthfrac = 0;
      else if (barlengthfrac > 1)
        barlengthfrac = 1;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();

      // Background
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x - 120
                                    , bar_topleft.y + bar_height * (h - 0.1f)
                                    , bar_topleft.x + bar_length + 5
                                    , bar_topleft.y + bar_height * (h + 1.1f)
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      // Bar Background
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                          , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                          , bar_topleft.x + bar_length
                                          , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                          , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                                  , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                                  , bar_topleft.x + bar_length * barlengthfrac
                                                  , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                                  , color.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Action_End2D();


      Engine.Instance().TVScreen2DText.Action_BeginText();

      Engine.Instance().TVScreen2DText.TextureFont_DrawText(text
        , bar_topleft.x - 115
        , bar_topleft.y + bar_height * h
        , color.GetIntColor()
        , Font.GetFont("Text_12").ID
        );

      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
