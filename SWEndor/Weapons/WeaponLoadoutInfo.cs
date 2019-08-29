using SWEndor.FileFormat.INI;

namespace SWEndor.Weapons
{
  public class WeaponLoadoutInfo
  {
    public WeaponLoadoutInfo(INIFile file, string sectionname)
    {
      Name = sectionname;
      WeaponName = file.GetStringValue(sectionname, "WeaponName", WeaponName);

      Primary = file.GetIntList(sectionname, "Primary", Primary);
      Secondary = file.GetIntList(sectionname, "Secondary", Secondary);
      AI = file.GetIntList(sectionname, "AI", AI);
    }
    
    public readonly string Name = "Null Loadout";
    public readonly string WeaponName;

    public readonly int[] Primary = new int[0];
    public readonly int[] Secondary = new int[0];
    public readonly int[] AI = new int[0];
  }
}
