using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.UI.Widgets
{
  public class Steering: Widget
  {
    private TV_2DVECTOR steering_position = new TV_2DVECTOR(10, 110);
    private float steering_height = 60;
    private float steering_width = 60;

    public Steering(Screen2D owner) : base(owner, "steering") { }

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

      TV_COLOR pcolor = (p.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : p.Faction.Color;

      TVScreen2DImmediate.Action_Begin2D();

      TVScreen2DImmediate.Draw_FilledBox(steering_position.x
                                                   , steering_position.y
                                                   , steering_position.x + steering_width
                                                   , steering_position.y + steering_height
                                                   , new TV_COLOR(0, 0, 0, 0.25f).GetIntColor());

      TVScreen2DImmediate.Draw_Box(steering_position.x
                                                   , steering_position.y
                                                   , steering_position.x + steering_width
                                                   , steering_position.y + steering_height
                                                   , pcolor.GetIntColor());

      TVScreen2DImmediate.Draw_Line(steering_position.x + steering_width / 2
                                                   , steering_position.y
                                                   , steering_position.x + steering_width / 2
                                                   , steering_position.y + steering_height
                                                   , pcolor.GetIntColor());

      TVScreen2DImmediate.Draw_Line(steering_position.x
                                                   , steering_position.y + steering_height / 2
                                                   , steering_position.x + steering_width
                                                   , steering_position.y + steering_height / 2
                                                   , pcolor.GetIntColor());

      float xfrac = p.MoveData.YTurnAngle / p.MoveData.MaxTurnRate / 2;
      float yfrac = p.MoveData.XTurnAngle / p.MoveData.MaxTurnRate / 2;
      float size = 3;

      TVScreen2DImmediate.Draw_Box(steering_position.x + steering_width * (xfrac + 0.5f) - size
                                                   , steering_position.y + steering_height * (yfrac + 0.5f) - size
                                                   , steering_position.x + steering_width * (xfrac + 0.5f) + size
                                                   , steering_position.y + steering_height * (yfrac + 0.5f) + size
                                                   , pcolor.GetIntColor());

      TVScreen2DImmediate.Action_End2D();
    }
  }
}
