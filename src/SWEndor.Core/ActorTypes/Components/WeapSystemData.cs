using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct WeapSystemData
  {
    private const string sWeapon = "Weapon";
    private static WeapData[] NullWeap = new WeapData[0];

    [INISubSectionList(sWeapon, "WEAP", "Weapon")]
    internal WeapData[] Loadouts;

    [INIValue(sWeapon, "TrackerDummyWeapon")]
    internal bool TrackerDummyWeapon;

    public static WeapSystemData Default { get { return new WeapSystemData(NullWeap, false); } }

    public WeapSystemData(WeapData[] loadouts, bool track)
    {
      Loadouts = loadouts;
      TrackerDummyWeapon = track;
    }
  }
}
