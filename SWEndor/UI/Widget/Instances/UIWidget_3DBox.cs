using SWEndor.Actors;
using SWEndor.Actors.Types;

namespace SWEndor.UI
{
  public class UIWidget_3DBox : UIWidget
  {
    public UIWidget_3DBox() : base("3Dbox") { }

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
