using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.Weapons
{
  public struct WeaponShotInfo
  {
    public readonly static WeaponShotInfo[] NullArrayCache = new WeaponShotInfo[0];

    public readonly WeaponInfo Weapon;
    public readonly int Burst;
    public bool IsNull { get { return Weapon.MaxAmmo == 0; } }

    // special cases
    public readonly static WeaponShotInfo Default = new WeaponShotInfo(null, 0);
    public readonly static WeaponShotInfo Automatic = new WeaponShotInfo(null, -1);

    public WeaponShotInfo(WeaponInfo weapon, int burst)
    {
      Weapon = weapon ?? NullWeapon.Instance;
      Burst = burst;
    }

    public static WeaponShotInfo Get(string name, int burst)
    {
      return new WeaponShotInfo(WeaponFactory.Get(name), burst);
    }

    public static WeaponShotInfo GetFromSource(string name, int burst, Dictionary<string, WeaponInfo> source)
    {
      return new WeaponShotInfo(source[name], burst);
    }

    public bool Fire(Engine engine, ActorInfo owner, ActorInfo target)
    {
      return Weapon.Fire(engine, owner, target, Burst);
    }

    public override string ToString()
    {
      return (Weapon is NullWeapon || Weapon is TrackerDummyWeapon) ? "NONE" : "{0}:{1}".F(Burst, Weapon.DisplayName.ToUpper());
    }
  }
}
