using MTV3D65;

namespace SWEndor.UI
{


  public class UIWidget_MessageText : UIWidget
  {
    public UIWidget_MessageText() : base("message") { }

    public override bool Visible
    {
      get
      {
        return !Screen2D.Instance().ShowPage
            && Screen2D.Instance().ShowUI;
      }
    }

    public override void Draw()
    {
      TextInfo t1 = Screen2D.Instance().PrimaryText;
      TextInfo t2 = Screen2D.Instance().SecondaryText;
      int fntID = Font.GetFont("Text_12").ID;

      float letter_width = 4.5f;
      float t1_opacity = t1.ExpireTime - Game.Instance().GameTime;
      float t2_opacity = t2.ExpireTime - Game.Instance().GameTime;
      Utilities.Clamp(ref t1_opacity, 0, 1);
      Utilities.Clamp(ref t2_opacity, 0, 1);

      // boxes
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - 5 - letter_width * t1.Text.Length
                                                           , Engine.Instance().ScreenHeight / 2 - 62
                                                           , Engine.Instance().ScreenWidth / 2 + 5 + letter_width * t1.Text.Length
                                                           , Engine.Instance().ScreenHeight / 2 - 38
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t1_opacity).GetIntColor());
      }

      if (t2_opacity > 0 && t1.Text.Length > 0)
      {
        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(Engine.Instance().ScreenWidth / 2 - 5 - letter_width * t2.Text.Length
                                                           , Engine.Instance().ScreenHeight / 2 - 92
                                                           , Engine.Instance().ScreenWidth / 2 + 5 + letter_width * t2.Text.Length
                                                           , Engine.Instance().ScreenHeight / 2 - 68
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t2_opacity).GetIntColor());
      }
      Engine.Instance().TVScreen2DImmediate.Action_End2D();
      // text

      Engine.Instance().TVScreen2DText.Action_BeginText();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        t1.Color.a = t1_opacity;
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(t1.Text
                                                              , Engine.Instance().ScreenWidth / 2 - letter_width * t1.Text.Length
                                                              , Engine.Instance().ScreenHeight / 2 - 60
                                                              , t1.Color.GetIntColor()
                                                              , fntID);
      }

      if (t2_opacity > 0 && t2.Text.Length > 0)
      {
        t2.Color.a = t2_opacity;
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(t2.Text
                                                              , Engine.Instance().ScreenWidth / 2 - letter_width * t2.Text.Length
                                                              , Engine.Instance().ScreenHeight / 2 - 90
                                                              , t2.Color.GetIntColor()
                                                              , fntID);
      }
      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
