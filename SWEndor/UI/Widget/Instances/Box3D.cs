using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class Box3D : Widget
  {
    public Box3D() : base("box3D") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI
            && Screen2D.Instance().Box3D_Enable);
      }
    }

    public override void Draw()
    {
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_Box3D(Screen2D.Instance().Box3D_min, Screen2D.Instance().Box3D_max, Screen2D.Instance().Box3D_color.GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();
    }
  }
}
