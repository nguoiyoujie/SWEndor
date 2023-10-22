﻿using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using SWEndor.Game.Player;
using System;

namespace SWEndor.Game.UI.Widgets
{
  public class TargetScannerWidget : Widget
  {
    private TV_2DVECTOR tgt_center;

    public TargetScannerWidget(Screen2D owner) : base(owner, "targetinfo")
    {
      if (Engine.Settings.IsSmallResolution)
        tgt_center = new TV_2DVECTOR(Engine.ScreenWidth * 0.5f, Engine.ScreenHeight - Engine.Surfaces.Scanner_height * 0.5f - 20);
      else
        tgt_center = new TV_2DVECTOR(Engine.ScreenWidth * 0.5f, Engine.ScreenHeight - Engine.Surfaces.Scanner_height * 0.5f - 50);
    }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      int w = Engine.Surfaces.Scanner_width;
      int h = Engine.Surfaces.Scanner_height;
      int tex = -1;

      if (p.IsSystemOperational(SystemPart.SCANNER))
          tex = (PlayerInfo.TargetActor != null) ? Engine.Surfaces.RS_Scanner.GetTexture() : Engine.Surfaces.RS_Scanner_Null.GetTexture();
      {
        SystemState status = p.GetStatus(SystemPart.SCANNER);
        if (status == SystemState.DAMAGED)
          tex = (Engine.Game.GameTime % 2 > 1) ? Engine.Surfaces.RS_Scanner_Destroyed.GetTexture() : Engine.Surfaces.RS_Scanner_Destroyed_2.GetTexture();
        else if (status == SystemState.DISABLED)
          tex = Engine.Surfaces.RS_Scanner_Null.GetTexture();
      }

      TVScreen2DImmediate.Action_Begin2D();

#if DEBUG
      if (tex <= -1) // sanity check
        throw new InvalidOperationException("Attempted to draw null texture to Screen2D.TargetInfo");
#endif

        TVScreen2DImmediate.Draw_Texture(tex
                                  , tgt_center.x - w / 2
                                  , tgt_center.y - h / 2
                                  , tgt_center.x + w / 2
                                  , tgt_center.y + h / 2);

      TVScreen2DImmediate.Action_End2D();
    }
  }
}
