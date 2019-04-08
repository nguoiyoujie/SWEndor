using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public struct WeaponSystemInfo
  {
    private readonly ActorInfo Actor;
    public Dictionary<string, WeaponInfo> Weapons;
    public string[] PrimaryWeapons;
    public string[] SecondaryWeapons;
    public string[] AIWeapons;

    public WeaponSystemInfo(ActorInfo actor)
    {
      Actor = actor;

      Weapons = new Dictionary<string, WeaponInfo>();
      PrimaryWeapons = new string[0];
      SecondaryWeapons = new string[0];
      AIWeapons = new string[0];
    }

    public void Reset()
    {
      Weapons = new Dictionary<string, WeaponInfo>();
      PrimaryWeapons = new string[0];
      SecondaryWeapons = new string[0];
      AIWeapons = new string[0];
    }
  }
}
