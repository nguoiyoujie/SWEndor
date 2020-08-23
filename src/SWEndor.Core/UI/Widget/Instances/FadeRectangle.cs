using MTV3D65;
using SWEndor.UI.Menu;

namespace SWEndor.UI.Widgets
{
  public class FadeRectangle : Widget
  {
    public FadeRectangle(Screen2D owner) : base(owner, "fadeRectangle") { }

    public override bool Visible
    {
      get
      {
        return true;
      }
    }

    public override void Draw()
    {
      float fill = Engine.PlayerCameraInfo.Fade;
      if (fill > 0)
      {
        TVScreen2DImmediate.Action_Begin2D();
        TVScreen2DImmediate.Draw_FilledBox(0
                                      , 0
                                      , Engine.ScreenWidth
                                      , Engine.ScreenHeight
                                      , new TV_COLOR(0, 0, 0, fill).GetIntColor());
        TVScreen2DImmediate.Action_End2D();
      }
    }
  }
}
