using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class TIE_D_ATI : TIEGroup
  {
    private static TIE_D_ATI _instance;
    public static TIE_D_ATI Instance()
    {
      if (_instance == null) { _instance = new TIE_D_ATI(); }
      return _instance;
    }

    private TIE_D_ATI() : base("TIE Defender")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 24;
      ImpactDamage = 16;
      MaxSpeed = 600;
      MinSpeed = 300;
      MaxSpeedChangeRate = 300;
      MaxTurnRate = 65;

      ZTilt = 2.75f;
      ZNormFrac = 0.005f;

      Score_perStrength = 800;
      Score_DestroyBonus = 2000;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"tie\tie_defender.x");
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

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "ExplosionSm";
      ainfo.ExplosionRate = 0.75f;
      ainfo.ExplosionSize = 1;
      ainfo.ExplosionType = "Explosion";

      ainfo.SelfRegenRate = 0.075f;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new TIE_D_LaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser", "2:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };
      
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
            ActorCreationInfo acinfo = new ActorCreationInfo(TIE_InterceptorWingATI.Instance());
            acinfo.Position = ainfo.GetPosition() + new TV_3DVECTOR(-30, 0, 0);
            acinfo.Rotation = new TV_3DVECTOR(ainfo.Rotation.x + x, ainfo.Rotation.y + y, ainfo.Rotation.z + z);
            acinfo.InitialSpeed = ainfo.Speed;
            ActorInfo a = ActorInfo.Create(acinfo);
          }

          d = Engine.Instance().Random.NextDouble();
          if (d > 0.5f)
          {
            ActorCreationInfo acinfo = new ActorCreationInfo(TIE_InterceptorWingATI.Instance());
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

