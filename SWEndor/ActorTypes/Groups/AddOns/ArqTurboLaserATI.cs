using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class ArqTurboLaserATI : AddOnGroup
  {
    private static ArqTurboLaserATI _instance;
    public static ArqTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new ArqTurboLaserATI(); }
      return _instance;
    }

    private ArqTurboLaserATI() : base("Arquitens Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 15;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\acclamator_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType= "Explosion";
      ainfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new AcclamatorTurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };

      /*
      ainfo.SetStateB("EnableLaser", true);
      ainfo.SetStateS("LaserType", "Yellow Laser");
      ainfo.SetStateB("EnableLaserTrueAim", true);
      ainfo.SetStateF("LaserCooldown", Game.Instance().Time + 10f * (float)Engine.Instance().Random.NextDouble());
      ainfo.SetStateF("LaserCooldownRate", 0.9f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMin", 0.2f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMax", 1.8f);

      ainfo.SetStateB("EnableLaserBurst", true);
      ainfo.SetStateF("LaserBurst", 6);
      ainfo.SetStateF("LaserBurstAmount", 6);
      ainfo.SetStateF("LaserBurstMinCooldownRate", 3);
      ainfo.SetStateF("LaserBurstMaxCooldownRate", 6);
      */
    }
  }
}

