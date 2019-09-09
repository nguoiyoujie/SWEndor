namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// A light implementation of an Armor system, replaces old Damage and CollisionDamage coefficients
  /// </summary>
  public struct ArmorData
  {
    public readonly float Light;
    //public readonly float Heavy;
    //public readonly float Bomb;
    public readonly float Hull;
    // float readonly Heal;

    public ArmorData(float light, float hull)
    {
      Light = light;
      Hull = hull;
    }

    public static ArmorData Immune { get { return new ArmorData(); } }
    public static ArmorData Default { get { return new ArmorData(1, 1); } }
  }
}

