using SWEndor.Actors;
using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct CombatData
  {
    public bool IsCombatObject;
    public bool HitWhileDyingLeadsToDeath;

    public float MaxStrength;
    public float ImpactDamage;
    public DamageType DamageType;
    public bool IsLaser;

    // Projectiles
    public float ImpactCloseEnoughDistance;

    public CombatData Disabled { get { return new CombatData(false, false); } }
    public CombatData DefaultFighter { get { return new CombatData(true, true); } }
    public CombatData DefaultShip { get { return new CombatData(true, false); } }

    public CombatData(bool enabled, bool hitdeath)
    {
      IsCombatObject = enabled;
      HitWhileDyingLeadsToDeath = hitdeath;
      MaxStrength = 1;
      ImpactDamage = 1;
      DamageType = DamageType.COLLISION;
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
      IsCombatObject = f.GetBoolValue(sectionname, "IsCombatObject", IsCombatObject);
      HitWhileDyingLeadsToDeath = f.GetBoolValue(sectionname, "HitWhileDyingLeadsToDeath", HitWhileDyingLeadsToDeath);

      MaxStrength = f.GetFloatValue(sectionname, "MaxStrength", MaxStrength);
      ImpactDamage = f.GetFloatValue(sectionname, "ImpactDamage", ImpactDamage);
      DamageType = f.GetEnumValue(sectionname, "DamageType", DamageType);
      IsLaser = f.GetBoolValue(sectionname, "IsLaser", IsLaser);

      ImpactCloseEnoughDistance = f.GetFloatValue(sectionname, "ImpactCloseEnoughDistance", ImpactCloseEnoughDistance);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetBoolValue(sectionname, "IsCombatObject", IsCombatObject);
      f.SetBoolValue(sectionname, "HitWhileDyingLeadsToDeath", HitWhileDyingLeadsToDeath);

      f.SetFloatValue(sectionname, "MaxStrength", MaxStrength);
      f.SetFloatValue(sectionname, "ImpactDamage", ImpactDamage);
      f.SetEnumValue(sectionname, "DamageType", DamageType);
      f.SetBoolValue(sectionname, "IsLaser", IsLaser);

      f.SetFloatValue(sectionname, "ImpactCloseEnoughDistance", ImpactCloseEnoughDistance);
    }
  }
}
