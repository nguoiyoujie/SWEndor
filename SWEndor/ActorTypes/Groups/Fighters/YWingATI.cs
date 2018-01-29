using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class YWingATI : FighterGroup
  {
    private static YWingATI _instance;
    public static YWingATI Instance()
    {
      if (_instance == null) { _instance = new YWingATI(); }
      return _instance;
    }

    private YWingATI() : base("Y-Wing")
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
      MaxTurnRate = 25;

      Score_perStrength = 400;
      Score_DestroyBonus = 2000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"ywing\ywing_far.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 4, 30));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 5, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionRate = 0.75f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "Explosion";

      ainfo.SelfRegenRate = 0.08f;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new YWingTorpWeapon() }
                                                        , {"ion", new YWingIonWeapon() }
                                                        , {"laser", new YWingLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none", "1:ion", "1:torp" };
      ainfo.AIWeapons = new List<string> { "1:torp", "1:ion", "1:laser" };
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      if (ainfo.ActorState == ActorState.DEAD)
      {
        if (!ainfo.IsAggregateMode() && !Game.Instance().IsLowFPS())
        {
          double d = Engine.Instance().Random.NextDouble();

          float x = Engine.Instance().Random.Next(-1000, 1000) / 100f;
          float y = Engine.Instance().Random.Next(0, 3000) / 100f;
          float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;
          float x2 = Engine.Instance().Random.Next(-1000, 1000) / 100f;
          float y2 = Engine.Instance().Random.Next(-3000, 0) / 100f;
          float z2 = Engine.Instance().Random.Next(-2500, 2500) / 100f;

          if (d > 0.5f)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("YWing_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);
            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("YWing_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x2, ainfo.Rotation.y + y2, ainfo.Rotation.z + z2);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }
        }
      }
    }

  }
}

