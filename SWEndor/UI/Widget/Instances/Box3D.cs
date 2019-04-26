using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.UI.Widgets
{
  public class Box3D : Widget
  {
    public Box3D(Screen2D owner) : base(owner, "box3D") { }

    public override bool Visible
    {
      get
      {
        return (!Owner.ShowPage
            && this.GetEngine().PlayerInfo.Actor != null
            && this.GetEngine().PlayerInfo.Actor.ActorState != ActorState.DEAD
            && this.GetEngine().PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Owner.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI
            && Owner.Box3D_Enable);
      }
    }

    public override void Draw()
    {
      this.GetEngine().TrueVision.TVScreen2DImmediate.Action_Begin2D();
      this.GetEngine().TrueVision.TVScreen2DImmediate.Draw_Box3D(Globals.Engine.Screen2D.Box3D_min, Globals.Engine.Screen2D.Box3D_max, Globals.Engine.Screen2D.Box3D_color.GetIntColor());
      this.GetEngine().TrueVision.TVScreen2DImmediate.Action_End2D();
    }
  }
}
