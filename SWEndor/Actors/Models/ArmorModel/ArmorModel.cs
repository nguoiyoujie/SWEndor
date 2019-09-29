using SWEndor.ActorTypes;

namespace SWEndor.Actors.Models
{
  /// <summary>
  /// A light implementation of an Armor system
  /// </summary>
  public struct ArmorModel
  {
    public float Light { get; private set; }
    //public float Heavy;
    //public float Bomb;
    public float Hull { get; private set; }
    // float Heal;

    public static ArmorModel Immune { get { return new ArmorModel(); } }
    public static ArmorModel Default { get { return new ArmorModel { Light = 1, Hull = 1 }; } }

    public void Init(ActorTypeInfo type)
    {
      Light = type.ArmorData.Light;
      Hull = type.ArmorData.Hull;
    }

    public float Get(DamageType dmgtype)
    {
      switch (dmgtype)
      {
        case DamageType.NORMAL:
          return Light;
        case DamageType.COLLISION:
          return Hull;
        default:
          return 1;
      }
    }

    public void Set(DamageType dmgtype, float value)
    {
      switch (dmgtype)
      {
        case DamageType.NORMAL:
          Light = value;
          break;
        case DamageType.COLLISION:
          Hull = value;
          break;
        case DamageType.ALL:
          Light = value;
          Hull = value;
          break;
      }
    }
  }
}
