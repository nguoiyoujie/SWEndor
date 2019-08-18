using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public class WeaponSystemInfo
  {
    private readonly ActorInfo Actor;
    public Dictionary<string, WeaponInfo> Weapons;
    public string[] PrimaryWeapons;
    public string[] SecondaryWeapons;
    public string[] AIWeapons;

    public WeaponSystemInfo(ActorInfo actor)
    {
      Actor = actor;

      Weapons = new Dictionary<string, WeaponInfo>();
      PrimaryWeapons = new string[0];
      SecondaryWeapons = new string[0];
      AIWeapons = new string[0];
    }

    public void Reset()
    {
      Weapons = new Dictionary<string, WeaponInfo>();
      PrimaryWeapons = new string[0];
      SecondaryWeapons = new string[0];
      AIWeapons = new string[0];
    }

    public float GetWeaponRange()
    {
      float ret = 0;
      foreach (WeaponInfo w in Weapons.Values)
      {
        if (ret < w.Range)
          ret = w.Range;
      }
      return ret;
    }

    public bool SelectWeapon(int targetActorID, float delta_angle, float delta_distance, out WeaponInfo weapon, out int burst)
    {
      weapon = null;
      burst = 0;
      WeaponInfo weap = null;
      foreach (string ws in AIWeapons)
      {
        Actor.TypeInfo.InterpretWeapon(Actor.ID, ws, out weap, out burst);

        if (weap != null)
        {
          if ((delta_angle < weap.AngularRange
            && delta_angle > -weap.AngularRange)
            && (delta_distance < weap.Range
            && delta_distance > -weap.Range)
            && weap.CanTarget(Actor.Engine, Actor.ID, targetActorID)
            && (weap.MaxAmmo == -1 || weap.Ammo > 0))
          {
            weapon = weap;
            return true;
          }
        }
      }
      return false;
    }
  }
}
