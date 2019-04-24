using MTV3D65;

namespace SWEndor.UI.Widgets
{
  public class MessageText : Widget
  {
    public MessageText() : base("message") { }

    public override bool Visible
    {
      get
      {
        return !Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.Screen2D.ShowUI;
      }
    }

    public override void Draw()
    {
      TextInfo t1 = Globals.Engine.Screen2D.PrimaryText;
      TextInfo t2 = Globals.Engine.Screen2D.SecondaryText;
      int fntID = Font.Factory.Get("Text_12").ID;

      float letter_width = 4.5f;
      float t1_opacity = t1.ExpireTime - Globals.Engine.Game.GameTime;
      float t2_opacity = t2.ExpireTime - Globals.Engine.Game.GameTime;
      Utilities.Clamp(ref t1_opacity, 0, 1);
      Utilities.Clamp(ref t2_opacity, 0, 1);

      // boxes
      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 - 5 - letter_width * t1.Text.Length
                                                           , Globals.Engine.ScreenHeight / 2 - 62
                                                           , Globals.Engine.ScreenWidth / 2 + 5 + letter_width * t1.Text.Length
                                                           , Globals.Engine.ScreenHeight / 2 - 38
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t1_opacity).GetIntColor());
      }

      if (t2_opacity > 0 && t1.Text.Length > 0)
      {
        Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(Globals.Engine.ScreenWidth / 2 - 5 - letter_width * t2.Text.Length
                                                           , Globals.Engine.ScreenHeight / 2 - 92
                                                           , Globals.Engine.ScreenWidth / 2 + 5 + letter_width * t2.Text.Length
                                                           , Globals.Engine.ScreenHeight / 2 - 68
                                                           , new TV_COLOR(0, 0, 0, 0.5f * t2_opacity).GetIntColor());
      }
      Globals.Engine.TVScreen2DImmediate.Action_End2D();
      // text

      Globals.Engine.TVScreen2DText.Action_BeginText();
      if (t1_opacity > 0 && t1.Text.Length > 0)
      {
        t1.Color.a = t1_opacity;
        Globals.Engine.TVScreen2DText.TextureFont_DrawText(t1.Text
                                                              , Globals.Engine.ScreenWidth / 2 - letter_width * t1.Text.Length
                                                              , Globals.Engine.ScreenHeight / 2 - 60
                                                              , t1.Color.GetIntColor()
                                                              , fntID);
      }

      if (t2_opacity > 0 && t2.Text.Length > 0)
      {
        t2.Color.a = t2_opacity;
        Globals.Engine.TVScreen2DText.TextureFont_DrawText(t2.Text
                                                              , Globals.Engine.ScreenWidth / 2 - letter_width * t2.Text.Length
                                                              , Globals.Engine.ScreenHeight / 2 - 90
                                                              , t2.Color.GetIntColor()
                                                              , fntID);
      }
      Globals.Engine.TVScreen2DText.Action_EndText();
    }
  }
}
