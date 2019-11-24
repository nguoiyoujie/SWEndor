using SWEndor.Actors.Components;
using SWEndor.Core;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Components
{
  internal struct UnfixedWeaponData
  {
    private List<WeapData> _weapons;
    private int _primary;
    private int _secondary;
    private int _ai;
    private List<UnfixedWeapon> _weaponclasses;

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

    public void Load(Engine engine, ActorTypeInfo atype)
    {
      this = new UnfixedWeaponData();
      _weapons = new List<WeapData>(4);
      _weaponclasses = new List<UnfixedWeapon>(8);

      foreach (WeapData s in atype.Loadouts)
      {
        InsertLoadout(engine, s);
      }

      if (atype.TrackerDummyWeapon)
        InsertDummyTrackerAILoadout();
    }

    public WeaponData Fix(WeapRegistry registry)
    {
      WeaponData d = new WeaponData(_weapons.Count, _primary, _secondary + 1, _ai);
      for (int i = 0; i < _weapons.Count; i++)
        d.Weapons[i] = registry.BuildWeapon(_weapons[i]);

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

    public void InsertLoadout(Engine engine, WeapData wdata)
    {
      WeapLoadInfo wload;
      engine.WeaponRegistry.Get(wdata.Load, out wload);

      int c = _weapons.Count;
      _weapons.Add(wdata);
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
    }
  }
}
