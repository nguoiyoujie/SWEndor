using System;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public enum ExplosionTrigger { NONE, DYING, ALWAYS }
  public enum DeathExplosionTrigger { NONE, TIMENOTEXPIRED_ONLY, ALWAYS }

  public struct ExplodeData
  {
    public const string ExplosionTypeDefault = "Explosion";
    public const string DeathExplosionTypeDefault = "ExplosionSm";

    public float ExplosionCooldown;
    public float ExplosionRate;
    public float ExplosionSize;
    public string ExplosionType;
    public DeathExplosionTrigger DeathExplosionTrigger;
    public string DeathExplosionType;
    public float DeathExplosionSize;

    internal ActorTypeInfo _cache;

    public ExplodeData(float explosionRate = 1, float explosionSize = 1, string explosionType = ExplosionTypeDefault, DeathExplosionTrigger deathTrigger = DeathExplosionTrigger.NONE, float deathExplosionSize = 1, string deathExplosionType = DeathExplosionTypeDefault)
    {
      ExplosionCooldown = 0;
      ExplosionRate = explosionRate;
      ExplosionSize = explosionSize;
      ExplosionType = explosionType;
      DeathExplosionTrigger = deathTrigger;
      DeathExplosionType = deathExplosionType;
      DeathExplosionSize = deathExplosionSize;
      _cache = null;
    }

    public void CopyFrom(ExplodeData src)
    {
      ExplosionRate = src.ExplosionRate;
      ExplosionSize = src.ExplosionSize;
      ExplosionType = src.ExplosionType;
      DeathExplosionTrigger = src.DeathExplosionTrigger;
      DeathExplosionType = src.DeathExplosionType;
      DeathExplosionSize = src.DeathExplosionSize;
    }

    public void Reset()
    {
      ExplosionCooldown = 0;
      ExplosionRate = 1;
      ExplosionSize = 1;
      ExplosionType = ExplosionTypeDefault;
      DeathExplosionTrigger = DeathExplosionTrigger.NONE;
      DeathExplosionType = DeathExplosionTypeDefault;
      DeathExplosionSize = 1;

      _cache = null;
    }

    public static implicit operator float(ExplodeData v)
    {
      throw new NotImplementedException();
    }
  }
}
