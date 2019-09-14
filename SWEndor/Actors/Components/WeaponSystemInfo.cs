using SWEndor.ActorTypes;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public class UnfixedWeaponData
  {
    private List<string> _weapons = new List<string>(4);
    private int _primary;
    private int _secondary;
    private int _ai;
    private List<UnfixedWeapon> _weaponclasses = new List<UnfixedWeapon>(8);

    internal enum WeaponClass { PRIMARY, SECONDARY, AI }
    internal struct UnfixedWeapon
    {
      public readonly int Order;
      public readonly WeaponClass Class;
      public readonly int Burst;

      public UnfixedWeapon(int order, WeaponClass wclass, int burst)
      {
        Order = order;
        Class = wclass;
        Burst = burst;
      }
    }

    public UnfixedWeaponData() { }

    public UnfixedWeaponData(ActorTypeInfo atype)
    {
      foreach (string s in atype.Loadouts)
        InsertLoadout(s);

      if (atype.TrackerDummyWeapon)
        InsertDummyTrackerAILoadout();
    }

    public WeaponData Fix()
    {
      WeaponData d = new WeaponData(_weapons.Count, _primary, _secondary + 1, _ai);
      for (int i = 0; i < _weapons.Count; i++)
        d.Weapons[i] = WeaponFactory.Get(_weapons[i]);

      int p = 0;
      int s = 1;
      int a = 0;
      d.SecondaryWeapons[0] = WeaponShotInfo.Default;

      for (int i = 0; i < _weaponclasses.Count; i++)
      {
        if (_weaponclasses[i].Class == WeaponClass.PRIMARY)
          d.PrimaryWeapons[p++] = new WeaponShotInfo(d.Weapons[_weaponclasses[i].Order], _weaponclasses[i].Burst);

        if (_weaponclasses[i].Class == WeaponClass.SECONDARY)
          d.SecondaryWeapons[s++] = new WeaponShotInfo(d.Weapons[_weaponclasses[i].Order], _weaponclasses[i].Burst);

        if (_weaponclasses[i].Class == WeaponClass.AI)
          d.AIWeapons[a++] = new WeaponShotInfo(d.Weapons[_weaponclasses[i].Order], _weaponclasses[i].Burst);
      }

      if (a != _ai)
        d.AIWeapons[a++] = new WeaponShotInfo(TrackerDummyWeapon.Instance, 1);

      return d;
    }

    public void InsertLoadout(string wload)
    {
      InsertLoadout(WeaponLoadoutFactory.Get(wload));
    }

    private void InsertLoadout(WeaponLoadoutInfo wload)
    {
      int c = _weapons.Count;
      _weapons.Add(wload.WeaponName);
      foreach (int p in wload.Primary)
      {
        _primary++;
        _weaponclasses.Add(new UnfixedWeapon(c, WeaponClass.PRIMARY, p));
      }

      foreach (int p in wload.Secondary)
      {
        _secondary++;
        _weaponclasses.Add(new UnfixedWeapon(c, WeaponClass.SECONDARY, p));
      }

      foreach (int p in wload.AI)
      {
        _ai++;
        _weaponclasses.Add(new UnfixedWeapon(c, WeaponClass.AI, p));
      }
    }

    private void InsertDummyTrackerAILoadout()
    {
      _ai++;
      //WeaponInfo weap = TrackerDummyWeapon.Instance;
      //AIWeapons.Add(new WeaponShotInfo(weap, 1));
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
      //Init(new UnfixedWeaponData(atype)); //atype.cachedWeaponData);
      this = atype.cachedWeaponData.Fix();
    }

    public WeaponData(int weapons, int primary, int secondary, int ai)
    {
      Weapons = new WeaponInfo[weapons];
      PrimaryWeapons = new WeaponShotInfo[primary];
      SecondaryWeapons = new WeaponShotInfo[secondary];
      AIWeapons = new WeaponShotInfo[ai];
    }

    public void Init(UnfixedWeaponData preinit)
    {
      this = preinit.Fix();
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
