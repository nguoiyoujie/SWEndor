using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Weapons;

namespace SWEndor.Actors.Components
{
  // TO-DO: Seperate Ammo, Cooldown and actor specific var from Weapons to ease assignment
  public struct WeaponData
  {
    public WeaponInfo[] Weapons { get; private set; }
    public WeaponShotInfo[] PrimaryWeapons { get; private set; }
    public WeaponShotInfo[] SecondaryWeapons { get; private set; }
    public WeaponShotInfo[] AIWeapons { get; private set; }

    public void Init(WeaponFactory wfact, ref UnfixedWeaponData data)
    {
      this = data.Fix(wfact);
    }

    public WeaponData(int weapons, int primary, int secondary, int ai)
    {
      Weapons = new WeaponInfo[weapons];
      PrimaryWeapons = new WeaponShotInfo[primary];
      SecondaryWeapons = new WeaponShotInfo[secondary];
      AIWeapons = new WeaponShotInfo[ai];
    }

    public void Load(WeaponFactory wfact, UnfixedWeaponData preinit) { this = preinit.Fix(wfact); }
    
    public void Reset()
    {
      Weapons = WeaponInfo.NullArrayCache;
      PrimaryWeapons = WeaponShotInfo.NullArrayCache;
      SecondaryWeapons = WeaponShotInfo.NullArrayCache;
      AIWeapons = WeaponShotInfo.NullArrayCache;
    }

    public float GetWeaponRange()
    {
      float ret = 0;
      foreach (WeaponInfo w in Weapons)
      {
        if (ret < w.Range)
          ret = w.Range;
      }
      return ret;
    }

    public bool SelectWeapon(Engine engine, ActorInfo actor, ActorInfo target, float delta_angle, float delta_distance, out WeaponShotInfo weapon)
    {
      weapon = WeaponShotInfo.Default;
      foreach (WeaponShotInfo wb in AIWeapons)
      {
        WeaponInfo ws = wb.Weapon;

        if (!wb.IsNull)
        {
          if ((delta_angle < ws.AngularRange
            && delta_angle > -ws.AngularRange)
            && (delta_distance < ws.Range
            && delta_distance > -ws.Range)
            && ((actor.IsPlayer && !engine.PlayerInfo.PlayerAIEnabled) || ws.CanTarget( actor, target))
            && (ws.MaxAmmo == -1 || ws.Ammo > 0))
          {
            weapon = new WeaponShotInfo(ws, wb.Burst);
            return true;
          }

        }
      }
      return false;
    }
  }
}
