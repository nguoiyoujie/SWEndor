using SWEndor.FileFormat.INI;

namespace SWEndor.Weapons
{
  public class WeaponLoadoutInfo
  {
    public WeaponLoadoutInfo(INIFile file, string sectionname)
    {
      Name = sectionname;
      WeaponName = file.GetString(sectionname, "WeaponName", WeaponName);

      Primary = file.GetIntArray(sectionname, "Primary", Primary);
      Secondary = file.GetIntArray(sectionname, "Secondary", Secondary);
      AI = file.GetIntArray(sectionname, "AI", AI);
    }
    
    public readonly string Name = "Null Loadout";
    public readonly string WeaponName;

    public readonly int[] Primary = new int[0];
    public readonly int[] Secondary = new int[0];
    public readonly int[] AI = new int[0];
  }
}
