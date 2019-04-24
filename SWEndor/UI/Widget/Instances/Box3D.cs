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
        return (!Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.PlayerInfo.Actor != null
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Globals.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Globals.Engine.Screen2D.ShowUI
            && Globals.Engine.Screen2D.Box3D_Enable);
      }
    }

    public override void Draw()
    {
      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      Globals.Engine.TVScreen2DImmediate.Draw_Box3D(Globals.Engine.Screen2D.Box3D_min, Globals.Engine.Screen2D.Box3D_max, Globals.Engine.Screen2D.Box3D_color.GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Action_End2D();
    }
  }
}
