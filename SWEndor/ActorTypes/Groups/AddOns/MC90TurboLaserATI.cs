using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class mc90TurbolaserATI : AddOnGroup
  {
    private static mc90TurbolaserATI _instance;
    public static mc90TurbolaserATI Instance()
    {
      if (_instance == null) { _instance = new mc90TurbolaserATI(); }
      return _instance;
    }

    private mc90TurbolaserATI() : base("MC90 Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 150;
      ImpactDamage = 16;
      MaxTurnRate = 50f;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\mc90_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "Explosion";
      ainfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new MC90TurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };

      /*
      ainfo.SetStateB("EnableLaser", true);
      ainfo.SetStateS("LaserType", "Red Laser");
      ainfo.SetStateB("EnableLaserTrueAim", true);
      ainfo.SetStateF("LaserCooldown", Game.Instance().Time + 10f * (float)Engine.Instance().Random.NextDouble());
      ainfo.SetStateF("LaserCooldownRate", 0.8f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMin", 0.5f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMax", 1.5f);

      ainfo.SetStateB("EnableLaserBurst", true);
      ainfo.SetStateF("LaserBurst", 3);
      ainfo.SetStateF("LaserBurstAmount", 3);
      ainfo.SetStateF("LaserBurstMinCooldownRate", 3f);
      ainfo.SetStateF("LaserBurstMaxCooldownRate", 7.5f);
      */
    }
  }
}

