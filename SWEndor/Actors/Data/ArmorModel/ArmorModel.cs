using SWEndor.ActorTypes;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    /// <summary>
    /// A light implementation of an Armor system, replaces old Damage and CollisionDamage coefficients
    /// </summary>
    public struct ArmorModel
    {
      public float Light;
      //public float Heavy;
      //public float Bomb;
      public float Hull;
      // float Heal;

      public static ArmorModel Immune { get { return new ArmorModel(); } }
      public static ArmorModel Default { get { return new ArmorModel { Light = 1, Hull = 1 }; } }


      public void Init(ActorTypeInfo type)
      {
        this = type.Armor;
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

    public void SetArmor(DamageType dmgtype, float value) { Armor.Set(dmgtype, value); }
    public float GetArmor(DamageType dmgtype) { return Armor.Get(dmgtype); }

  }
}
