﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives.Extensions;
using SWEndor.Sound;

namespace SWEndor.UI.Widgets
{
  public class WarningText : Widget
  {
    public WarningText(Screen2D owner) : base(owner, "warning") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return !Owner.ShowPage
            && Owner.ShowUI;
            //&& p.GetStatus(SystemPart.SCANNER) == SystemState.ACTIVE;
      }
    }

    public bool Check(Engine engine, ActorInfo a)
    {
      if (a.TypeInfo.Mask == ComponentMask.GUIDED_PROJECTILE)
        if (a.CurrentAction is AI.Actions.ProjectileAttackActor)
          if (((AI.Actions.ProjectileAttackActor)a.CurrentAction).Target_Actor != null && ((AI.Actions.ProjectileAttackActor)a.CurrentAction).Target_Actor.TopParent == PlayerInfo.Actor)
          {
            warn++;
            float d = DistanceModel.GetDistance(engine, PlayerInfo.Actor, a);
            if (dist < 0 || dist > d)
              dist = d;
            //return false;
          }

      return true;
    }

    int warn = 0;
    int prev_warn = 0;
    float dist = -1;
    public override void Draw()
    {
      // TO-DO: Move missile detection out of Draw function
      // missile warning?
      warn = 0;
      dist = -1;
      Engine.ActorFactory.DoUntil(Check);

      if (warn == 0)
      {
        prev_warn = 0;
        return;
      }

      if (warn > prev_warn)
        Engine.SoundManager.SetSound(SoundGlobals.MissileAlert);

      prev_warn = warn;

      string text = "MISSILE WARNING x{0} [{1:0000.00}]".F(warn, dist);
      int fntID = FontFactory.Get(Font.T12).ID;
      int icolor = (Engine.Game.GameTime % 2 > 1 
                        ? ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_1) 
                        : ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_MISSILE_WARNING_2)).Value;

      float letter_width = 4.5f;

      // boxes
      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 - 5 - letter_width * text.Length
                                                         , Engine.ScreenHeight / 2 - 152
                                                         , Engine.ScreenWidth / 2 + 5 + letter_width * text.Length
                                                         , Engine.ScreenHeight / 2 - 128
                                                         , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();
      // text

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(text
                                                            , Engine.ScreenWidth / 2 - letter_width * text.Length
                                                            , Engine.ScreenHeight / 2 - 150
                                                            , icolor
                                                            , fntID);
      TVScreen2DText.Action_EndText();
    }
  }
}
