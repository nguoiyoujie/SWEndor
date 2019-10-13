﻿using MTV3D65;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class YWingATI : Groups.RebelWing
  {
    internal YWingATI(Factory owner) : base(owner, "YWING", "Y-Wing")
    {
      SystemData.Parts = new SystemPart[] { SystemPart.ENGINE, SystemPart.ENERGY_CHARGER, SystemPart.ENERGY_STORE, SystemPart.LASER_WEAPONS, SystemPart.PROJECTILE_LAUNCHERS, SystemPart.RADAR, SystemPart.SCANNER, SystemPart.TARGETING_SYSTEM, SystemPart.COMLINK, SystemPart.HYPERDRIVE, SystemPart.SHIELD_GENERATOR };
      SystemData.MaxShield = 16;
      SystemData.MaxHull = 8;
      CombatData.ImpactDamage = 16;
      MoveLimitData.MaxSpeed = 300;
      MoveLimitData.MinSpeed = 150;
      MoveLimitData.MaxSpeedChangeRate = 150;
      MoveLimitData.MaxTurnRate = 32;

      ScoreData = new ScoreData(400, 2000);

      RegenData = new RegenData(false, 0.16f, 0, 0, 0);

      MeshData = new MeshData(Name, @"ywing\ywing.x");
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineYWing, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };

      Cameras = new LookData[] {
        new LookData(new TV_3DVECTOR(0, 4, 30), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 25, -100), new TV_3DVECTOR(0, 0, 2000)),
        new LookData(new TV_3DVECTOR(0, 0, -40), new TV_3DVECTOR(0, 0, -2000))
      };

      Debris = new DebrisSpawnerData[] {
        new DebrisSpawnerData("YWWING", new TV_3DVECTOR(-30, 0, 0), -10, 10, 0, 30, -25, 25, 0.5f),
        new DebrisSpawnerData("YWWING", new TV_3DVECTOR(30, 0, 0), -10, 10, -30, 0, -25, 25, 0.5f),
        };

      Loadouts = new string[] { "Y_WG_TORP", "Y_WG_ION", "Y_WG_LASR" };
    }
  }
}

