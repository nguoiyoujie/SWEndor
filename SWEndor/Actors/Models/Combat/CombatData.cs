using SWEndor.FileFormat.INI;

namespace SWEndor.Actors.Data
{
  public struct CombatData
  {
    public bool IsCombatObject;
    public bool HitWhileDyingLeadsToDeath;

    public CombatData Disabled { get { return new CombatData(false, false); } }
    public CombatData DefaultFighter { get { return new CombatData(true, true); } }
    public CombatData DefaultShip { get { return new CombatData(true, false); } }

    public CombatData(bool enabled, bool hitdeath)
    {
      IsCombatObject = enabled;
      HitWhileDyingLeadsToDeath = hitdeath;
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
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetBoolValue(sectionname, "IsCombatObject", IsCombatObject);
      f.SetBoolValue(sectionname, "HitWhileDyingLeadsToDeath", HitWhileDyingLeadsToDeath);
    }
  }
}
