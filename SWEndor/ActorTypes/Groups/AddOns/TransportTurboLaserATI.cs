using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class TransportTurboLaserATI : AddOnGroup
  {
    private static TransportTurboLaserATI _instance;
    public static TransportTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new TransportTurboLaserATI(); }
      return _instance;
    }

    private TransportTurboLaserATI() : base("Transport Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 80;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\transport_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "Explosion";
      ainfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new TransportTurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };

      /*
      ainfo.SetStateB("EnableLaser", true);
      ainfo.SetStateS("LaserType", "Red Laser");
      ainfo.SetStateB("EnableLaserTrueAim", true);
      ainfo.SetStateF("LaserCooldown", Game.Instance().Time + 10f * (float)Engine.Instance().Random.NextDouble());
      ainfo.SetStateF("LaserCooldownRate", 1f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMin", 0.85f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMax", 1.15f);

      ainfo.SetStateB("EnableLaserBurst", true);
      ainfo.SetStateF("LaserBurst", 2);
      ainfo.SetStateF("LaserBurstAmount", 2);
      ainfo.SetStateF("LaserBurstMinCooldownRate", 3.5f);
      ainfo.SetStateF("LaserBurstMaxCooldownRate", 7.5f);
      */
    }
  }
}

