using MTV3D65;
using Primrose.Primitives.Extensions;

namespace SWEndor.UI.Widgets
{
  public class MessageText : Widget
  {
    private float y0_t1;
    private float y1_t1;
    private float y0_t2;
    private float y1_t2;
    private float y0_t3;
    private float y1_t3;
    private int fntID;
    private Font fnt;
    //private float letter_width;

    public MessageText(Screen2D owner) : base(owner, "message")
    {
      y0_t1 = -60;
      y1_t1 = -40;
      y0_t2 = -90;
      y1_t2 = -70;
      y0_t3 = -120;
      y1_t3 = -100;

      if (Engine.Settings.IsSmallResolution)
        fnt = FontFactory.Get(Font.T10);
      else
        fnt = FontFactory.Get(Font.T12);

      fntID = fnt.ID;
    }

    public override bool Visible
    {
      get
      {
        return !Owner.ShowPage
            && Owner.ShowUI;
      }
    }

    public override void Draw()
    {
      TextInfo t1 = Owner.PrimaryText;
      TextInfo t2 = Owner.SecondaryText;
      TextInfo ts = Owner.SystemsText;

      float t1_opacity = (t1.ExpireTime - Engine.Game.GameTime).Clamp(0, 1);
      float t2_opacity = (t2.ExpireTime - Engine.Game.GameTime).Clamp(0, 1);
      float ts_opacity = (ts.ExpireTime - Engine.Game.GameTime).Clamp(0, 1);
      float w1 = fnt.GetWidth(t1.Text) / 2;
      float w2 = fnt.GetWidth(t2.Text) / 2;
      float ws = fnt.GetWidth(ts.Text) / 2;

      // boxes
      TVScreen2DImmediate.Action_Begin2D();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x - 5 - w1
                                                           , Owner.ScreenCenter.y + y0_t1 - 2
                                                           , Owner.ScreenCenter.x + 5 + w1
                                                           , Owner.ScreenCenter.y + y1_t1 + 2
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t1_opacity).GetIntColor());
      }

      if (t2_opacity > 0 && t2.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x - 5 - w2
                                                           , Owner.ScreenCenter.y + y0_t2 - 2
                                                           , Owner.ScreenCenter.x + 5 + w2
                                                           , Owner.ScreenCenter.y + y1_t2 + 2
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t2_opacity).GetIntColor());
      }

      if (ts_opacity > 0 && ts.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x - 5 - ws
                                                           , Owner.ScreenCenter.y + y0_t3 - 2
                                                           , Owner.ScreenCenter.x + 5 + ws
                                                           , Owner.ScreenCenter.y + y1_t3 + 2
                                                           , new TV_COLOR(0, 0, 0, 0.5f * ts_opacity).GetIntColor());
      }
      TVScreen2DImmediate.Action_End2D();
      // text

      TVScreen2DText.Action_BeginText();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        t1.Color.fA = t1_opacity;
        TVScreen2DText.TextureFont_DrawText(t1.Text
                                                              , Owner.ScreenCenter.x - w1
                                                              , Owner.ScreenCenter.y + y0_t1
                                                              , t1.Color.Value
                                                              , fntID);
      }

      if (t2_opacity > 0 && t2.Text.Length > 0)
      {
        t2.Color.fA = t2_opacity;
        TVScreen2DText.TextureFont_DrawText(t2.Text
                                                              , Owner.ScreenCenter.x - w2
                                                              , Owner.ScreenCenter.y + y0_t2
                                                              , t2.Color.Value
                                                              , fntID);
      }

      if (ts_opacity > 0 && ts.Text.Length > 0)
      {
        ts.Color.fA = ts_opacity;
        TVScreen2DText.TextureFont_DrawText(ts.Text
                                                              , Owner.ScreenCenter.x - ws
                                                              , Owner.ScreenCenter.y + y0_t3
                                                              , ts.Color.Value
                                                              , fntID);
      }
      TVScreen2DText.Action_EndText();
    }
  }
}
