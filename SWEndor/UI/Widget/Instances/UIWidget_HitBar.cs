using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;

namespace SWEndor.UI
{
  public class UIWidget_HitBar : UIWidget
  {
    private float prevstrengthfrac = 0;

    public UIWidget_HitBar() : base("hit") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI
            && Screen2D.Instance().ShowStatus);
      }
    }

    public override void Draw()
    {
      ActorInfo m_target = PlayerInfo.Instance().AimTarget;

      if (m_target == null)
      {
        prevstrengthfrac = 0;
        return;
      }

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      float barlength = Engine.Instance().ScreenWidth * 0.75f - 100;
      TV_COLOR tcolor = (m_target.Faction != null) ? m_target.Faction.Color : new TV_COLOR(1, 0.5f, 0, 1);
      TV_COLOR tpcolor = new TV_COLOR(tcolor.r, tcolor.g, tcolor.b, 0.3f);

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.Instance().ScreenHeight - 25
                                        , barlength + 50
                                        , Engine.Instance().ScreenHeight - 20
                                        , new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.Instance().ScreenHeight - 25
                                        , 50 + barlength * m_target.StrengthFrac
                                        , Engine.Instance().ScreenHeight - 20
                                        , tpcolor.GetIntColor());

      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(50
                                        , Engine.Instance().ScreenHeight - 25
                                        , 50 + barlength * prevstrengthfrac
                                        , Engine.Instance().ScreenHeight - 20
                                        , tcolor.GetIntColor());

      if (prevstrengthfrac == 0)
      {
        prevstrengthfrac = m_target.StrengthFrac;
      }
      else
      {
        prevstrengthfrac = prevstrengthfrac + (m_target.StrengthFrac - prevstrengthfrac) * 0.2f;
      }
      Engine.Instance().TVScreen2DImmediate.Action_End2D();


      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(m_target.Name
                                        , 65
                                        , Engine.Instance().ScreenHeight - 50
                                        , tcolor.GetIntColor()
                                        , Font.GetFont("Text_12").ID);
      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
