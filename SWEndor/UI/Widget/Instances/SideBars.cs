using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
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
        return (!Owner.ShowPage
            && PlayerInfo.Actor != null
            && PlayerInfo.Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI
            && Owner.ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

        //Health Bar
        DrawSingleBar(0
                      , string.Format("HP [{0}%]", Math.Ceiling(PlayerInfo.StrengthFrac * 100))
                      , PlayerInfo.StrengthFrac
                      , PlayerInfo.StrengthColor
                      );

        //Speed Bar
        DrawSingleBar(1
              , string.Format("SPEED ")
              , p.MoveData.Speed / p.MoveData.MaxSpeed
              , new TV_COLOR(0.7f, 0.8f, 0.4f, 1)
              );

        int barnumber = 2;

        //Allies
        foreach (int i in GameScenarioManager.CriticalAllies.Values)
        {
          ActorInfo a = ActorFactory.Get(i);
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , Engine.SysDataSet.StrengthFrac_get(i)
              , new TV_COLOR(0, 0.8f, 0.6f, 1)
              );
          barnumber++;
        }

        //Enemies
        foreach (int i in GameScenarioManager.CriticalEnemies.Values)
        {
          ActorInfo a = ActorFactory.Get(i);
          DrawSingleBar(barnumber
              , a.SideBarName.PadRight(12).Remove(11)
              , Engine.SysDataSet.StrengthFrac_get(i)
              , new TV_COLOR(1f, 0, 0, 1)
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
    }

    private void DrawSingleBar(int barnumber, string text, float barlengthfrac, TV_COLOR color)
    {
      float h = barnumber * 1.2f;
      if (barlengthfrac < 0)
        barlengthfrac = 0;
      else if (barlengthfrac > 1)
        barlengthfrac = 1;

      TVScreen2DImmediate.Action_Begin2D();

      // Background
      TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x - 120
                                    , bar_topleft.y + bar_height * (h - 0.1f)
                                    , bar_topleft.x + bar_length + 5
                                    , bar_topleft.y + bar_height * (h + 1.1f)
                                    , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      // Bar Background
      TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                          , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                          , bar_topleft.x + bar_length
                                          , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                          , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      TVScreen2DImmediate.Draw_FilledBox(bar_topleft.x
                                                  , bar_topleft.y + bar_height * (h + 0.6f) - bar_barheight / 2
                                                  , bar_topleft.x + bar_length * barlengthfrac
                                                  , bar_topleft.y + bar_height * (h + 0.6f) + bar_barheight / 2
                                                  , color.GetIntColor());

      TVScreen2DImmediate.Action_End2D();


      TVScreen2DText.Action_BeginText();

      TVScreen2DText.TextureFont_DrawText(text
        , bar_topleft.x - 115
        , bar_topleft.y + bar_height * h
        , color.GetIntColor()
        , FontFactory.Get(Font.T12).ID
        );

      TVScreen2DText.Action_EndText();
    }
  }
}
