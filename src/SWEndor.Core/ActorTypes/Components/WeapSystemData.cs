using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct WeapSystemData
  {
    private static WeapData[] NullWeap = new WeapData[0];
    internal WeapData[] Loadouts;
    internal bool TrackerDummyWeapon;

    public static WeapSystemData Default { get { return new WeapSystemData(NullWeap, false); } }

    public WeapSystemData(WeapData[] loadouts, bool track)
    {
      Loadouts = loadouts;
      TrackerDummyWeapon = track;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      f.GetBool(sectionname, "TrackerDummyWeapon", TrackerDummyWeapon);
      WeapData.LoadFromINI(f, sectionname, "Weapon", out Loadouts);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetBool(sectionname, "TrackerDummyWeapon", TrackerDummyWeapon);
      WeapData.SaveToINI(f, sectionname, "Weapon", "WEAP", Loadouts);
    }
  }
}
