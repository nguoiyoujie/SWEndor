using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  /// <summary>
  /// A light implementation of an Armor system, replaces old Damage and CollisionDamage coefficients
  /// </summary>
  internal struct ArmorData
  {
    [INIRegistry]
    internal readonly Registry<DamageType, float> Data;

    internal readonly float DefaultMult;

    public ArmorData(float def)
    {
      Data = new Registry<DamageType, float>();
      DefaultMult = def;
    }

    public static ArmorData Immune { get { return new ArmorData(0); } }
    public static ArmorData Default { get { return new ArmorData(1); } }
  }
}
