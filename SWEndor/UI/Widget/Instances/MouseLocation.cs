using MTV3D65;
using SWEndor.Input;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class MouseLocation : Widget
  {
    public MouseLocation() : base("mouse") { }

    public override bool Visible
    {
      get
      {
        return true;
      }
    }

    public override void Draw()
    {
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      int mX = InputManager.Instance().MOUSE_X;
      int mY = InputManager.Instance().MOUSE_Y;

      TV_COLOR color = new TV_COLOR(1, 1, 1, 0.5f);
      if (PlayerInfo.Instance().Actor != null && PlayerInfo.Instance().Actor.Faction != null)
        color = PlayerInfo.Instance().Actor.Faction.Color;

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(mX - 2, mY - 2, mX + 2, mY + 2, color.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Action_End2D();
    }
  }
}
