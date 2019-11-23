using SWEndor.FileFormat.INI;

namespace SWEndor.Weapons
{
  internal struct WeapLoadInfo
  {
    //public string WeaponName;
    public int[] Primary;
    public int[] Secondary;
    public int[] AI;

    public static WeapLoadInfo Default = new WeapLoadInfo
    {
      //WeaponName = null,
      Primary = new int[0],
      Secondary = new int[0],
      AI = new int[0]
    };

    public void LoadFromINI(INIFile f, string sectionname)
    {
      this = Default;
      //WeaponName = f.GetString(sectionname, "WeaponName", WeaponName);
      Primary = f.GetIntArray(sectionname, "Primary", Primary);
      Secondary = f.GetIntArray(sectionname, "Secondary", Secondary);
      AI = f.GetIntArray(sectionname, "AI", AI);
    }
  }
}
