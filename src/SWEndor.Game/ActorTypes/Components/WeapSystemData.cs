using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct WeapSystemData
  {
    private static WeapData[] NullWeap = new WeapData[0];

    [INISubSectionList(Key = "Weapon", SubsectionPrefix = "WEAP")]
    internal WeapData[] Loadouts;

    [INIValue]
    internal bool TrackerDummyWeapon;

    public static WeapSystemData Default { get { return new WeapSystemData(NullWeap, false); } }

    public WeapSystemData(WeapData[] loadouts, bool track)
    {
      Loadouts = loadouts;
      TrackerDummyWeapon = track;
    }
  }
}
