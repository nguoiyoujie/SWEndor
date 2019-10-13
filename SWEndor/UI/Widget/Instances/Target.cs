using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.Core;
using SWEndor.Player;

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
      int w = Engine.Surfaces.RS_Target.GetWidth();
      int h = Engine.Surfaces.RS_Target.GetHeight();
      int tex = -1;

      if (!p.TypeInfo.SystemData.AllowSystemDamage || p.GetStatus(SystemPart.SCANNER) == SystemState.ACTIVE)
      {
        if (PlayerInfo.TargetActor != null)
          tex = Engine.Surfaces.RS_Target.GetTexture();
        else
          tex = Engine.Surfaces.RS_Target_Null.GetTexture();
      }
      else if (p.GetStatus(SystemPart.SCANNER) == SystemState.DISABLED)
        tex = Engine.Surfaces.RS_Target_Disabled.GetTexture();
      else if (p.GetStatus(SystemPart.SCANNER) == SystemState.DESTROYED)
      {
        if (Engine.Game.GameTime % 2 > 1)
          tex = Engine.Surfaces.RS_Target_Destroyed.GetTexture();
        else
          tex = Engine.Surfaces.Tex_Target_Destroyed;
      }
      TVScreen2DImmediate.Action_Begin2D();
      if (tex > -1)
      {
        TVScreen2DImmediate.Draw_Texture(tex
                                  , Engine.ScreenWidth / 2 - w / 2
                                  , Engine.ScreenHeight / 2 - h / 2 + dy
                                  , Engine.ScreenWidth / 2 + w / 2
                                  , Engine.ScreenHeight / 2 + h / 2 + dy);
      }
      TVScreen2DImmediate.Action_End2D();
    }
  }
}
