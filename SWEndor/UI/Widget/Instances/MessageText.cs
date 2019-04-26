﻿using MTV3D65;

namespace SWEndor.UI.Widgets
{
  public class MessageText : Widget
  {
    public MessageText(Screen2D owner) : base(owner, "message") { }

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
      int fntID = Font.Factory.Get("Text_12").ID;

      float letter_width = 4.5f;
      float t1_opacity = t1.ExpireTime - this.GetEngine().Game.GameTime;
      float t2_opacity = t2.ExpireTime - this.GetEngine().Game.GameTime;
      Utilities.Clamp(ref t1_opacity, 0, 1);
      Utilities.Clamp(ref t2_opacity, 0, 1);

      // boxes
      TVScreen2DImmediate.Action_Begin2D();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.Engine.ScreenWidth / 2 - 5 - letter_width * t1.Text.Length
                                                           , this.GetEngine().ScreenHeight / 2 - 62
                                                           , this.GetEngine().ScreenWidth / 2 + 5 + letter_width * t1.Text.Length
                                                           , this.GetEngine().ScreenHeight / 2 - 38
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t1_opacity).GetIntColor());
      }

      if (t2_opacity > 0 && t1.Text.Length > 0)
      {
        TVScreen2DImmediate.Draw_FilledBox(Owner.Engine.ScreenWidth / 2 - 5 - letter_width * t2.Text.Length
                                                           , this.GetEngine().ScreenHeight / 2 - 92
                                                           , this.GetEngine().ScreenWidth / 2 + 5 + letter_width * t2.Text.Length
                                                           , this.GetEngine().ScreenHeight / 2 - 68
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t2_opacity).GetIntColor());
      }
      TVScreen2DImmediate.Action_End2D();
      // text

      TVScreen2DText.Action_BeginText();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        t1.Color.a = t1_opacity;
        TVScreen2DText.TextureFont_DrawText(t1.Text
                                                              , this.GetEngine().ScreenWidth / 2 - letter_width * t1.Text.Length
                                                              , this.GetEngine().ScreenHeight / 2 - 60
                                                              , t1.Color.GetIntColor()
                                                              , fntID);
      }

      if (t2_opacity > 0 && t2.Text.Length > 0)
      {
        t2.Color.a = t2_opacity;
        TVScreen2DText.TextureFont_DrawText(t2.Text
                                                              , this.GetEngine().ScreenWidth / 2 - letter_width * t2.Text.Length
                                                              , this.GetEngine().ScreenHeight / 2 - 90
                                                              , t2.Color.GetIntColor()
                                                              , fntID);
      }
      TVScreen2DText.Action_EndText();
    }
  }
}
