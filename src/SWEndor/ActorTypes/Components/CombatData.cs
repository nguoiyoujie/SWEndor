using SWEndor.Actors;
using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct CombatData
  {
    public bool IsCombatObject;
    public bool HitWhileDyingLeadsToDeath;

    public float ImpactDamage;
    public DamageType DamageType;
    public bool IsLaser;

    // Projectiles
    public float ImpactCloseEnoughDistance;

    public CombatData Disabled { get { return new CombatData(false, false, DamageType.NONE); } }
    public CombatData DefaultProjectile { get { return new CombatData(true, true, DamageType.MISSILE); } }
    public CombatData DefaultFighter { get { return new CombatData(true, true, DamageType.COLLISION); } }
    public CombatData DefaultShip { get { return new CombatData(true, false, DamageType.COLLISION); } }

    public CombatData(bool enabled, bool hitdeath, DamageType type)
    {
      IsCombatObject = enabled;
      HitWhileDyingLeadsToDeath = hitdeath;
      ImpactDamage = 1;
      DamageType = type;
      IsLaser = false;
      ImpactCloseEnoughDistance = 0;
    }

    public void CopyFrom(CombatData src)
    {
      IsCombatObject = src.IsCombatObject;
      HitWhileDyingLeadsToDeath = src.HitWhileDyingLeadsToDeath;
    }

    public void Reset()
    {
      IsCombatObject = false;
      HitWhileDyingLeadsToDeath = false;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      IsCombatObject = f.GetBool(sectionname, "IsCombatObject", IsCombatObject);
      HitWhileDyingLeadsToDeath = f.GetBool(sectionname, "HitWhileDyingLeadsToDeath", HitWhileDyingLeadsToDeath);

      ImpactDamage = f.GetFloat(sectionname, "ImpactDamage", ImpactDamage);
      DamageType = f.GetEnumValue(sectionname, "DamageType", DamageType);
      IsLaser = f.GetBool(sectionname, "IsLaser", IsLaser);

      ImpactCloseEnoughDistance = f.GetFloat(sectionname, "ImpactCloseEnoughDistance", ImpactCloseEnoughDistance);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetBool(sectionname, "IsCombatObject", IsCombatObject);
      f.SetBool(sectionname, "HitWhileDyingLeadsToDeath", HitWhileDyingLeadsToDeath);

      f.SetFloat(sectionname, "ImpactDamage", ImpactDamage);
      f.SetEnum(sectionname, "DamageType", DamageType);
      f.SetBool(sectionname, "IsLaser", IsLaser);

      f.SetFloat(sectionname, "ImpactCloseEnoughDistance", ImpactCloseEnoughDistance);
    }
  }
}
