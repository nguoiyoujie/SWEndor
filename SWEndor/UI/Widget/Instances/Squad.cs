using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class Squad : Widget
  {
    private float m_targetSize = 5;
    private float m_targetSizeDiamond = 4;

    public Squad(Screen2D owner) : base(owner, "squad") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI
          && Owner.ShowSquad);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active || p.Squad == null)
        return;

      TVScreen2DImmediate.Action_Begin2D();
      foreach (ActorInfo s in p.Squad.Members)
      {
        if (!s.Active
          || s.IsDyingOrDead
          || !Engine.ActorDataSet.CombatData[s.dataID].IsCombatObject
          || !PlayerCameraInfo.Camera.IsPointVisible(s.GetGlobalPosition()))
          continue;

        TV_COLOR scolor = s.Faction.Color;
        float sx = 0;
        float sy = 0;
        TVScreen2DImmediate.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);

        float m2 = m_targetSizeDiamond + 5;
        if (s == s.Squad.Leader)
        {
          TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy + m2, scolor.GetIntColor());
          TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy - m2, scolor.GetIntColor());
          TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy + m2, scolor.GetIntColor());
          TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy - m2, scolor.GetIntColor());
        }

        m2 = m_targetSizeDiamond;
        TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy + m2, scolor.GetIntColor());
        TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy - m2, scolor.GetIntColor());
        TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy + m2, scolor.GetIntColor());
        TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy - m2, scolor.GetIntColor());

      }
      TVScreen2DImmediate.Action_End2D();
    }
  }
}