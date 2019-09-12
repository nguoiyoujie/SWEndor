using SWEndor.ActorTypes;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public class UnfixedWeaponData
  {
    public List<WeaponInfo> Weapons = new List<WeaponInfo>(4);
    public List<WeaponShotInfo> PrimaryWeapons = new List<WeaponShotInfo>(4);
    public List<WeaponShotInfo> SecondaryWeapons = new List<WeaponShotInfo>(4) { WeaponShotInfo.Default };
    public List<WeaponShotInfo> AIWeapons = new List<WeaponShotInfo>(4);

    public UnfixedWeaponData() { }

    public UnfixedWeaponData(ActorTypeInfo atype)
    {
      foreach (string s in atype.Loadouts)
        InsertLoadout(s);

      if (atype.TrackerDummyWeapon)
        InsertDummyTrackerAILoadout();
    }

    public void Reset()
    {
      Weapons.Clear();
      PrimaryWeapons.Clear();
      SecondaryWeapons.Clear();
      SecondaryWeapons.Add(WeaponShotInfo.Default);
      AIWeapons.Clear();
    }

    public void InsertLoadout(string wload)
    {
      InsertLoadout(WeaponLoadoutFactory.Get(wload));
    }

    private void InsertLoadout(WeaponLoadoutInfo wload)
    {
      WeaponInfo weap = WeaponFactory.Get(wload.WeaponName);
      Weapons.Add(weap);
      foreach (int p in wload.Primary)
        PrimaryWeapons.Add(new WeaponShotInfo(weap, p));

      foreach (int p in wload.Secondary)
        SecondaryWeapons.Add(new WeaponShotInfo(weap, p));

      foreach (int p in wload.AI)
        AIWeapons.Add(new WeaponShotInfo(weap, p));
    }

    private void InsertDummyTrackerAILoadout()
    {
      WeaponInfo weap = TrackerDummyWeapon.Instance;
      AIWeapons.Add(new WeaponShotInfo(weap, 1));
    }
  }

  public struct WeaponData
  {
    public WeaponInfo[] Weapons { get; private set; }
    public WeaponShotInfo[] PrimaryWeapons { get; private set; }
    public WeaponShotInfo[] SecondaryWeapons { get; private set; }
    public WeaponShotInfo[] AIWeapons { get; private set; }

    public void Init(ActorTypeInfo atype)
    {
      // TO DO: Seperate Ammo, Cooldown and actor specific var from Weapons to ease assignment
      Init(new UnfixedWeaponData(atype)); //atype.cachedWeaponData);
    }

    public void Init(UnfixedWeaponData preinit) // to split into Actor and Atype versions
    {
      Weapons = preinit.Weapons.ToArray();
      PrimaryWeapons = preinit.PrimaryWeapons.ToArray();
      SecondaryWeapons = preinit.SecondaryWeapons.ToArray();
      AIWeapons = preinit.AIWeapons.ToArray();
    }

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
            && ws.CanTarget(engine, actor, target)
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
