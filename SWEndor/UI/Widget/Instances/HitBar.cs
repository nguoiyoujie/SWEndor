using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.UI.Widgets
{
  public class HitBar : Widget
  {
    private float prevstrengthfrac = 0;

    public HitBar(Screen2D owner) : base(owner, "hit") { }

    public override bool Visible
    {
      get
      {
        return (!Owner.ShowPage
            && PlayerInfo.Actor != null
            && PlayerInfo.Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Owner.ShowUI
            && Owner.ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo m_target = Engine.ActorFactory.Get(PlayerInfo.AimTargetID);

      if (m_target == null)
      {
        prevstrengthfrac = 0;
        return;
      }

      TVScreen2DImmediate.Action_Begin2D();
      float barlength = Engine.ScreenWidth * 0.75f - 100;
      TV_COLOR tcolor = (m_target.Faction != null) ? m_target.Faction.Color : new TV_COLOR(1, 0.5f, 0, 1);
      TV_COLOR tpcolor = new TV_COLOR(tcolor.r, tcolor.g, tcolor.b, 0.3f);

      TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.ScreenHeight - 25
                                        , barlength + 50
                                        , Engine.ScreenHeight - 20
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.ScreenHeight - 25
                                        , 50 + barlength * m_target.StrengthFrac
                                        , Engine.ScreenHeight - 20
                                        , tpcolor.GetIntColor());

      TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.ScreenHeight - 25
                                        , 50 + barlength * prevstrengthfrac
                                        , Engine.ScreenHeight - 20
                                        , tcolor.GetIntColor());

      if (prevstrengthfrac == 0)
      {
        prevstrengthfrac = m_target.StrengthFrac;
      }
      else
      {
        prevstrengthfrac = prevstrengthfrac + (m_target.StrengthFrac - prevstrengthfrac) * 0.2f;
      }
      TVScreen2DImmediate.Action_End2D();


      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(m_target.Name
                                        , 65
                                        , Engine.ScreenHeight - 50
                                        , tcolor.GetIntColor()
                                        , Font.Factory.Get("Text_12").ID);
      TVScreen2DText.Action_EndText();
    }
  }
}
