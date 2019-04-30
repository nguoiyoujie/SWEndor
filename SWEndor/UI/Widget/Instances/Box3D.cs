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
            && PlayerInfo.Actor != null
            && PlayerInfo.Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI
            && Owner.Box3D_Enable);
      }
    }

    public override void Draw()
    {
      Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      Engine.TrueVision.TVScreen2DImmediate.Draw_Box3D(Globals.Engine.Screen2D.Box3D_min, Globals.Engine.Screen2D.Box3D_max, Globals.Engine.Screen2D.Box3D_color.GetIntColor());
      Engine.TrueVision.TVScreen2DImmediate.Action_End2D();
    }
  }
}
