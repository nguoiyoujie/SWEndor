using SWEndor.Actors;
using SWEndor.ActorTypes;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// A light implementation of an Armor system, replaces old Damage and CollisionDamage coefficients
  /// </summary>
  public struct ArmorInfo
  {
    public float Light;
    //public float Heavy;
    //public float Bomb;
    public float Hull;
    // float Heal;

    public static ArmorInfo Immune { get { return new ArmorInfo(); } }
    public static ArmorInfo Default { get { return new ArmorInfo { Light = 1, Hull = 1 }; } }
  }
}

