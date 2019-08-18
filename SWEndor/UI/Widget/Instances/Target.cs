using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Weapons;
using System;

namespace SWEndor.UI.Widgets
{
  public class Target : Widget
  {
    private int m_targetID;
    private float m_targetX = 0;
    private float m_targetY = 0;
    private float m_targetSize = 5;
    private float m_targetBigSize = 0;

    public Target(Screen2D owner) : base(owner, "target") { }

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
          && Owner.ShowUI);
        }
      }
    }

    public override void Draw()
    {
      using (var v = ActorFactory.Get(PlayerInfo.ActorID))
      {
        if (v != null)
        {
          ActorInfo p = v.Value;
          if (p == null || !p.Active)
            return;

          using (var vt = ActorFactory.Get(m_targetID))
          {
            ActorInfo m_target = vt?.Value ?? null;
            ActorInfo prev_target = m_target;

            PickTarget(!PlayerInfo.IsTorpedoMode, p);

            if (m_target == null)
            {
              PlayerInfo.AimTargetID = -1;
            }
            else
            {
              float x = 0;
              float y = 0;
              TVScreen2DImmediate.Math_3DPointTo2D(m_target.GetGlobalPosition(), ref x, ref y);
              float dist = ActorDistanceInfo.GetDistance(p, m_target, 7501);
              float limit = 0.05f * dist;
              if (limit < 250)
                limit = 250;

              if (m_target.Active
              || m_target.StateModel.IsDyingOrDead
              || !m_target.CombatData.IsCombatObject
              || dist > 7500
              || Math.Abs(x - Engine.ScreenWidth / 2) > limit
              || Math.Abs(y - Engine.ScreenHeight / 2) > limit
              || (p.Faction.IsAlliedWith(m_target.Faction) && PlayerInfo.IsTorpedoMode)
              || !PlayerCameraInfo.Camera.IsPointVisible(m_target.GetGlobalPosition()))
              {
                m_targetID = -1;
                PlayerInfo.AimTargetID = -1;
              }
              else
              {
                TV_COLOR acolor = (m_target.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : m_target.Faction.Color;
                string name = m_target.Name;
                TVScreen2DImmediate.Action_Begin2D();
                if (PlayerInfo.IsTorpedoMode)
                {
                  if (!p.Faction.IsAlliedWith(m_target.Faction) && prev_target != m_target)
                  {
                    SoundManager.SetSound("button_3");
                    m_targetBigSize = 15;
                  }
                  if (m_targetBigSize > m_targetSize)
                  {
                    m_targetBigSize -= 25 * Game.TimeSinceRender;
                    TVScreen2DImmediate.Draw_Box(x - m_targetBigSize, y - m_targetBigSize, x + m_targetBigSize, y + m_targetBigSize, acolor.GetIntColor());
                  }

                  WeaponInfo weap;
                  int burst = 0;
                  p.TypeInfo.InterpretWeapon(p, PlayerInfo.SecondaryWeapon, out weap, out burst);
                  if (weap != null && weap.Ammo > 0)
                  {
                    TVScreen2DImmediate.Draw_FilledBox(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
                  }
                  else
                  {
                    TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
                  }
                }
                else
                {
                  TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
                }
                TVScreen2DImmediate.Action_End2D();

                TVScreen2DText.Action_BeginText();
                TVScreen2DText.TextureFont_DrawText("{0}\nDamage: {1:0}%".F(name, (int)(100 * (1 - m_target.Health.Frac)))
                  , x, y + m_targetSize + 10, acolor.GetIntColor()
                  , FontFactory.Get(Font.T10).ID
                  );
                TVScreen2DText.Action_EndText();

                PlayerInfo.AimTargetID = p.Faction.IsAlliedWith(m_target.Faction) ? -1 : m_target.ID;

                if (p.Faction != null && !p.Faction.IsAlliedWith(m_target.Faction) && !PlayerInfo.IsTorpedoMode)
                {
                  // Targeting cross
                  // Anticipate
                  float d = dist / Globals.LaserSpeed; // Laser Speed
                  TV_3DVECTOR target = m_target.GetRelativePositionXYZ(0, 0, m_target.MoveData.Speed * d);
                  TVScreen2DImmediate.Math_3DPointTo2D(target, ref x, ref y);

                  TVScreen2DImmediate.Action_Begin2D();
                  TVScreen2DImmediate.Draw_Line(x - m_targetSize, y, x + m_targetSize, y, acolor.GetIntColor());
                  TVScreen2DImmediate.Draw_Line(x, y - m_targetSize, x, y + m_targetSize, acolor.GetIntColor());
                  TVScreen2DImmediate.Action_End2D();
                }
              }
            }
          }
        }
      }
    }

    private bool PickTarget(bool pick_allies, ActorInfo p)
    {
      bool ret = false;

      if (p != null && p.Active)
      {
        // Attempt close enough
        float bestlimit = 9999;

        Action<Engine, ActorInfo> action = new Action<Engine, ActorInfo>(
          (_, a) =>
          {
            if (a != null
              && p != a
              && a.Active
              && !a.StateModel.IsDyingOrDead
              && a.StateModel.ComponentMask.Has(ComponentMask.CAN_BETARGETED)
              && (pick_allies || !p.Faction.IsAlliedWith(a.Faction))
              && PlayerCameraInfo.Camera.IsPointVisible(a.GetGlobalPosition())
              )
            {
              float dist = ActorDistanceInfo.GetDistance(p, a, 7501);
              if (dist < 7500)
              {
                float x = 0;
                float y = 0;
                float limit = 0.015f * dist;
                if (limit < 50)
                  limit = 50;
                m_targetX = limit;
                m_targetY = limit;
                TVScreen2DImmediate.Math_3DPointTo2D(a.GetGlobalPosition(), ref x, ref y);

                x -= Engine.ScreenWidth / 2;
                y -= Engine.ScreenHeight / 2;

                x = Math.Abs(x);
                y = Math.Abs(y);

                if (x < limit && y < limit && x + y < bestlimit)
                {
                  bestlimit = x + y;
                  m_targetID = a.ID;
                  m_targetX = x;
                  m_targetY = y;
                  ret = true;
                }
              }
            }
          }
        );

        ActorFactory.DoEach(action);
      }
      return ret;
    }
  }
}
