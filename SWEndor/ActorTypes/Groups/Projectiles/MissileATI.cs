using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class MissileATI : Groups.MissileProjectile
  {
    internal MissileATI(Factory owner) : base(owner, "Missile")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 35);

      ImpactDamage = 7.5f;
      MoveLimitData.MaxSpeed = 700;
      MoveLimitData.MinSpeed = 700;
      MoveLimitData.MaxTurnRate = 120;

      Scale = 2.5f;

      MeshData = new MeshData(Name, @"projectiles\missile.x");
      SoundSources = new SoundSourceData[] { new SoundSourceData(SoundGlobals.EngineMissile, 200f, new TV_3DVECTOR(0, 0, -30), true, isEngineSound: true) };
    }
  }
}

