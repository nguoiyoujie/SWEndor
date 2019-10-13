using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class AWingATI : Groups.RebelWing
  {
    internal AWingATI(Factory owner) : base(owner, "AWING", "A-Wing")
    {
      SystemData.Parts = new SystemPart[] { SystemPart.ENGINE, SystemPart.ENERGY_CHARGER, SystemPart.ENERGY_STORE, SystemPart.LASER_WEAPONS, SystemPart.PROJECTILE_LAUNCHERS, SystemPart.RADAR, SystemPart.SCANNER, SystemPart.TARGETING_SYSTEM, SystemPart.COMLINK, SystemPart.HYPERDRIVE, SystemPart.SHIELD_GENERATOR };
      SystemData.MaxShield = 8;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 250;
      MoveLimitData.MaxSpeedChangeRate = 300;
      MoveLimitData.MaxTurnRate = 55;

      ScoreData = new ScoreData(500, 2500);

      RegenData = new RegenData(false, 0.16f, 0, 0, 0);

      MeshData = new MeshData(Name, @"awing\awing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 2, 20), new TV_3DVECTOR(0, 2, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineAWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      Loadouts = new string[] { "A_WG_TORP", "A_WG_MISL", "A_WG_LASR" };
    }
  }
}

