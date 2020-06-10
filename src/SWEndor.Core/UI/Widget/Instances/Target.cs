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
    private TV_2DVECTOR tgt_center;

    public TargetInfo(Screen2D owner) : base(owner, "targetinfo")
    {
      if (Engine.Settings.IsSmallResolution)
        tgt_center = new TV_2DVECTOR(Engine.ScreenWidth * 0.5f, Engine.ScreenHeight - Engine.Surfaces.Target_height * 0.5f - 20);
      else
        tgt_center = new TV_2DVECTOR(Engine.ScreenWidth * 0.5f, Engine.ScreenHeight - Engine.Surfaces.Target_height * 0.5f - 50);
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
                                  , tgt_center.x - w / 2
                                  , tgt_center.y - h / 2
                                  , tgt_center.x + w / 2
                                  , tgt_center.y + h / 2);

      TVScreen2DImmediate.Action_End2D();
    }
  }
}
