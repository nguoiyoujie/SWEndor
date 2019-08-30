using SWEndor.ActorTypes;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public struct WeaponBit
  {
    public static readonly WeaponBit NULL = new WeaponBit(byte.MaxValue, byte.MaxValue);
    public readonly byte Index;
    public readonly byte Burst;

    public WeaponBit(byte index, byte burst) { Index = index; Burst = burst; }
    public bool IsNull { get { return Index == byte.MaxValue && Burst == byte.MaxValue; } }
  }

  public class PreWeaponSystemInfo
  {
    public List<WeaponInfo> Weapons = new List<WeaponInfo>(4);
    public List<WeaponShotInfo> PrimaryWeapons = new List<WeaponShotInfo>(4);
    public List<WeaponShotInfo> SecondaryWeapons = new List<WeaponShotInfo>(4) { WeaponShotInfo.Default };
    public List<WeaponShotInfo> AIWeapons = new List<WeaponShotInfo>(4);

    public PreWeaponSystemInfo() { }

    public PreWeaponSystemInfo(ActorTypeInfo atype)
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

  public struct WeaponSystemInfo
  {
    private static readonly WeaponInfo[] NullWeaponInfoArrayCache = new WeaponInfo[0];
    private static readonly WeaponShotInfo[] NullWeaponShotInfoCache = new WeaponShotInfo[0];

    public WeaponInfo[] Weapons;
    public WeaponShotInfo[] PrimaryWeapons;
    public WeaponShotInfo[] SecondaryWeapons;
    public WeaponShotInfo[] AIWeapons;

    public void Init(ActorTypeInfo atype)
    {
      Init(new PreWeaponSystemInfo(atype));
    }

    public void Init(PreWeaponSystemInfo preinit)
    {
      Weapons = preinit.Weapons.ToArray() ?? NullWeaponInfoArrayCache;
      PrimaryWeapons = preinit.PrimaryWeapons.ToArray() ?? NullWeaponShotInfoCache;
      SecondaryWeapons = preinit.SecondaryWeapons.ToArray() ?? NullWeaponShotInfoCache;
      AIWeapons = preinit.AIWeapons.ToArray() ?? NullWeaponShotInfoCache;
    }

    public void Reset()
    {
      Weapons = NullWeaponInfoArrayCache;
      PrimaryWeapons = NullWeaponShotInfoCache;
      SecondaryWeapons = NullWeaponShotInfoCache;
      AIWeapons = NullWeaponShotInfoCache;
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
      if (AIWeapons != null)
      {
        foreach (WeaponShotInfo wb in AIWeapons)
        {
          WeaponInfo ws = wb.Weapon;

          if (ws != null)
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
      }
      return false;
    }
  }
}
