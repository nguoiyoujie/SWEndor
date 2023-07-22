using SWEndor.Game.Actors;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct CombatData
  {
    public static CombatData Disabled { get { return new CombatData(false, false, DamageType.NONE); } }
    public static CombatData DefaultProjectile { get { return new CombatData(true, true, DamageType.MISSILE) { IsMissile = true }; } }
    public static CombatData DefaultFighter { get { return new CombatData(true, true, DamageType.COLLISION); } }
    public static CombatData DefaultShip { get { return new CombatData(true, false, DamageType.COLLISION); } }

    [INIValue]
    public bool IsCombatObject;

    [INIValue]
    public bool HitWhileDyingLeadsToDeath;

    [INIValue]
    public float ImpactDamage;

    [INIValue]
    public DamageType DamageType;

    [INIValue]
    public bool IsTeleport;

    [INIValue]
    public bool IsLaser;

    [INIValue]
    public bool IsMissile;


    // Projectile-only
    [INIValue]
    public float ImpactCloseEnoughDistance;

    public CombatData(bool enabled, bool hitdeath, DamageType type)
    {
      IsCombatObject = enabled;
      HitWhileDyingLeadsToDeath = hitdeath;
      ImpactDamage = 1;
      DamageType = type;
      IsTeleport = false;
      IsLaser = false;
      IsMissile = false;
      ImpactCloseEnoughDistance = 0;
    }

    public void Reset()
    {
      this = Disabled;
    }
  }
}
