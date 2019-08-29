using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Groups;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Primitives;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.Weapons
{
  public struct WeaponShotInfo
  {
    public readonly WeaponInfo Weapon;
    public readonly int Burst;

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
      return (Weapon is NullWeapon || Weapon is TrackerDummyWeapon) ? "none" : "{0}:{1}".F(Burst, Weapon.DisplayName.ToUpper());
    }
  }
}
