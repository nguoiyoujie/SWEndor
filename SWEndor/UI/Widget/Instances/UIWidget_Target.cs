using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using SWEndor.Sound;
using SWEndor.Weapons;
using System;

namespace SWEndor.UI
{
  public class UIWidget_Target : UIWidget
  {
    private ActorInfo m_target;
    private float m_targetX = 0;
    private float m_targetY = 0;
    private float m_targetSize = 5;
    private float m_targetBigSize = 0;

    public UIWidget_Target() : base("target") { }

    public override bool Visible
    {
      get
      {
        return (!Screen2D.Instance().ShowPage
            && PlayerInfo.Instance().Actor != null
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD
            && PlayerInfo.Instance().Actor.ActorState != ActorState.DYING
            && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
            && Screen2D.Instance().ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      ActorInfo prev_target = m_target;

      PickTarget(!PlayerInfo.Instance().IsTorpedoMode);

      if (m_target == null)
      {
        PlayerInfo.Instance().AimTarget = null;
      }
      else
      {
        float x = 0;
        float y = 0;
        Engine.Instance().TVScreen2DImmediate.Math_3DPointTo2D(m_target.GetPosition(), ref x, ref y);
        float dist = ActorDistanceInfo.GetDistance(p, m_target, 7501);
        float limit = 0.005f * dist;
        if (limit < 50)
          limit = 50;

        if (m_target.CreationState != CreationState.ACTIVE
        || m_target.ActorState == ActorState.DYING
        || m_target.ActorState == ActorState.DEAD
        || !m_target.CombatInfo.IsCombatObject
        || dist > 7500
        || Math.Abs(x - Engine.Instance().ScreenWidth / 2) > limit
        || Math.Abs(y - Engine.Instance().ScreenHeight / 2) > limit
        || (PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) && PlayerInfo.Instance().IsTorpedoMode)
        || !PlayerCameraInfo.Instance().Camera.IsPointVisible(m_target.GetPosition()))
        {
          m_target = null;
          PlayerInfo.Instance().AimTarget = null;
        }
        else
        {
          TV_COLOR acolor = (m_target.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : m_target.Faction.Color;
          string name = m_target.Name;
          Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
          if (PlayerInfo.Instance().IsTorpedoMode)
          {
            if (!PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) && prev_target != m_target)
            {
              SoundManager.Instance().SetSound("button_3");
              m_targetBigSize = 15;
            }
            if (m_targetBigSize > m_targetSize)
            {
              m_targetBigSize -= 25 * Game.Instance().TimeSinceRender;
              Engine.Instance().TVScreen2DImmediate.Draw_Box(x - m_targetBigSize, y - m_targetBigSize, x + m_targetBigSize, y + m_targetBigSize, acolor.GetIntColor());
            }

            WeaponInfo weap;
            int burst = 0;
            p.TypeInfo.InterpretWeapon(p, PlayerInfo.Instance().SecondaryWeapon, out weap, out burst);
            if (weap != null && weap.Ammo > 0)
            {
              Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
            }
            else
            {
              Engine.Instance().TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
            }
          }
          else
          {
            Engine.Instance().TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
          }
          Engine.Instance().TVScreen2DImmediate.Action_End2D();

          Engine.Instance().TVScreen2DText.Action_BeginText();
          Engine.Instance().TVScreen2DText.TextureFont_DrawText(string.Format("{0}\nDamage: {1:0}%"
            , name
            , (int)(100 * (1 - m_target.StrengthFrac))
            )
            , x, y + m_targetSize + 10, acolor.GetIntColor()
            , Font.GetFont("Text_10").ID
            );
          Engine.Instance().TVScreen2DText.Action_EndText();

          PlayerInfo.Instance().AimTarget = PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) ? null : m_target;

          if (PlayerInfo.Instance().Actor.Faction != null && !PlayerInfo.Instance().Actor.Faction.IsAlliedWith(m_target.Faction) && !PlayerInfo.Instance().IsTorpedoMode)
          {
            // Targeting cross
            // Anticipate
            float d = dist / Globals.LaserSpeed; // Laser Speed
            TV_3DVECTOR target = m_target.GetRelativePositionXYZ(0, 0, m_target.MovementInfo.Speed * d);
            Engine.Instance().TVScreen2DImmediate.Math_3DPointTo2D(target, ref x, ref y);

            Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
            Engine.Instance().TVScreen2DImmediate.Draw_Line(x - m_targetSize, y, x + m_targetSize, y, acolor.GetIntColor());
            Engine.Instance().TVScreen2DImmediate.Draw_Line(x, y - m_targetSize, x, y + m_targetSize, acolor.GetIntColor());
            Engine.Instance().TVScreen2DImmediate.Action_End2D();
          }
        }
      }
    }

    private bool PickTarget(bool pick_allies)
    {
      ActorInfo p = PlayerInfo.Instance().Actor;
      bool ret = false;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        // Attempt close enough
        float bestlimit = 9999;
        foreach (ActorInfo a in ActorFactory.Instance().GetHoldingList())
        {
          if (a != null
            && p != a
            && a.CreationState == CreationState.ACTIVE
            && a.ActorState != ActorState.DYING
            && a.ActorState != ActorState.DEAD
            && a.TypeInfo.IsSelectable
            && (pick_allies || !p.Faction.IsAlliedWith(a.Faction))
            && PlayerCameraInfo.Instance().Camera.IsPointVisible(a.GetPosition())
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
              Engine.Instance().TVScreen2DImmediate.Math_3DPointTo2D(a.GetPosition(), ref x, ref y);

              x -= Engine.Instance().ScreenWidth / 2;
              y -= Engine.Instance().ScreenHeight / 2;

              x = Math.Abs(x);
              y = Math.Abs(y);

              if (x < limit && y < limit && x + y < bestlimit)
              {
                bestlimit = x + y;
                m_target = a;
                m_targetX = x;
                m_targetY = y;
                ret = true;
              }
            }
          }
        }
      }
      return ret;
    }
  }
}
