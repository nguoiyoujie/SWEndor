using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.UI.Widgets
{
  public class HitBar : Widget
  {
    public HitBar(Screen2D owner) : base(owner, "hit") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI
          && Owner.ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo m_target = Engine.ActorFactory.Get(PlayerInfo.TargetActorID);

      if (m_target == null)
        return;

      TVScreen2DImmediate.Action_Begin2D();
      float barlength = Engine.ScreenWidth * 0.75f - 100;
      float frac = m_target.HP_Frac;
      float dfrac = m_target.DisplayHP_Frac;
      TV_COLOR tcolor = (m_target.Faction != null) ? m_target.Faction.Color : new TV_COLOR(1, 0.5f, 0, 1);
      TV_COLOR tpcolor = new TV_COLOR(tcolor.r, tcolor.g, tcolor.b, 0.3f);

      TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.ScreenHeight - 25
                                        , barlength + 50
                                        , Engine.ScreenHeight - 20
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.ScreenHeight - 25
                                        , 50 + barlength * frac
                                        , Engine.ScreenHeight - 20
                                        , tpcolor.GetIntColor());

      TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.ScreenHeight - 25
                                        , 50 + barlength * dfrac
                                        , Engine.ScreenHeight - 20
                                        , tcolor.GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(m_target.Name
                                        , 65
                                        , Engine.ScreenHeight - 50
                                        , tcolor.GetIntColor()
                                        , FontFactory.Get(Font.T12).ID);
      TVScreen2DText.Action_EndText();
    }
  }
}
