using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class HitBar : Widget
  {
    private float prevstrengthfrac = 0;

    public HitBar() : base("hit") { }

    public override bool Visible
    {
      get
      {
        return (!Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.PlayerInfo.Actor != null
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Globals.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Globals.Engine.Screen2D.ShowUI
            && Globals.Engine.Screen2D.ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo m_target = ActorInfo.Factory.Get(Globals.Engine.PlayerInfo.AimTargetID);

      if (m_target == null)
      {
        prevstrengthfrac = 0;
        return;
      }

      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      float barlength = Globals.Engine.ScreenWidth * 0.75f - 100;
      TV_COLOR tcolor = (m_target.Faction != null) ? m_target.Faction.Color : new TV_COLOR(1, 0.5f, 0, 1);
      TV_COLOR tpcolor = new TV_COLOR(tcolor.r, tcolor.g, tcolor.b, 0.3f);

      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(50
                                        , Globals.Engine.ScreenHeight - 25
                                        , barlength + 50
                                        , Globals.Engine.ScreenHeight - 20
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(50
                                        , Globals.Engine.ScreenHeight - 25
                                        , 50 + barlength * m_target.StrengthFrac
                                        , Globals.Engine.ScreenHeight - 20
                                        , tpcolor.GetIntColor());

      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(50
                                        , Globals.Engine.ScreenHeight - 25
                                        , 50 + barlength * prevstrengthfrac
                                        , Globals.Engine.ScreenHeight - 20
                                        , tcolor.GetIntColor());

      if (prevstrengthfrac == 0)
      {
        prevstrengthfrac = m_target.StrengthFrac;
      }
      else
      {
        prevstrengthfrac = prevstrengthfrac + (m_target.StrengthFrac - prevstrengthfrac) * 0.2f;
      }
      Globals.Engine.TVScreen2DImmediate.Action_End2D();


      Globals.Engine.TVScreen2DText.Action_BeginText();
      Globals.Engine.TVScreen2DText.TextureFont_DrawText(m_target.Name
                                        , 65
                                        , Globals.Engine.ScreenHeight - 50
                                        , tcolor.GetIntColor()
                                        , Font.Factory.Get("Text_12").ID);
      Globals.Engine.TVScreen2DText.Action_EndText();
    }
  }
}
