using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.Scenarios;
using System;

namespace SWEndor.UI.Widgets
{
  public class SideBars: Widget
  {
    // Right Info
    private TV_2DVECTOR bar_topleft = new TV_2DVECTOR(Globals.Engine.ScreenWidth * 0.85f - 5, 25); //new TV_2DVECTOR(0, -150);
    private float bar_length = Globals.Engine.ScreenWidth * 0.15f;
    private float bar_height = 16;
    private float bar_barheight = 6;

    public SideBars() : base("sidebar") { }

    public override bool Visible
    {
      get
      {
        return (!Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.PlayerInfo.Actor != null
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Globals.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Globals.Engine.Screen2D.ShowUI
            && Globals.Engine.Screen2D.ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo p = Globals.Engine.PlayerInfo.Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        //Health Bar
        DrawSingleBar(0
                      , string.Format("HP [{0}%]", Math.Ceiling(Globals.Engine.PlayerInfo.StrengthFrac * 100))
                      , Globals.Engine.PlayerInfo.StrengthFrac
                      , Globals.Engine.PlayerInfo.HealthColor
                      );

        //Speed Bar
        DrawSingleBar(1
              , string.Format("SPEED ")
              , p.MovementInfo.Speed / p.MovementInfo.MaxSpeed
              , new TV_COLOR(0.7f, 0.8f, 0.4f, 1)
              );

        int barnumber = 2;

        //Allies
        foreach (ActorInfo a in Globals.Engine.GameScenarioManager.CriticalAllies.Values)
        {
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , a.StrengthFrac
              , new TV_COLOR(0, 0.8f, 0.6f, 1)
              );
          barnumber++;
        }

        //Enemies
        foreach (ActorInfo a in Globals.Engine.GameScenarioManager.CriticalEnemies.Values)
        {
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , a.StrengthFrac
              , new TV_COLOR(1f, 0, 0, 1)
              );
          barnumber++;
        }

        /*
        Globals.Engine.TVScreen2DText.Action_BeginText();
        Globals.Engine.TVScreen2DText.TextureFont_DrawText(string.Format("TOGGLE: {0} "
            , ((CONST_TV_KEY)InputKeyMap.GetFnKey("g_ui_status_toggle")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
            )
          , bar_topleft.x - 90
          , bar_topleft.y - 15
          , pcolor.GetIntColor()
          , FontID08
          );
        Globals.Engine.TVScreen2DText.Action_EndText();
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

      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();

      // Background
      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x - 120
                                    , bar_topleft.y + bar_height * (h - 0.1f)
                                    , bar_topleft.x + bar_length + 5
                                    , bar_topleft.y + bar_height * (h + 1.1f)
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Action_End2D();

      // Bar Background
      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                          , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                          , bar_topleft.x + bar_length
                                          , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                          , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                                  , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                                  , bar_topleft.x + bar_length * barlengthfrac
                                                  , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                                  , color.GetIntColor());

      Globals.Engine.TVScreen2DImmediate.Action_End2D();


      Globals.Engine.TVScreen2DText.Action_BeginText();

      Globals.Engine.TVScreen2DText.TextureFont_DrawText(text
        , bar_topleft.x - 115
        , bar_topleft.y + bar_height * h
        , color.GetIntColor()
        , Font.Factory.Get("Text_12").ID
        );

      Globals.Engine.TVScreen2DText.Action_EndText();
    }
  }
}
