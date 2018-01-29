using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWEndor
{
  public class NebulonBTurboLaserATI : AddOnGroup
  {
    private static NebulonBTurboLaserATI _instance;
    public static NebulonBTurboLaserATI Instance()
    {
      if (_instance == null) { _instance = new NebulonBTurboLaserATI(); }
      return _instance;
    }

    private NebulonBTurboLaserATI() : base("Nebulon B Turbolaser Tower")
    {
      // Combat
      IsCombatObject = true;
      IsSelectable = true;
      IsDamage = false;
      CollisionEnabled = true;

      MaxStrength = 120;
      ImpactDamage = 16;

      Score_perStrength = 250;
      Score_DestroyBonus = 1250;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"turbotowers\nebulonb_turbolaser.x");
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.EnableDeathExplosion = true;
      ainfo.DeathExplosionType = "Explosion";
      ainfo.DeathExplosionSize = 5;

      ainfo.Weapons = new Dictionary<string, WeaponInfo>{ {"laser", new NebulonBTurboLaserWeapon() }
                                                        };
      ainfo.PrimaryWeapons = new List<string> { "1:laser" };
      ainfo.SecondaryWeapons = new List<string> { "none" };
      ainfo.AIWeapons = new List<string> { "1:laser" };

      /*
      ainfo.SetStateB("EnableLaser", true);
      ainfo.SetStateS("LaserType", "Red Laser");
      ainfo.SetStateB("EnableLaserTrueAim", true);
      ainfo.SetStateF("LaserCooldown", Game.Instance().Time + 10f * (float)Engine.Instance().Random.NextDouble());
      ainfo.SetStateF("LaserCooldownRate", 1.2f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMin", 0.75f);
      ainfo.SetStateF("LaserTrueAimInaccuracyMax", 1.25f);

      ainfo.SetStateB("EnableLaserBurst", true);
      ainfo.SetStateF("LaserBurst", 4);
      ainfo.SetStateF("LaserBurstAmount", 4);
      ainfo.SetStateF("LaserBurstMinCooldownRate", 6);
      ainfo.SetStateF("LaserBurstMaxCooldownRate", 12);
      */
    }
  }
}

