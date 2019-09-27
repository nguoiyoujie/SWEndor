using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Sound;
using SWEndor.Weapons;
using System;

namespace SWEndor.UI.Widgets
{
  public class Target : Widget
  {
    private int m_targetID = -1;
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

      ActorInfo prev_target = ActorFactory.Get(m_targetID);

      if (PlayerInfo.PlayerAIEnabled)
      {
        ActorInfo t = p.AIData.GetTargetActor(p);
        m_targetID = t?.ID ?? -1;
        PlayerInfo.LockTarget = true;
      }
      else
      {
        if (!PlayerInfo.LockTarget)
        {
          if (p.TypeInfo is ActorTypes.Groups.LargeShip)
            PickTargetLargeShip();
          else
            PickTargetFighter(!PlayerInfo.IsTorpedoMode);
        }
      }

      ActorInfo m_target = ActorFactory.Get(m_targetID);
      if (m_target == null)
      {
        PlayerInfo.TargetActorID = -1;
        PlayerInfo.LockTarget = false;
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

        if (Check(p, x, y, limit, dist, m_target))
        {
          m_targetID = -1;
          PlayerInfo.TargetActorID = -1;
          PlayerInfo.LockTarget = false;
        }
        else
        {
          Draw(p, x, y, dist, m_target, prev_target);
        }
      }
    }

    private bool Check(ActorInfo p, float x, float y, float limit, float dist, ActorInfo target)
    {
      if (p.TypeInfo is ActorTypes.Groups.LargeShip)
      {
        return !target.Active
        || target.IsDyingOrDead
        || !target.CombatData.IsCombatObject
        || !PlayerCameraInfo.Camera.IsPointVisible(target.GetGlobalPosition());
      }
      else
        return !target.Active
        || target.IsDyingOrDead
        || !target.CombatData.IsCombatObject
        || (PlayerInfo.Actor.Faction.IsAlliedWith(target.Faction) && PlayerInfo.IsTorpedoMode)
        || dist > Globals.AcquisitionRange
        || !PlayerInfo.LockTarget
         && (!PlayerCameraInfo.Camera.IsPointVisible(target.GetGlobalPosition())
        || Math.Abs(x - Engine.ScreenWidth / 2) > limit
        || Math.Abs(y - Engine.ScreenHeight / 2) > limit
        );
    }

    private void Draw(ActorInfo p, float x, float y, float dist, ActorInfo target, ActorInfo prev_target)
    {
      TV_COLOR acolor = target.Faction.Color;
      string name = target.Name;
      TVScreen2DImmediate.Action_Begin2D();
      if (PlayerInfo.IsTorpedoMode)
      {
        if (!PlayerInfo.Actor.Faction.IsAlliedWith(target.Faction) && prev_target != target)
        {
          SoundManager.SetSound(SoundGlobals.Button3);
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
      TVScreen2DText.TextureFont_DrawText(string.Format("{0}\nDamage: {1}%", name, (100 - target.HP_Perc).ToString("0"))
        , x, y + m_targetSize + 10, acolor.GetIntColor()
        , FontFactory.Get(Font.T10).ID
        );
      TVScreen2DText.Action_EndText();

      PlayerInfo.TargetActorID = target.ID;

      if (!PlayerInfo.Actor.Faction.IsAlliedWith(target.Faction) && !PlayerInfo.IsTorpedoMode)
      {
        // Targeting cross
        // Anticipate
        float d = dist / Globals.LaserSpeed; // Laser Speed
        TV_3DVECTOR t = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d);
        TVScreen2DImmediate.Math_3DPointTo2D(t, ref x, ref y);

        TVScreen2DImmediate.Action_Begin2D();
        TVScreen2DImmediate.Draw_Line(x - m_targetSize, y, x + m_targetSize, y, acolor.GetIntColor());
        TVScreen2DImmediate.Draw_Line(x, y - m_targetSize, x, y + m_targetSize, acolor.GetIntColor());
        TVScreen2DImmediate.Action_End2D();
      }

      // Squad diamond
      if (!target.Squad.IsNull)
      {
        TVScreen2DImmediate.Action_Begin2D();
        foreach (ActorInfo s in target.Squad.Members)
        {
          if (s != target && !s.IsPlayer && PlayerCameraInfo.Camera.IsPointVisible(s.GetGlobalPosition()))
          {
            float sx = 0;
            float sy = 0;
            TVScreen2DImmediate.Math_3DPointTo2D(s.GetGlobalPosition(), ref sx, ref sy);

            float m2 = m_targetSizeDiamond + 5;
            if (s == target.Squad.Leader)
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

      TVScreen2DImmediate.Action_Begin2D();
      int tex = Engine.TrueVision.TargetRenderSurface.GetTexture();
      int w = Engine.TrueVision.TargetRenderSurface.GetWidth() / 2;
      int h = Engine.TrueVision.TargetRenderSurface.GetHeight() / 2;
      TVScreen2DImmediate.Draw_Texture(tex
                                , Engine.ScreenWidth / 2 - w 
                                , Engine.ScreenHeight / 2 - h + 200
                                , Engine.ScreenWidth / 2 + w
                                , Engine.ScreenHeight / 2 + h + 200
                                , acolor.GetIntColor()
                                , acolor.GetIntColor()
                                , acolor.GetIntColor()
                                , acolor.GetIntColor());
      TVScreen2DImmediate.Action_End2D();
    }

    float bestlimit = 9999;
    Action<Engine, ActorInfo, Target, bool> targetAction = new Action<Engine, ActorInfo, Target, bool>(
      (e, a, t, b) =>
      {
        ActorInfo p0 = e.PlayerInfo.Actor;
        if (a != null
          && !a.IsPlayer
          && a.Active
          && !a.IsDyingOrDead
          && a.Mask.Has(ComponentMask.CAN_BETARGETED)
          && (b || !p0.Faction.IsAlliedWith(a.Faction))
          && e.PlayerCameraInfo.Camera.IsPointVisible(a.GetGlobalPosition())
          )
        {
          float dist = ActorDistanceInfo.GetDistance(p0, a, 7501);
          if (dist < 7500)
          {
            float x = 0;
            float y = 0;
            float limit = 0.015f * dist;
            if (limit < 50)
              limit = 50;
            t.m_targetX = limit;
            t.m_targetY = limit;
            e.TrueVision.TVScreen2DImmediate.Math_3DPointTo2D(a.GetGlobalPosition(), ref x, ref y);

            x -= e.ScreenWidth / 2;
            y -= e.ScreenHeight / 2;

            x = Math.Abs(x);
            y = Math.Abs(y);

            if (x < limit && y < limit && x + y < t.bestlimit)
            {
              t.bestlimit = x + y;
              t.m_targetID = a.ID;
              t.m_targetX = x;
              t.m_targetY = y;
            }
          }
        }
      }
    );

    private bool PickTargetFighter(bool pick_allies)
    {
      ActorInfo p = PlayerInfo.Actor;
      bestlimit = 9999;

      if (p != null || p.Active)
        ActorFactory.DoEach(targetAction, this, pick_allies);

      return bestlimit < 9999;
    }

    private bool PickTargetLargeShip()
    {
      ActorInfo p = PlayerInfo.Actor;
      bool ret = false;

      if (p != null || p.Active)
      {
        TVCollisionResult tvcres;
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          tvcres = Engine.TrueVision.TVScene.MousePick(Engine.InputManager.MOUSE_X, Engine.InputManager.MOUSE_Y);
        TVMesh mesh = tvcres.GetCollisionMesh();
        if (mesh != null)
        {
          int n = MeshModel.GetID(mesh.GetIndex());
          if (n != -1)
          {
            ActorInfo a = Engine.ActorFactory.Get(n);

            if (a != null) //&& a.TypeInfo.CollisionEnabled)
            {
              m_targetID = a.ID;
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
