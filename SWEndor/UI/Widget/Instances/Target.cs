﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.Sound;
using SWEndor.Weapons;
using System;

namespace SWEndor.UI.Widgets
{
  public class Target : Widget
  {
    private ActorInfo m_target;
    private float m_targetX = 0;
    private float m_targetY = 0;
    private float m_targetSize = 5;
    private float m_targetBigSize = 0;

    public Target() : base("target") { }

    public override bool Visible
    {
      get
      {
        return (!Globals.Engine.Screen2D.ShowPage
            && Globals.Engine.PlayerInfo.Actor != null
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DEAD
            && Globals.Engine.PlayerInfo.Actor.ActorState != ActorState.DYING
            && !(Globals.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
            && Globals.Engine.Screen2D.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = Globals.Engine.PlayerInfo.Actor;
      if (p == null || p.CreationState != CreationState.ACTIVE)
        return;

      ActorInfo prev_target = m_target;

      PickTarget(!Globals.Engine.PlayerInfo.IsTorpedoMode);

      if (m_target == null)
      {
        Globals.Engine.PlayerInfo.AimTargetID = -1;
      }
      else
      {
        float x = 0;
        float y = 0;
        Globals.Engine.TVScreen2DImmediate.Math_3DPointTo2D(m_target.GetPosition(), ref x, ref y);
        float dist = ActorDistanceInfo.GetDistance(p.ID, m_target.ID, 7501);
        float limit = 0.005f * dist;
        if (limit < 50)
          limit = 50;

        if (m_target.CreationState != CreationState.ACTIVE
        || m_target.ActorState == ActorState.DYING
        || m_target.ActorState == ActorState.DEAD
        || !m_target.CombatInfo.IsCombatObject
        || dist > 7500
        || Math.Abs(x - Globals.Engine.ScreenWidth / 2) > limit
        || Math.Abs(y - Globals.Engine.ScreenHeight / 2) > limit
        || (Globals.Engine.PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) && Globals.Engine.PlayerInfo.IsTorpedoMode)
        || !PlayerCameraInfo.Instance().Camera.IsPointVisible(m_target.GetPosition()))
        {
          m_target = null;
          Globals.Engine.PlayerInfo.AimTargetID = -1;
        }
        else
        {
          TV_COLOR acolor = (m_target.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : m_target.Faction.Color;
          string name = m_target.Name;
          Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
          if (Globals.Engine.PlayerInfo.IsTorpedoMode)
          {
            if (!Globals.Engine.PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) && prev_target != m_target)
            {
              Globals.Engine.SoundManager.SetSound("button_3");
              m_targetBigSize = 15;
            }
            if (m_targetBigSize > m_targetSize)
            {
              m_targetBigSize -= 25 * Globals.Engine.Game.TimeSinceRender;
              Globals.Engine.TVScreen2DImmediate.Draw_Box(x - m_targetBigSize, y - m_targetBigSize, x + m_targetBigSize, y + m_targetBigSize, acolor.GetIntColor());
            }

            WeaponInfo weap;
            int burst = 0;
            p.TypeInfo.InterpretWeapon(p.ID, Globals.Engine.PlayerInfo.SecondaryWeapon, out weap, out burst);
            if (weap != null && weap.Ammo > 0)
            {
              Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
            }
            else
            {
              Globals.Engine.TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
            }
          }
          else
          {
            Globals.Engine.TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, acolor.GetIntColor());
          }
          Globals.Engine.TVScreen2DImmediate.Action_End2D();

          Globals.Engine.TVScreen2DText.Action_BeginText();
          Globals.Engine.TVScreen2DText.TextureFont_DrawText(string.Format("{0}\nDamage: {1:0}%"
            , name
            , (int)(100 * (1 - m_target.StrengthFrac))
            )
            , x, y + m_targetSize + 10, acolor.GetIntColor()
            , Font.Factory.Get("Text_10").ID
            );
          Globals.Engine.TVScreen2DText.Action_EndText();

          Globals.Engine.PlayerInfo.AimTargetID = Globals.Engine.PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) ? -1 : m_target.ID;

          if (Globals.Engine.PlayerInfo.Actor.Faction != null && !Globals.Engine.PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) && !Globals.Engine.PlayerInfo.IsTorpedoMode)
          {
            // Targeting cross
            // Anticipate
            float d = dist / Globals.LaserSpeed; // Laser Speed
            TV_3DVECTOR target = m_target.GetRelativePositionXYZ(0, 0, m_target.MovementInfo.Speed * d);
            Globals.Engine.TVScreen2DImmediate.Math_3DPointTo2D(target, ref x, ref y);

            Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
            Globals.Engine.TVScreen2DImmediate.Draw_Line(x - m_targetSize, y, x + m_targetSize, y, acolor.GetIntColor());
            Globals.Engine.TVScreen2DImmediate.Draw_Line(x, y - m_targetSize, x, y + m_targetSize, acolor.GetIntColor());
            Globals.Engine.TVScreen2DImmediate.Action_End2D();
          }
        }
      }
    }

    private bool PickTarget(bool pick_allies)
    {
      ActorInfo p = Globals.Engine.PlayerInfo.Actor;
      bool ret = false;

      if (p != null && p.CreationState == CreationState.ACTIVE)
      {
        // Attempt close enough
        float bestlimit = 9999;

        foreach (int actorID in ActorInfo.Factory.GetHoldingList())
        {
          ActorInfo a = ActorInfo.Factory.Get(actorID);
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
            float dist = ActorDistanceInfo.GetDistance(p.ID, actorID, 7501);
            if (dist < 7500)
            {
              float x = 0;
              float y = 0;
              float limit = 0.015f * dist;
              if (limit < 50)
                limit = 50;
              m_targetX = limit;
              m_targetY = limit;
              Globals.Engine.TVScreen2DImmediate.Math_3DPointTo2D(a.GetPosition(), ref x, ref y);

              x -= Globals.Engine.ScreenWidth / 2;
              y -= Globals.Engine.ScreenHeight / 2;

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
