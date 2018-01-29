using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class BWingATI : FighterGroup
  {
    private static BWingATI _instance;
    public static BWingATI Instance()
    {
      if (_instance == null) { _instance = new BWingATI(); }
      return _instance;
    }

    private BWingATI() : base("B-Wing")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 45;
      ImpactDamage = 16;
      MaxSpeed = 400;
      MinSpeed = 200;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 35;

      Score_perStrength = 400;
      Score_DestroyBonus = 2500;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"bwing\bwing_far.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 0, 14));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
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

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new BWingTorpWeapon() }
                                                        , {"ion", new BWingIonWeapon() }
                                                        , {"laser", new BWingLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser", "2:laser", "4:laser" };
      ainfo.SecondaryWeapons = new List<string> { "4:laser", "1:ion", "1:torp" };
      ainfo.AIWeapons = new List<string> { "1:ion", "1:torp", "1:laser" };
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);

      if (ainfo.ActorState == ActorState.DEAD)
      {
        if (!ainfo.IsAggregateMode() && !Game.Instance().IsLowFPS())
        {
          double d = Engine.Instance().Random.NextDouble();

          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(-1000, 1000) / 100f;
            float y = Engine.Instance().Random.Next(0, 3000) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("BWing_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, -30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);
            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(-1000, 1000) / 100f;
            float y = Engine.Instance().Random.Next(-3000, 0) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("BWing_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, -30, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(-1000, 1000) / 100f;
            float y = Engine.Instance().Random.Next(-1000, 1000) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("BWing_Top_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            float x = Engine.Instance().Random.Next(-1000, 1000) / 100f;
            float y = Engine.Instance().Random.Next(-1000, 1000) / 100f;
            float z = Engine.Instance().Random.Next(-2500, 2500) / 100f;

            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("BWing_Bottom_WingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(0, -70, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);

            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }
        }
      }
    }
  }
}

