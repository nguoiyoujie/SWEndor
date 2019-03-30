using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;

namespace SWEndor.UI
{
  public class UIWidget_Steering: UIWidget
  {
    private TV_2DVECTOR steering_position = new TV_2DVECTOR(10, 110);
    private float steering_height = 60;
    private float steering_width = 60;

    public UIWidget_Steering() : base("steering") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(steering_position.x
                                                   , steering_position.y
                                                   , steering_position.x + steering_width
                                                   , steering_position.y + steering_height
                                                   , new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_Box(steering_position.x
                                                   , steering_position.y
                                                   , steering_position.x + steering_width
                                                   , steering_position.y + steering_height
                                                   , pcolor.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_Line(steering_position.x + steering_width / 2
                                                   , steering_position.y
                                                   , steering_position.x + steering_width / 2
                                                   , steering_position.y + steering_height
                                                   , pcolor.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_Line(steering_position.x
                                                   , steering_position.y + steering_height / 2
                                                   , steering_position.x + steering_width
                                                   , steering_position.y + steering_height / 2
                                                   , pcolor.GetIntColor());

      float xfrac = p.MovementInfo.YTurnAngle / p.MovementInfo.MaxTurnRate / 2;
      float yfrac = p.MovementInfo.XTurnAngle / p.MovementInfo.MaxTurnRate / 2;
      float size = 3;

      Engine.Instance().TVScreen2DImmediate.Draw_Box(steering_position.x + steering_width * (xfrac + 0.5f) - size
                                                   , steering_position.y + steering_height * (yfrac + 0.5f) - size
                                                   , steering_position.x + steering_width * (xfrac + 0.5f) + size
                                                   , steering_position.y + steering_height * (yfrac + 0.5f) + size
                                                   , pcolor.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Action_End2D();
    }
  }
}
