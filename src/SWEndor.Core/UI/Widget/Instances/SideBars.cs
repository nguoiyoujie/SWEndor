﻿using MTV3D65;
using SWEndor.Actors;
using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.UI.Widgets
{
  public class SideBars: Widget
  {
    // Right Info
    private TV_2DVECTOR bar_topleft;
    private float bar_length;
    private float bar_height;
    private float bar_barheight;

    public SideBars(Screen2D owner) : base(owner, "sidebar")
    {
      bar_topleft = new TV_2DVECTOR(Engine.ScreenWidth * 0.85f - 5, 25);
      bar_length = Engine.ScreenWidth * 0.15f;
      bar_height = 16;
      bar_barheight = 6;
    }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI
          && Owner.ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      //Health Bar
      DrawSingleBar(0
                    , "HP [{0}%]".F(Math.Ceiling(PlayerInfo.StrengthFrac * 100))
                    , PlayerInfo.StrengthFrac
                    , PlayerInfo.StrengthColor.Value
                    );

      //Speed Bar
      DrawSingleBar(1
            , "SPEED "
            , p.MoveData.Speed / p.MoveData.MaxSpeed
            , ColorLocalization.Get(ColorLocalKeys.GAME_STAT_SPEED).Value
            );

      int barnumber = 2;

      //Allies
      foreach (ActorInfo a in GameScenarioManager.Scenario.State.CriticalAllies)
      {
        DrawSingleBar(barnumber
            , a.SideBarName.PadRight(12).Remove(11)
            , a.DisplayHP_Frac
            , ColorLocalization.Get(ColorLocalKeys.GAME_STAT_CRITICAL_ALLY).Value
            );
        barnumber++;
      }

      //Enemies
      foreach (ActorInfo a in GameScenarioManager.Scenario.State.CriticalEnemies)
      {
        DrawSingleBar(barnumber
            , a.SideBarName.PadRight(12).Remove(11)
            , a.DisplayHP_Frac
            , ColorLocalization.Get(ColorLocalKeys.GAME_STAT_CRITICAL_ENEMY).Value
            );
        barnumber++;
      }

      /*
      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(string.Format("TOGGLE: {0} "
          , ((CONST_TV_KEY)InputKeyMap.GetFnKey("g_ui_status_toggle")).ToString().Replace("TV_KEY_", "").Replace("-1", "").PadLeft(1)
          )
        , bar_topleft.x - 90
        , bar_topleft.y - 15
        , pcolor.GetIntColor()
        , FontID08
        );
      TVScreen2DText.Action_EndText();
      */
    }

    private void DrawSingleBar(int barnumber, string text, float barlengthfrac, int color)
    {
      float h = barnumber * 1.2f;
      if (barlengthfrac < 0)
        barlengthfrac = 0;
      else if (barlengthfrac > 1)
        barlengthfrac = 1;

      TVScreen2DImmediate.Action_Begin2D();

      int bgcolor = ColorLocalization.Get(ColorLocalKeys.UI_BACKGROUND_DARK).Value;
      // Background
      TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x - 120
                                    , bar_topleft.y + bar_height * (h - 0.1f)
                                    , bar_topleft.x + bar_length + 5
                                    , bar_topleft.y + bar_height * (h + 1.1f)
                                    , bgcolor); 
      TVScreen2DImmediate.Action_End2D();

      // Bar Background
      TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                          , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                          , bar_topleft.x + bar_length
                                          , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                          , bgcolor);

      TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                                  , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                                  , bar_topleft.x + bar_length * barlengthfrac
                                                  , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                                  , color);

      TVScreen2DImmediate.Action_End2D();


      TVScreen2DText.Action_BeginText();

      TVScreen2DText.TextureFont_DrawText(text
        , bar_topleft.x - 115
        , bar_topleft.y + bar_height * h
        , color
        , FontFactory.Get(Font.T12).ID
        );

      TVScreen2DText.Action_EndText();
    }
  }
}
