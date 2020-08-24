using MTV3D65;

namespace SWEndor.Game.UI.Widgets
{
  public class MouseLocationWidget : Widget
  {
    public MouseLocationWidget(Screen2D owner) : base(owner, "mouse") { }

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
      int mX = Engine.InputManager.MOUSE_X;
      int mY = Engine.InputManager.MOUSE_Y;

      int color = (PlayerInfo.Exists 
                    ? PlayerInfo.FactionColor 
                    : ColorLocalization.Get(ColorLocalKeys.WHITE)).Value;

      TVScreen2DImmediate.Draw_FilledBox(mX - 2, mY - 2, mX + 2, mY + 2, color);

      TVScreen2DImmediate.Action_End2D();
    }
  }
}
