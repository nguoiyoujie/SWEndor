using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class Squad : Widget
  {
    private float m_targetSize = 5;
    private bool enabled;

    public Squad(Screen2D owner) : base(owner, "squad") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        bool ret = (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI
          && Owner.ShowSquad);

        return ret;
      }
    }

    public override void Draw()
    {
      if (enabled != Owner.ShowSquad)
      {
        if (Owner.ShowSquad)
          Engine.Screen2D.MessageSecondaryText("Squad Indicator: ENABLED", 5, new TV_COLOR(0.5f, 0.5f, 1, 1));
        else
          Engine.Screen2D.MessageSecondaryText("Squad Indicator: DISABLED", 5, new TV_COLOR(0.5f, 0.5f, 1, 1));

        enabled = Owner.ShowSquad;
      }

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

        float m2 = m_targetSize + 5;
        if (s == s.Squad.Leader)
        {
          TVScreen2DImmediate.Draw_Circle(sx, sy, m2, 6, scolor.GetIntColor());
        }

        m2 = m_targetSize;
        TVScreen2DImmediate.Draw_Circle(sx, sy, m2, 6, scolor.GetIntColor());
      }
      TVScreen2DImmediate.Action_End2D();
    }
  }
}