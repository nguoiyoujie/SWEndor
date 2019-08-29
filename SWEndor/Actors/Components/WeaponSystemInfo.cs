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

  public struct WeaponSystemInfo
  {
    private static readonly WeaponInfo[] NullWeaponInfoArrayCache = new WeaponInfo[0];
    private static readonly WeaponShotInfo[] NullWeaponBitArrayCache = new WeaponShotInfo[0];

    private List<WeaponInfo> m_weapons;
    private List<WeaponShotInfo> m_primary;
    private List<WeaponShotInfo> m_secondary;
    private List<WeaponShotInfo> m_ai;

    public WeaponInfo[] Weapons { get { return m_weapons?.ToArray() ?? NullWeaponInfoArrayCache; } }
    public WeaponShotInfo[] PrimaryWeapons { get { return m_primary?.ToArray() ?? NullWeaponBitArrayCache; } }
    public WeaponShotInfo[] SecondaryWeapons { get { return m_secondary?.ToArray() ?? NullWeaponBitArrayCache; } }
    public WeaponShotInfo[] AIWeapons { get { return m_ai?.ToArray() ?? NullWeaponBitArrayCache; } }

    public void Init(ActorTypeInfo atype)
    {
      m_weapons = new List<WeaponInfo>();
      m_primary = new List<WeaponShotInfo>();
      m_secondary = new List<WeaponShotInfo> { WeaponShotInfo.Default };
      m_ai = new List<WeaponShotInfo>();

      foreach (string s in atype.Loadouts)
        InsertLoadout(s);

      if (atype.TrackerDummyWeapon)
        InsertDummyTrackerAILoadout();
    }

    public void Reset()
    {
      m_weapons.Clear();
      m_primary.Clear();
      m_secondary.Clear();
      m_secondary.Add(WeaponShotInfo.Default);
      m_ai.Clear();
    }

    public void InsertLoadout(string wload)
    {
      InsertLoadout(WeaponLoadoutFactory.Get(wload));
    }

    private void InsertLoadout(WeaponLoadoutInfo wload)
    {
      WeaponInfo weap = WeaponFactory.Get(wload.WeaponName);
      m_weapons.Add(weap);
      foreach (int p in wload.Primary)
        m_primary.Add(new WeaponShotInfo(weap, p));

      foreach (int p in wload.Secondary)
        m_secondary.Add(new WeaponShotInfo(weap, p));

      foreach (int p in wload.AI)
        m_ai.Add(new WeaponShotInfo(weap, p));
    }

    private void InsertDummyTrackerAILoadout()
    {
      WeaponInfo weap = TrackerDummyWeapon.Instance;
      m_ai.Add(new WeaponShotInfo(weap, 1));
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
      return false;
    }
  }
}
