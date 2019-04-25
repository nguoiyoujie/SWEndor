using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class XWingATI : Groups.RebelWing
  {
    internal XWingATI(Factory owner) : base(owner, "X-Wing")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 30;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 500;
      MaxTurnRate = 50;

      Score_perStrength = 500;
      Score_DestroyBonus = 2500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"xwing\xwing_far.x");

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("XWing_RU_LD_WingATI", new TV_3DVECTOR(-30, -30, 0), 0, 2000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RU_LD_WingATI", new TV_3DVECTOR(30, 30, 0), -2000, 0, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RD_LU_WingATI", new TV_3DVECTOR(30, -30, 0), 0, 2000, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("XWing_RD_LU_WingATI", new TV_3DVECTOR(-30, 30, 0), -2000, 0, 0, 3000, -2500, 2500, 0.5f)
        };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 3, 25),
        new TV_3DVECTOR(0, 125, -200),
        new TV_3DVECTOR(0, 40, 250)
      };

      ainfo.CameraSystemInfo.CamTargets = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 5, 2000),
        new TV_3DVECTOR(0, 0, 2000),
        new TV_3DVECTOR(0, 0, -2000)
      };

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.RegenerationInfo.SelfRegenRate = 0.08f;

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new XWingTorpWeapon() }
                                                        , {"laser", new XWingLaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:laser" };
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      /*
      if (ainfo.ActorState == ActorState.DEAD)
      {
        if (!ainfo.IsAggregateMode() && !Globals.Engine.Game.IsLowFPS())
        {
          double d = Globals.Engine.Random.NextDouble();

          if (d > 0.5f)
          {
            float x = Globals.Engine.Random.Next(0, 2000) / 100f;
            float y = Globals.Engine.Random.Next(0, 3000) / 100f;
            float z = Globals.Engine.Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("XWing_RU_LD_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, -30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);
            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Globals.Engine.Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Globals.Engine.Random.Next(-2000, 0) / 100f;
            float y = Globals.Engine.Random.Next(-3000, 0) / 100f;
            float z = Globals.Engine.Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("XWing_RU_LD_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, 30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Globals.Engine.Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Globals.Engine.Random.Next(0, 2000) / 100f;
            float y = Globals.Engine.Random.Next(-3000, 0) / 100f;
            float z = Globals.Engine.Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("XWing_RD_LU_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, -30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Globals.Engine.Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Globals.Engine.Random.Next(-2000, 0) / 100f;
            float y = Globals.Engine.Random.Next(-3000, 0) / 100f;
            float z = Globals.Engine.Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("XWing_RD_LU_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, 30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }
        }
      }
      */
    }
  }
}

