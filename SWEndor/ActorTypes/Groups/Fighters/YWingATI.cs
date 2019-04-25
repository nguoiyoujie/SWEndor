using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class YWingATI : Groups.RebelWing
  {
    internal YWingATI(Factory owner) : base(owner, "Y-Wing")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 36;
      ImpactDamage = 16;
      MaxSpeed = 300;
      MinSpeed = 150;
      MaxSpeedChangeRate = 150;
      MaxTurnRate = 32;

      Score_perStrength = 400;
      Score_DestroyBonus = 2000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing_far.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.CameraSystemInfo.CamLocations = new TV_3DVECTOR[]
      {
        new TV_3DVECTOR(0, 4, 30),
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

      ainfo.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new YWingTorpWeapon() }
                                                        , {"ion", new YWingIonWeapon() }
                                                        , {"laser", new YWingLaserWeapon() }
                                                        };
      ainfo.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser" };
      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "none", "1:ion", "1:torp" };
      ainfo.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:ion", "1:laser" };

      Debris = new DebrisSpawnerInfo[] {
        new DebrisSpawnerInfo("YWing_WingATI", new TV_3DVECTOR(-30, 0, 0), -1000, 1000, 0, 3000, -2500, 2500, 0.5f),
        new DebrisSpawnerInfo("YWing_WingATI", new TV_3DVECTOR(30, 0, 0), -1000, 1000, -3000, 0, -2500, 2500, 0.5f),
        };
    }

    /*
    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      if (ainfo.ActorState == ActorState.DEAD)
      {
        if (!ainfo.IsAggregateMode() && !Globals.Engine.Game.IsLowFPS())
        {
          double d = Globals.Engine.Random.NextDouble();

          float x = Globals.Engine.Random.Next(-1000, 1000) / 100f;
          float y = Globals.Engine.Random.Next(0, 3000) / 100f;
          float z = Globals.Engine.Random.Next(-2500, 2500) / 100f;
          float x2 = Globals.Engine.Random.Next(-1000, 1000) / 100f;
          float y2 = Globals.Engine.Random.Next(-3000, 0) / 100f;
          float z2 = Globals.Engine.Random.Next(-2500, 2500) / 100f;

          if (d > 0.5f)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("YWing_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);
            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Globals.Engine.Random.NextDouble();
          if (d > 0.5f)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(Globals.Engine.ActorTypeFactory.Get("YWing_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x2, ainfo.Rotation.y + y2, ainfo.Rotation.z + z2);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }
        }
      }
    }
    */
  }
}

