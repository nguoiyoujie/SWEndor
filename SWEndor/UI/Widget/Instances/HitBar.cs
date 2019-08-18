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
        using (var v = ActorFactory.Get(PlayerInfo.ActorID))
        {
          if (v == null)
            return false;

          ActorInfo p = v.Value;
          return (!Owner.ShowPage
          && !p.StateModel.IsDyingOrDead
          && Owner.ShowUI
          && Owner.ShowStatus);
        }
      }
    }

    public override void Draw()
    {
      using (var v = Engine.ActorFactory.Get(PlayerInfo.AimTargetID))
      {
        if (v == null)
          return;

        ActorInfo m_target = v.Value;

        TVScreen2DImmediate.Action_Begin2D();
        float barlength = Engine.ScreenWidth * 0.75f - 100;
        float frac = m_target.Health.Frac;
        float dfrac = m_target.Health.DisplayFrac;
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
}
