using MTV3D65;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class Debug_GeneralInfo : Widget
  {
    public Debug_GeneralInfo() : base("debug_generalinfo") { }

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

      Globals.Engine.TVScreen2DText.Action_BeginText();
      TVCamera tvp = PlayerCameraInfo.Instance().Camera;

      Globals.Engine.TVScreen2DText.TextureFont_DrawText(string.Format("POS: {0:0.00},{1:0.00},{2:0.00}\nROT: {3:0.00},{4:0.00},{5:0.00}\nTime/Loop: {6:0000.0000}\nTime: {7:0000.0000}",
          tvp.GetPosition().x,
          tvp.GetPosition().y,
          tvp.GetPosition().z,
          tvp.GetRotation().x,
          tvp.GetRotation().y,
          tvp.GetRotation().z,
          Globals.Engine.Game.TimeSinceRender,
          Globals.Engine.Game.GameTime
          )
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor());

      Globals.Engine.TVScreen2DText.Action_EndText();
    }
  }
}
