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
      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      int mX = Globals.Engine.InputManager.MOUSE_X;
      int mY = Globals.Engine.InputManager.MOUSE_Y;

      TV_COLOR color = new TV_COLOR(1, 1, 1, 0.5f);
      if (Globals.Engine.PlayerInfo.Actor != null && Globals.Engine.PlayerInfo.Actor.Faction != null)
        color = Globals.Engine.PlayerInfo.Actor.Faction.Color;

      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(mX - 2, mY - 2, mX + 2, mY + 2, color.GetIntColor());

      Globals.Engine.TVScreen2DImmediate.Action_End2D();
    }
  }
}
