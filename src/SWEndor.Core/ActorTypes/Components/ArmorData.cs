using Primrose.Primitives.Factories;
using SWEndor.Actors;
using Primitives.FileFormat.INI;
using System;
using System.Collections.Generic;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// A light implementation of an Armor system, replaces old Damage and CollisionDamage coefficients
  /// </summary>
  internal struct ArmorData
  {
    private static DamageType[] _dmgs;

    static ArmorData()
    {
      Array a = Enum.GetValues(typeof(DamageType));
      List<DamageType> ls = new List<DamageType>(a.Length);
      for (int i = 0; i < a.Length; i++)
      {
        DamageType d = (DamageType)a.GetValue(i);
        if (!ls.Contains(d) && d != DamageType.NONE && d != DamageType.ALWAYS_100PERCENT)
          ls.Add(d);
      }
      _dmgs = ls.ToArray();
    }

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
