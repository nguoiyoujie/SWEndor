using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Weapons;

namespace SWEndor.Actors.Components
{
  // TO-DO: Seperate Ammo, Cooldown and actor specific var from Weapons to ease assignment
  internal struct WeaponData
  {
    public WeaponInfo[] Weapons { get; private set; }
    public WeaponShotInfo[] PrimaryWeapons { get; private set; }
    public WeaponShotInfo[] SecondaryWeapons { get; private set; }
    public WeaponShotInfo[] AIWeapons { get; private set; }

    public void Init(WeapRegistry wreg, ref UnfixedWeaponData data)
    {
      this = data.Fix(wreg);
    }

    public WeaponData(int weapons, int primary, int secondary, int ai)
    {
      Weapons = new WeaponInfo[weapons];
      PrimaryWeapons = new WeaponShotInfo[primary];
      SecondaryWeapons = new WeaponShotInfo[secondary];
      AIWeapons = new WeaponShotInfo[ai];
    }

    public void Load(WeapRegistry wreg, UnfixedWeaponData preinit) { this = preinit.Fix(wreg); }
    
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
        if (ret < w.Targeter.Range)
          ret = w.Targeter.Range;
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
          if ((delta_angle < ws.Targeter.AngularRange
            && delta_angle > -ws.Targeter.AngularRange)
            && (delta_distance < ws.Targeter.Range
            && delta_distance > -ws.Targeter.Range)
            && ((actor.IsPlayer && !engine.PlayerInfo.PlayerAIEnabled) || ws.CanTarget( actor, target))
            && (ws.Ammo.Max == -1 || ws.Ammo.Count > 0))
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
