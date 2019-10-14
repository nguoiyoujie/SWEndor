using MTV3D65;
using SWEndor.Primitives.Extensions;

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
    private float letter_width;

    public MessageText(Screen2D owner) : base(owner, "message")
    {
      y0_t1 = -60;
      y1_t1 = -40;
      y0_t2 = -90;
      y1_t2 = -70;
      y0_t3 = -120;
      y1_t3 = -100;

      fntID = FontFactory.Get(Font.T12).ID;
      letter_width = 4.5f;
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

      // boxes
      TVScreen2DImmediate.Action_Begin2D();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x - 5 - letter_width * t1.Text.Length
                                                           , Owner.ScreenCenter.y + y0_t1 - 2
                                                           , Owner.ScreenCenter.x + 5 + letter_width * t1.Text.Length
                                                           , Owner.ScreenCenter.y + y1_t1 + 2
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t1_opacity).GetIntColor());
      }

      if (t2_opacity > 0 && t2.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x - 5 - letter_width * t2.Text.Length
                                                           , Owner.ScreenCenter.y + y0_t2 - 2
                                                           , Owner.ScreenCenter.x + 5 + letter_width * t2.Text.Length
                                                           , Owner.ScreenCenter.y + y1_t2 + 2
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t2_opacity).GetIntColor());
      }

      if (ts_opacity > 0 && ts.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.ScreenCenter.x - 5 - letter_width * ts.Text.Length
                                                           , Owner.ScreenCenter.y + y0_t3 - 2
                                                           , Owner.ScreenCenter.x + 5 + letter_width * ts.Text.Length
                                                           , Owner.ScreenCenter.y + y1_t3 + 2
                                                           , new TV_COLOR(0, 0, 0, 0.5f * ts_opacity).GetIntColor());
      }
      TVScreen2DImmediate.Action_End2D();
      // text

      TVScreen2DText.Action_BeginText();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        TVScreen2DText.TextureFont_DrawText(t1.Text
                                                              , Owner.ScreenCenter.x - letter_width * t1.Text.Length
                                                              , Owner.ScreenCenter.y + y0_t1
                                                              , t1.Color.SetA(t1_opacity)
                                                              , fntID);
      }

      if (t2_opacity > 0 && t2.Text.Length > 0)
      {
        TVScreen2DText.TextureFont_DrawText(t2.Text
                                                              , Owner.ScreenCenter.x - letter_width * t2.Text.Length
                                                              , Owner.ScreenCenter.y + y0_t2
                                                              , t2.Color.SetA(t2_opacity)
                                                              , fntID);
      }

      if (ts_opacity > 0 && ts.Text.Length > 0)
      {
        TVScreen2DText.TextureFont_DrawText(ts.Text
                                                              , Owner.ScreenCenter.x - letter_width * ts.Text.Length
                                                              , Owner.ScreenCenter.y + y0_t3
                                                              , ts.Color.SetA(ts_opacity)
                                                              , fntID);
      }
      TVScreen2DText.Action_EndText();
    }
  }
}
