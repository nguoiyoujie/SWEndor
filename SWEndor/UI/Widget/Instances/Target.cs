using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;
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
    private float m_targetSizeDiamond = 4;
    private float m_targetBigSize = 0;

    public Target(Screen2D owner) : base(owner, "target") { }

    public override bool Visible
    {
      get
      {
        ActorInfo p = PlayerInfo.Actor;
        return (!Owner.ShowPage
          && p != null
          && !p.IsDyingOrDead
          && Owner.ShowUI);
      }
    }

    public override void Draw()
    {
      ActorInfo p = PlayerInfo.Actor;
      if (p == null || !p.Active)
        return;

      ActorInfo prev_target = m_target;

      if (p.TypeInfo is ActorTypes.Groups.LargeShip)
        PickTargetLargeShip();
      else
        PickTargetFighter(!PlayerInfo.IsTorpedoMode);

      if (m_target == null)
      {
        PlayerInfo.AimTargetID = -1;
        PlayerInfo.AssistTargetID = -1;
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

        if (Check(p, x, y, limit, dist, prev_target))
        {
          m_target = null;
          PlayerInfo.AimTargetID = -1;
        }
        else
        {
          Draw(p, x, y, dist, prev_target);
        }
      }
    }

    private bool Check(ActorInfo p, float x, float y, float limit, float dist, ActorInfo prev_target)
    {
      if (p.TypeInfo is ActorTypes.Groups.LargeShip)
      {
        return !m_target.Active
        || m_target.IsDyingOrDead
        || !Engine.ActorDataSet.CombatData[m_target.dataID].IsCombatObject
        || !PlayerCameraInfo.Camera.IsPointVisible(m_target.GetGlobalPosition());
      }
      else
        return !m_target.Active
        || m_target.IsDyingOrDead
        || !Engine.ActorDataSet.CombatData[m_target.dataID].IsCombatObject
        || dist > 7500
        || Math.Abs(x - Engine.ScreenWidth / 2) > limit
        || Math.Abs(y - Engine.ScreenHeight / 2) > limit
        || (PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) && PlayerInfo.IsTorpedoMode)
        || !PlayerCameraInfo.Camera.IsPointVisible(m_target.GetGlobalPosition());
    }

    private void Draw(ActorInfo p, float x, float y, float dist, ActorInfo prev_target)
    {
      TV_COLOR acolor = (m_target.Faction == null) ? new TV_COLOR(1, 1, 1, 1) : m_target.Faction.Color;
      string name = m_target.Name;
      TVScreen2DImmediate.Action_Begin2D();
      if (PlayerInfo.IsTorpedoMode)
      {
        if (!PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) && prev_target != m_target)
        {
          SoundManager.SetSound("button_3");
          m_targetBigSize = 15;
        }
        if (m_targetBigSize > m_targetSize)
        {
          m_targetBigSize -= 25 * Game.TimeSinceRender;
          TVScreen2DImmediate.Draw_Box(x - m_targetBigSize, y - m_targetBigSize, x + m_targetBigSize, y + m_targetBigSize, acolor.GetIntColor());
        }

        WeaponInfo weap = PlayerInfo.SecondaryWeapon.Weapon;
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
      TVScreen2DText.TextureFont_DrawText(string.Format("{0}\nDamage: {1:0}%", name, 100 - m_target.HP_Perc)
        , x, y + m_targetSize + 10, acolor.GetIntColor()
        , FontFactory.Get(Font.T10).ID
        );
      TVScreen2DText.Action_EndText();

      PlayerInfo.AimTargetID = PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) ? -1 : m_target.ID;
      PlayerInfo.AssistTargetID = PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) ? m_target.ID : -1;

      if (!PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) && !PlayerInfo.IsTorpedoMode)
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

      //if (m_target.TypeInfo is ActorTypes.Groups.Fighter)
      //if (PlayerInfo.Actor.Faction.IsAlliedWith(m_target.Faction) && m_target.TypeInfo is ActorTypes.Groups.Fighter)
      //{
      // Squad diamond
      if (!m_target.Squad.IsNull)
      {
        TVScreen2DImmediate.Action_Begin2D();
        foreach (ActorInfo s in m_target.Squad.Members)
        {
          if (s != m_target && PlayerCameraInfo.Camera.IsPointVisible(s.GetGlobalPosition()))
          {
            float sx = 0;
            float sy = 0;
            TVScreen2DImmediate.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);

            float m2 = m_targetSizeDiamond + 5;
            if (s == m_target.Squad.Leader)
            {
              TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy + m2, acolor.GetIntColor());
              TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy - m2, acolor.GetIntColor());
              TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy + m2, acolor.GetIntColor());
              TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy - m2, acolor.GetIntColor());
            }

            m2 = m_targetSizeDiamond;
            TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy + m2, acolor.GetIntColor());
            TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy - m2, acolor.GetIntColor());
            TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy + m2, acolor.GetIntColor());
            TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy - m2, acolor.GetIntColor());
          }
        }
        TVScreen2DImmediate.Action_End2D();
      }
      //}
    }

    private bool PickTargetFighter(bool pick_allies)
    {
      ActorInfo p = PlayerInfo.Actor;
      bool ret = false;

      if (p != null || p.Active)
      {
        // Attempt close enough
        float bestlimit = 9999;

        Action<Engine, ActorInfo> action = new Action<Engine, ActorInfo>(
          (_, a) =>
          {
            if (a != null
              && p != a
              && a.Active
              && !a.IsDyingOrDead
              && Engine.MaskDataSet[a].Has(ComponentMask.CAN_BETARGETED)
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
                  m_target = a;
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

    private bool PickTargetLargeShip()
    {
      ActorInfo p = PlayerInfo.Actor;
      bool ret = false;

      if (p != null || p.Active)
      {
        TVCollisionResult tvcres = Engine.TrueVision.TVScene.MousePick(Engine.InputManager.MOUSE_X, Engine.InputManager.MOUSE_Y);
        TVMesh mesh = tvcres.GetCollisionMesh();
        if (mesh != null)
        {
          int n = ActorInfo.MeshModel.GetID(mesh.GetIndex());
          if (n != -1)
          {
            ActorInfo a = Engine.ActorFactory.Get(n);

            if (a != null) //&& a.TypeInfo.CollisionEnabled)
            {
              m_target = a;
              m_targetX = Engine.InputManager.MOUSE_X;
              m_targetY = Engine.InputManager.MOUSE_Y;
              ret = true;
            }
          }
        }
      }
      return ret;
    }
  }
}
