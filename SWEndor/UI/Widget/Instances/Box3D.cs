using SWEndor.Actors;

namespace SWEndor.UI.Widgets
{
  public class Box3D : Widget
  {
    public Box3D(Screen2D owner) : base(owner, "box3D") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
            && p != null
            && !p.IsDyingOrDead
            && Owner.ShowUI
            && Owner.Box3D_Enable);
      }
    }

    public override void Draw()
    {
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box3D(Owner.Box3D_min, Owner.Box3D_max, Owner.Box3D_color.Value);
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
    }
  }
}
