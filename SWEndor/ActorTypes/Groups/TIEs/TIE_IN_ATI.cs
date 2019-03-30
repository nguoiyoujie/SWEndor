using MTV3D65;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class TIE_IN_ATI : TIEGroup
  {
    private static TIE_IN_ATI _instance;
    public static TIE_IN_ATI Instance()
    {
      if (_instance == null) { _instance = new TIE_IN_ATI(); }
      return _instance;
    }

    private TIE_IN_ATI() : base("TIE Interceptor")
    {
      MaxStrength = 8;
      ImpactDamage = 16;
      MaxSpeed = 500;
      MinSpeed = 250;
      MaxSpeedChangeRate = 200;
      MaxTurnRate = 55;

      Score_perStrength = 500;
      Score_DestroyBonus = 1000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor.x");
      SourceFarMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_interceptor_far.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      // Camera System
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 0, 20));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 125, -200));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, 2000));
      ainfo.CamLocations.Add(new TV_3DVECTOR(0, 40, 250));
      ainfo.CamTargets.Add(new TV_3DVECTOR(0, 0, -2000));

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionInfo.ExplosionRate = 0.75f;
      ainfo.ExplosionInfo.ExplosionSize = 1;
      ainfo.ExplosionInfo.ExplosionType = "Explosion";

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new TIE_IN_LaserWeapon() }
                                                        , {"2xlsr", new TIE_IN_DblLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new string[] { "1:laser", "2:laser" };
      ainfo.SecondaryWeapons = new string[] { "none" };
      ainfo.AIWeapons = new string[] { "1:2xlsr" };

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
            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("TIE_InterceptorWingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);
            acinfo.InitialSpeed = ainfo.MovementInfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType("TIE_InterceptorWingATI"));
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(30, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x2, ainfo.Rotation.y + y2, ainfo.Rotation.z + z2);

            acinfo.InitialSpeed = ainfo.MovementInfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }
        }
      }
    }
  }
}

