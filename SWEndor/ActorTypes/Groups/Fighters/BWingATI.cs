using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class BWingATI : Groups.RebelWing
  {
    internal BWingATI(Factory owner) : base(owner, "BWING", "B-Wing")
    {
      SystemData.Parts = new SystemPart[] { SystemPart.ENGINE, SystemPart.ENERGY_CHARGER, SystemPart.ENERGY_STORE, SystemPart.LASER_WEAPONS, SystemPart.PROJECTILE_LAUNCHERS, SystemPart.RADAR, SystemPart.SCANNER, SystemPart.TARGETING_SYSTEM, SystemPart.COMLINK, SystemPart.HYPERDRIVE, SystemPart.SHIELD_GENERATOR };
      SystemData.MaxShield = 30;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 400;
      MoveLimitData.MinSpeed = 200;
      MoveLimitData.MaxSpeedChangeRate = 200;
      MoveLimitData.MaxTurnRate = 40;

      ScoreData = new ScoreData(400, 2500);

      RegenData = new RegenData(false, 0.16f, 0, 0, 0);

      MeshData = new MeshData(Name, @"bwing\bwing.x");

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 0, 14), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
     };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("BWWING", new TV_3DVECTOR(-30, -30, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("BWWING", new TV_3DVECTOR(30, -30, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        new DebrisSpawnerData("BWWTOP", new TV_3DVECTOR(0, 0, 0), -10, 10, -10, 10, -25, 25, 0.5f),
        new DebrisSpawnerData("BWWBOT", new TV_3DVECTOR(0, -70, 0), -10, 10, -10, 10, -25, 25, 0.5f)
        };

      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineXWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      Loadouts = new string[] { "B_WG_TORP", "B_WG_ION", "B_WG_LASR" };
    }
  }
}

