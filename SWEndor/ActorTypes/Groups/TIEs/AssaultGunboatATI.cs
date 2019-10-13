using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class AssaultGunboatATI : Groups.TIE
  {
    internal AssaultGunboatATI(Factory owner) : base(owner, "GUN", "Assault Gunboat")
    {
      SystemData.Parts = new SystemPart[] { SystemPart.ENGINE, SystemPart.ENERGY_CHARGER, SystemPart.ENERGY_STORE, SystemPart.LASER_WEAPONS, SystemPart.PROJECTILE_LAUNCHERS, SystemPart.RADAR, SystemPart.SCANNER, SystemPart.TARGETING_SYSTEM, SystemPart.COMLINK, SystemPart.HYPERDRIVE, SystemPart.SHIELD_GENERATOR };
      SystemData.MaxShield = 16;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 550;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 300;
      MoveLimitData.MaxTurnRate = 56;

      MoveLimitData.ZTilt = 0.75f;
      MoveLimitData.ZNormFrac = 0.004f;

      ScoreData = new ScoreData(800, 2000);

      RegenData = new RegenData(false, 0.25f, 0, 0, 0);

      MeshData = new MeshData(Name, @"gunboat\gunboat.x");

      Cameras = new LookData[]
      {
        new LookData(new TV_3DVECTOR(0, 7, 18), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -150), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Loadouts = new string[] { "GUN__TORP", "GUN__MISL", "GUN__LASR" };
    }
  }
}

