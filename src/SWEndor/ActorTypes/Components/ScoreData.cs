using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  public struct ScoreData
  {
    public int PerStrength;
    public int DestroyBonus;

    public ScoreData(int perStrength, int destroyBonus)
    {
      PerStrength = perStrength;
      DestroyBonus = destroyBonus;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      PerStrength = f.GetIntValue(sectionname, "PerStrength", PerStrength);
      DestroyBonus = f.GetIntValue(sectionname, "DestroyBonus", DestroyBonus);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetIntValue(sectionname, "PerStrength", PerStrength);
      f.SetIntValue(sectionname, "DestroyBonus", DestroyBonus);
    }
  }
}

