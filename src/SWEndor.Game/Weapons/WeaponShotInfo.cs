using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Weapons.Types;

namespace SWEndor.Game.Weapons
{
  public struct WeaponShotInfo
  {
    public readonly static WeaponShotInfo[] NullArrayCache = new WeaponShotInfo[0];

    public readonly WeaponInfo Weapon;
    public readonly int Burst;
    public bool IsNull { get { return Weapon.Ammo.Max == 0; } }

    // special cases
    public readonly static WeaponShotInfo Default = new WeaponShotInfo(null, 0);
    public readonly static WeaponShotInfo Automatic = new WeaponShotInfo(null, -1);

    public WeaponShotInfo(WeaponInfo weapon, int burst)
    {
      Weapon = weapon ?? NullWeapon.Instance;
      Burst = burst;
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
