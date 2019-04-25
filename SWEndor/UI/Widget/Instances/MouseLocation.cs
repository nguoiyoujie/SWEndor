using MTV3D65;

namespace SWEndor.UI.Widgets
{
  public class MouseLocation : Widget
  {
    public MouseLocation(Screen2D owner) : base(owner, "mouse") { }

    public override bool Visible
    {
      get
      {
        return true;
      }
    }

    public override void Draw()
    {
      TVScreen2DImmediate.Action_Begin2D();
      int mX = Owner.Engine.InputManager.MOUSE_X;
      int mY = Owner.Engine.InputManager.MOUSE_Y;

      TV_COLOR color = new TV_COLOR(1, 1, 1, 0.5f);
      if (Owner.Engine.PlayerInfo.Actor != null && Owner.Engine.PlayerInfo.Actor.Faction != null)
        color = Owner.Engine.PlayerInfo.Actor.Faction.Color;

      TVScreen2DImmediate.Draw_FilledBox(mX - 2, mY - 2, mX + 2, mY + 2, color.GetIntColor());

      TVScreen2DImmediate.Action_End2D();
    }
  }
}
