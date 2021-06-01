using MTV3D65;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Geometry;
using SWEndor.Game.Actors;
using SWEndor.Game.Models;
using SWEndor.Game.Player;
using SWEndor.Game.UI.Helpers;

namespace SWEndor.Game.UI.Widgets
{
  public class SquadWidget : Widget
  {
    private readonly float m_targetSize = 5;
    //private bool enabled;

    public SquadWidget(Screen2D owner) : base(owner, "squad") { }

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
          foreach (ActorInfo s in p.ActorFactory.GetValues())
            Draw(p, s);
          break;
      }
      TVScreen2DImmediate.Action_End2D();

      if (p.Squad.Mission is AI.Squads.Missions.AttackActor actor)
      {
        ActorInfo tgt = p.ActorFactory.Get(actor.Target_ActorID);
        if (tgt != null)
        {
          COLOR tcolor = tgt.Faction.Color;
          float tx = 0;
          float ty = 0;
          TVScreen2DImmediate.Math_3DPointTo2D(tgt.GetGlobalPosition(), ref tx, ref ty);
          DrawSquadMissionAttackTarget(tx, ty, m_targetSize, tcolor);
        }
      }
      else if (p.Squad.Mission is AI.Squads.Missions.AssistActor actor1)
      {
        ActorInfo tgt = p.ActorFactory.Get(actor1.Target_ActorID);
        if (tgt != null)
        {
          COLOR tcolor = tgt.Faction.Color;
          float tx = 0;
          float ty = 0;
          TVScreen2DImmediate.Math_3DPointTo2D(tgt.GetGlobalPosition(), ref tx, ref ty);
          DrawSquadMissionAssistTarget(tx, ty, m_targetSize, tcolor);
        }
      }
    }

    public void Draw(ActorInfo p, ActorInfo s)
    {
      if (!s.Active
          || s == p
          || s.IsDyingOrDead
          || !s.InCombat
          || !PlayerCameraInfo.Camera.IsPointVisible(s.GetGlobalPosition()))
        return;

      COLOR scolor = s.Faction.Color;
      float sx = 0;
      float sy = 0;
      TVScreen2DImmediate.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);

      if (s.TypeInfo.AIData.TargetType.Has(TargetType.SHIP))
        DrawIcons.DrawShip(TVScreen2DImmediate, Owner, s.GetBoundingBox(false), scolor);
      if (s == s.Squad.Leader)
        DrawIcons.DrawSquadLeader(TVScreen2DImmediate, Owner, s);
      else if (s.TypeInfo.AIData.TargetType.Has(TargetType.ADDON))
        DrawIcons.DrawAddon(TVScreen2DImmediate, Owner, s);
      else if (s.TypeInfo.AIData.TargetType.Has(TargetType.MUNITION))
        DrawIcons.DrawProjectile(TVScreen2DImmediate, Owner, s);
      else
        DrawIcons.DrawSquadMember(TVScreen2DImmediate, Owner, s);
    }

    public void DrawSquadMissionAttackTarget(float x, float y, float iconsize, COLOR color)
    {
      TVScreen2DImmediate.Draw_Circle(x, y, iconsize, 6, color.Value);
      TVScreen2DImmediate.Draw_Line(x - iconsize, y, x + iconsize, y, color.Value);
      TVScreen2DImmediate.Draw_Line(x, y - iconsize, x, y + iconsize, color.Value);
    }

    public void DrawSquadMissionAssistTarget(float x, float y, float iconsize, COLOR color)
    {
      TVScreen2DImmediate.Draw_Triangle(x, y + iconsize, x + iconsize / 2, y, x - iconsize / 2, y, color.Value);
    }

  }
}