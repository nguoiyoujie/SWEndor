using MTV3D65;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class XWingATI : FighterGroup
  {
    private static XWingATI _instance;
    public static XWingATI Instance()
    {
      if (_instance == null) { _instance = new XWingATI(); }
      return _instance;
    }

    private XWingATI() : base("X-Wing")
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
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("XWing_RU_LD_WingATI"), new TV_3DVECTOR(-30, -30, 0), 0, 2000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("XWing_RU_LD_WingATI"), new TV_3DVECTOR(30, 30, 0), -2000, 0, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("XWing_RD_LU_WingATI"), new TV_3DVECTOR(30, -30, 0), 0, 2000, -3000, 0, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo(ActorTypeFactory.Instance().GetActorType("XWing_RD_LU_WingATI"), new TV_3DVECTOR(-30, 30, 0), -2000, 0, 0, 3000, -2500, 2500, 0.5f)
        };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 3, 25));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 5, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.RegenerationInfo.SelfRegenRate = 0.08f;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new XWingTorpWeapon() }
                                                        , {"laser", new XWingLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
      ainfo.SecondaryWeapons = new string[] { "none", "1:torp" };
      ainfo.AIWeapons = new string[] { "1:torp", "1:laser" };
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      /*
      if (ainfo.ActorState == ActorState.DEAD)
      {
        if (!ainfo.IsAggregateMode() && !Game.Instance().IsLowFPS())
        {
          double d = Engine.Instance().Random.NextDouble();

          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(0, 2000) / 100f;
            float y = Engine.Instance().Random.Next(0, 3000) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("XWing_RU_LD_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, -30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);
            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(-2000, 0) / 100f;
            float y = Engine.Instance().Random.Next(-3000, 0) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("XWing_RU_LD_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, 30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(0, 2000) / 100f;
            float y = Engine.Instance().Random.Next(-3000, 0) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("XWing_RD_LU_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, -30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(-2000, 0) / 100f;
            float y = Engine.Instance().Random.Next(-3000, 0) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("XWing_RD_LU_WingATI"));
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

