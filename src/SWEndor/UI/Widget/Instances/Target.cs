using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.Core;
using SWEndor.Player;
using System;

namespace SWEndor.UI.Widgets
{
  public class TargetInfo : Widget
  {
    public TargetInfo(Screen2D owner) : base(owner, "targetinfo") { }

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

      int dy = 250;
      int w = Engine.Surfaces.Target_width;
      int h = Engine.Surfaces.Target_height;
      int tex = -1;

      if (!p.TypeInfo.SystemData.AllowSystemDamage || p.GetStatus(SystemPart.SCANNER) == SystemState.ACTIVE)
          tex = (PlayerInfo.TargetActor != null) ? Engine.Surfaces.RS_Target.GetTexture() : Engine.Surfaces.RS_Target_Null.GetTexture();
      else if (p.GetStatus(SystemPart.SCANNER) == SystemState.DISABLED)
        tex = Engine.Surfaces.RS_Target_Disabled.GetTexture();
      else if (p.GetStatus(SystemPart.SCANNER) == SystemState.DESTROYED)
          tex = (Engine.Game.GameTime % 2 > 1) ? Engine.Surfaces.RS_Target_Destroyed.GetTexture() : Engine.Surfaces.Tex_Target_Destroyed;

      TVScreen2DImmediate.Action_Begin2D();

#if DEBUG
      if (tex <= -1) // sanity check
        throw new InvalidOperationException("Attempted to draw null texture to Screen2D.TargetInfo");
#endif

        TVScreen2DImmediate.Draw_Texture(tex
                                  , Engine.ScreenWidth / 2 - w / 2
                                  , Engine.ScreenHeight / 2 - h / 2 + dy
                                  , Engine.ScreenWidth / 2 + w / 2
                                  , Engine.ScreenHeight / 2 + h / 2 + dy);

      TVScreen2DImmediate.Action_End2D();
    }
  }
}
