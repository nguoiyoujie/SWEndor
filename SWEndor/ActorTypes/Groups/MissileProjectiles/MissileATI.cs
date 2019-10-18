﻿using MTV3D65;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;

namespace SWEndor.ActorTypes.Instances
{
  public class MissileATI : Groups.MissileProjectile
  {
    internal MissileATI(Factory owner) : base(owner, "PROJ_MISL", "Missile")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 35);

      CombatData.ImpactDamage = 7.5f;
      MoveLimitData.MaxSpeed = 700;
      MoveLimitData.MinSpeed = 700;
      MoveLimitData.MaxTurnRate = 120;

      MeshData = new MeshData(Name, @"projectiles\missile.x", 1.5f);
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineMissile, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
    }
  }
}
