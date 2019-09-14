using MTV3D65;

namespace SWEndor.UI.Widgets
{
  public class Debug_GeneralInfo : Widget
  {
    public Debug_GeneralInfo(Screen2D owner) : base(owner, "debug_generalinfo") { }

    public override bool Visible
    {
      get
      {
        return false;// Settings.GameDebug;
      }
    }

    public override void Draw()
    {
      TV_2DVECTOR loc = new TV_2DVECTOR(30, 475);

      TVScreen2DText.Action_BeginText();
      TVCamera tvp = Engine.PlayerCameraInfo.Camera;

      TVScreen2DText.TextureFont_DrawText(string.Format("POS: {0}\nROT: {1}\nTime/Loop: {2:0000.0000}\nTime: {3:0000.0000}"
          , Utilities.ToString(tvp.GetPosition())
          , Utilities.ToString(tvp.GetRotation())
          , Engine.Game.TimeSinceRender
          , Engine.Game.GameTime
          )
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

      TVScreen2DText.Action_EndText();
    }
  }
}
