using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using SWEndor.Game.Player;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Sound;
using System;
using SWEndor.Game.UI.Helpers;

namespace SWEndor.Game.UI.Widgets
{
  public class TargetWidget : Widget
  {
    private int m_targetID = -1;
    private readonly float m_targetSize = 5;
    private readonly float m_targetSizeDiamond = 4;
    private float m_targetBigSize = 0;

    public TargetWidget(Screen2D owner) : base(owner, "target") { }

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

      ActorInfo prev_target = Engine.ActorFactory.Get(m_targetID);

      if (p.IsSystemOperational(SystemPart.TARGETING_SYSTEM))
      { 
        if (PlayerInfo.PlayerAIEnabled)
        {
          ActorInfo t = p.AI.Target.GetTargetActor(Engine);
          m_targetID = t?.ID ?? -1;
          PlayerInfo.LockTarget = true;
        }
        else
        {
          if (!PlayerInfo.LockTarget)
          {
            if (p.TargetType.Has(TargetType.SHIP))
              TargetPicker.PickTargetLargeShip(Engine, ref m_targetID);
            else
              TargetPicker.PickTargetFighter(Engine, !PlayerInfo.IsTorpedoMode, ref m_targetID);
          }
        }
      }
      else
      {
        m_targetID = -1;
      }

      ActorInfo m_target = Engine.ActorFactory.Get(m_targetID);
      if (m_target == null || !m_target.Active)
      {
        PlayerInfo.TargetActorID = -1;
        PlayerInfo.LockTarget = false;
      }
      else
      {
        float x = 0;
        float y = 0;
        TVScreen2DImmediate.Math_3DPointTo2D(m_target.GetGlobalPosition(), ref x, ref y);
        float dist = DistanceModel.GetDistance(Engine, p, m_target, 7501);
        float limit = 0.05f * dist;
        if (limit < 250)
          limit = 250;

        if (Check(Engine, p, x, y, limit, dist, m_target))
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

    private static bool Check(Engine e, ActorInfo p, float x, float y, float limit, float dist, ActorInfo target)
    {
      if (p.TargetType.Has(TargetType.SHIP))
      {
        return !target.Active
        || target.IsDyingOrDead
        || !target.InCombat
        || !e.PlayerCameraInfo.Camera.IsPointVisible(target.GetGlobalPosition());
      }
      else
        return !target.Active
        || target.IsDyingOrDead
        || !target.InCombat
        || target.TopParent == p
        || (e.PlayerInfo.Actor.IsAlliedWith(target) && e.PlayerInfo.IsTorpedoMode)
        || dist > Globals.AcquisitionRange
        || !e.PlayerInfo.LockTarget
         && (!e.PlayerCameraInfo.Camera.IsPointVisible(target.GetGlobalPosition())
        || Math.Abs(x - e.ScreenWidth / 2) > limit
        || Math.Abs(y - e.ScreenHeight / 2) > limit
        );
    }

    private void Draw(ActorInfo p, float x, float y, float dist, ActorInfo target, ActorInfo prev_target)
    {
      int icolor = target.Faction.Color.Value;
      string name = target.Name;
      TVScreen2DImmediate.Action_Begin2D();

      DrawIcons.DrawActiveTarget(TVScreen2DImmediate, Owner, target.GetBoundingBox(false), target.Faction.Color);

      if (PlayerInfo.IsTorpedoMode)
      {
        if (!PlayerInfo.Actor.IsAlliedWith(target) && prev_target != target)
        {
          Engine.SoundManager.SetSound(SoundGlobals.Button3);
          m_targetBigSize = m_targetSize * 3;
        }
        if (m_targetBigSize > m_targetSize)
        {
          m_targetBigSize -= 25 * Engine.Game.TimeSinceRender;
          TVScreen2DImmediate.Draw_Box(x - m_targetBigSize, y - m_targetBigSize, x + m_targetBigSize, y + m_targetBigSize, icolor);
        }

        Weapons.WeaponInfo weap = PlayerInfo.SecondaryWeapon.Weapon;
        if (weap != null && weap.Ammo.Count > 0)
        {
          TVScreen2DImmediate.Draw_FilledBox(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, icolor);
        }
        else
        {
          TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, icolor);
        }
      }
      else
      {
        TVScreen2DImmediate.Draw_Box(x - m_targetSize, y - m_targetSize, x + m_targetSize, y + m_targetSize, icolor);
      }
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      string msg;
      if (p.IsSystemOperational(SystemPart.SCANNER))
        msg = "{0}\nDamage: {1}%".F(name, (100 - target.HP_Perc).ToString("0"));
      else
        msg = name;

        TVScreen2DText.TextureFont_DrawText(msg
          , x, y + m_targetSize + 10, icolor
          , FontFactory.Get(Font.T10).ID
          );
      TVScreen2DText.Action_EndText();

      PlayerInfo.TargetActorID = target.ID;

      if (!PlayerInfo.Actor.IsAlliedWith(target) && !PlayerInfo.IsTorpedoMode)
      {
        // Targeting cross
        // Anticipate
        float spd = PlayerInfo.PrimaryWeapon.Weapon.Proj.ProjSpeed;
        float d = dist / spd; // Laser Speed
        TV_3DVECTOR t = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d);
        TVScreen2DImmediate.Math_3DPointTo2D(t, ref x, ref y);

        TVScreen2DImmediate.Action_Begin2D();
        TVScreen2DImmediate.Draw_Line(x - m_targetSize, y, x + m_targetSize, y, icolor);
        TVScreen2DImmediate.Draw_Line(x, y - m_targetSize, x, y + m_targetSize, icolor);
        TVScreen2DImmediate.Action_End2D();

        if (PlayerInfo.SecondaryWeapon.Weapon.Proj.WeaponType == Weapons.WeaponType.ION || PlayerInfo.SecondaryWeapon.Weapon.Proj.WeaponType == Weapons.WeaponType.LASER)
        {
          float spd2 = PlayerInfo.SecondaryWeapon.Weapon.Proj.ProjSpeed;
          if (spd2 != spd)
          {
            d = dist / spd2; // Laser Speed
            t = target.GetRelativePositionXYZ(0, 0, target.MoveData.Speed * d);
            int scolor = icolor;
            if (PlayerInfo.SecondaryWeapon.Weapon.Proj.WeaponType == Weapons.WeaponType.ION)
            {
              scolor = ColorLocalization.Get(ColorLocalKeys.GAME_SYSTEM_ION).Value;
            }
            TVScreen2DImmediate.Math_3DPointTo2D(t, ref x, ref y);

            TVScreen2DImmediate.Action_Begin2D();
            // draw a smaller 'I'
            float ionsize = (m_targetSize - 2).Max(2);
            TVScreen2DImmediate.Draw_Line(x - ionsize, y - ionsize, x + ionsize, y - ionsize, scolor);
            TVScreen2DImmediate.Draw_Line(x, y - ionsize, x, y + ionsize, scolor);
            TVScreen2DImmediate.Draw_Line(x - ionsize, y + ionsize, x + ionsize, y + ionsize, scolor);
            TVScreen2DImmediate.Action_End2D();
          }
        }
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
              TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy + m2, icolor);
              TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy - m2, icolor);
              TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy + m2, icolor);
              TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy - m2, icolor);
            }

            m2 = m_targetSizeDiamond;
            TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy + m2, icolor);
            TVScreen2DImmediate.Draw_Line(sx - m2, sy, sx, sy - m2, icolor);
            TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy + m2, icolor);
            TVScreen2DImmediate.Draw_Line(sx + m2, sy, sx, sy - m2, icolor);
          }
        }
        TVScreen2DImmediate.Action_End2D();
      }
    }
  }
}
