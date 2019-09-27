using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;

namespace SWEndor.UI.Widgets
{
  public class Squad : Widget
  {
    private float m_targetSize = 5;
    //private bool enabled;

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
          && Owner.ShowSquad != 0);

        return ret;
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      TVScreen2DImmediate.Action_Begin2D();
      switch (Owner.ShowSquad)
      {
        case Screen2D.ShowSquadMode.SQUAD:
          if (!p.Squad.IsNull)
            foreach (ActorInfo s in p.Squad.Members)
              Draw(p, s);
          break;

        case Screen2D.ShowSquadMode.ALL:
          foreach (ActorInfo s in p.ActorFactory.GetAll())
            Draw(p, s);
          break;
      }
      TVScreen2DImmediate.Action_End2D();

      if (p.Squad.Mission is AI.Squads.Missions.AttackActor)
      {
        ActorInfo tgt = p.ActorFactory.Get(((AI.Squads.Missions.AttackActor)p.Squad.Mission).Target_ActorID);
        if (tgt != null)
        {
          TV_COLOR tcolor = tgt.Faction.Color;
          float tx = 0;
          float ty = 0;
          TVScreen2DImmediate.Math_3DPointTo2D(tgt.GetGlobalPosition(), ref tx, ref ty);
          DrawSquadMissionAttackTarget(tx, ty, m_targetSize, tcolor.GetIntColor());
        }
      }
      else if (p.Squad.Mission is AI.Squads.Missions.AssistActor)
      {
        ActorInfo tgt = p.ActorFactory.Get(((AI.Squads.Missions.AssistActor)p.Squad.Mission).Target_ActorID);
        if (tgt != null)
        {
          TV_COLOR tcolor = tgt.Faction.Color;
          float tx = 0;
          float ty = 0;
          TVScreen2DImmediate.Math_3DPointTo2D(tgt.GetGlobalPosition(), ref tx, ref ty);
          DrawSquadMissionAssistTarget(tx, ty, m_targetSize, tcolor.GetIntColor());
        }
      }
    }

    public void Draw(ActorInfo p, ActorInfo s)
    {
      if (!s.Active
          || s == p
          || s.IsDyingOrDead
          || !s.CombatData.IsCombatObject
          || !PlayerCameraInfo.Camera.IsPointVisible(s.GetGlobalPosition()))
          return;

        TV_COLOR scolor = s.Faction.Color;
        float sx = 0;
        float sy = 0;
        TVScreen2DImmediate.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);

        if (s == s.Squad.Leader)
          DrawSquadLeader(sx, sy, m_targetSize, scolor.GetIntColor());
        else
          DrawSquadMember(sx, sy, m_targetSize, scolor.GetIntColor());
    }

    public void DrawSquadLeader(float x, float y, float iconsize, int color)
    {
      TVScreen2DImmediate.Draw_Circle(x, y, iconsize + 5, 6, color);
      TVScreen2DImmediate.Draw_Circle(x, y, iconsize, 6, color);
    }

    public void DrawSquadMember(float x, float y, float iconsize, int color)
    {
      TVScreen2DImmediate.Draw_Circle(x, y, iconsize, 6, color);
    }

    public void DrawSquadMissionAttackTarget(float x, float y, float iconsize, int color)
    {
      TVScreen2DImmediate.Draw_Circle(x, y, iconsize, 6, color);
      TVScreen2DImmediate.Draw_Line(x - iconsize, y, x + iconsize, y, color);
      TVScreen2DImmediate.Draw_Line(x, y - iconsize, x, y + iconsize, color);
    }

    public void DrawSquadMissionAssistTarget(float x, float y, float iconsize, int color)
    {
      TVScreen2DImmediate.Draw_Triangle(x, y + iconsize, x + iconsize / 2, y, x - iconsize / 2, y, color);
    }
  }
}