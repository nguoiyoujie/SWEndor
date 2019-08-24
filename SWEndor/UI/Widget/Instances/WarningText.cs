using MTV3D65;
using SWEndor.Actors;
using SWEndor.Primitives;

namespace SWEndor.UI.Widgets
{
  public class WarningText : Widget
  {
    public WarningText(Screen2D owner) : base(owner, "warning") { }

    public override bool Visible
    {
      get
      {
        return !Owner.ShowPage
            && Owner.ShowUI;
      }
    }

    public bool Check(Engine engine, ActorInfo a)
    {
      if (a.TypeInfo.Mask == ComponentMask.GUIDED_PROJECTILE)
        if (a.CurrentAction is AI.Actions.ProjectileAttackActor)
          if (((AI.Actions.ProjectileAttackActor)a.CurrentAction).Target_Actor == PlayerInfo.Actor)
          {
            warn = true;
            dist = ActorDistanceInfo.GetDistance(PlayerInfo.Actor, a);
            return false;
          }

      return true;
    }

    bool warn = false;
    float dist = 0;
    public override void Draw()
    {
      // missile warning?
      warn = false;
      Engine.ActorFactory.DoUntil(Check);

      if (!warn)
        return;

      string text = "MISSILE WARNING [{0:0000.00}]".F(dist);
      int fntID = FontFactory.Get(Font.T12).ID;
      int colorint = Engine.Game.GameTime % 2 > 1 ? new TV_COLOR(1, 0.2f, 0.2f, 1).GetIntColor() : new TV_COLOR(1, 0.8f, 0.2f, 1).GetIntColor();

      float letter_width = 4.5f;

      // boxes
      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(Engine.ScreenWidth / 2 - 5 - letter_width * text.Length
                                                         , Engine.ScreenHeight / 2 - 122
                                                         , Engine.ScreenWidth / 2 + 5 + letter_width * text.Length
                                                         , Engine.ScreenHeight / 2 - 98
                                                         , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();
      // text

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(text
                                                            , Engine.ScreenWidth / 2 - letter_width * text.Length
                                                            , Engine.ScreenHeight / 2 - 120
                                                            , colorint
                                                            , fntID);
      TVScreen2DText.Action_EndText();
    }
  }
}
