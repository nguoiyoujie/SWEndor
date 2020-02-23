using SWEndor.Actors;
using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct CombatData
  {
    private const string sCombat = "Combat";
    public static CombatData Disabled { get { return new CombatData(false, false, DamageType.NONE); } }
    public static CombatData DefaultProjectile { get { return new CombatData(true, true, DamageType.MISSILE); } }
    public static CombatData DefaultFighter { get { return new CombatData(true, true, DamageType.COLLISION); } }
    public static CombatData DefaultShip { get { return new CombatData(true, false, DamageType.COLLISION); } }

    [INIValue(sCombat, "IsCombatObject")]
    public bool IsCombatObject;

    [INIValue(sCombat, "HitWhileDyingLeadsToDeath")]
    public bool HitWhileDyingLeadsToDeath;

    [INIValue(sCombat, "ImpactDamage")]
    public float ImpactDamage;

    [INIValue(sCombat, "DamageType")]
    public DamageType DamageType;

    [INIValue(sCombat, "IsLaser")]
    public bool IsLaser;

    // Projectile-only
    [INIValue(sCombat, "ImpactCloseEnoughDistance")]
    public float ImpactCloseEnoughDistance;

    public CombatData(bool enabled, bool hitdeath, DamageType type)
    {
      IsCombatObject = enabled;
      HitWhileDyingLeadsToDeath = hitdeath;
      ImpactDamage = 1;
      DamageType = type;
      IsLaser = false;
      ImpactCloseEnoughDistance = 0;
    }

    public void Reset()
    {
      this = Disabled;
    }
  }
}
