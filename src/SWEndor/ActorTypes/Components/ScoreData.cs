using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct ScoreData
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
      PerStrength = f.GetInt(sectionname, "PerStrength", PerStrength);
      DestroyBonus = f.GetInt(sectionname, "DestroyBonus", DestroyBonus);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetInt(sectionname, "PerStrength", PerStrength);
      f.SetInt(sectionname, "DestroyBonus", DestroyBonus);
    }
  }
}

