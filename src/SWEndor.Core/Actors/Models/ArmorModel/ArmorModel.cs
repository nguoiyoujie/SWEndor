using Primrose.Primitives.Factories;
using SWEndor.ActorTypes.Components;

namespace SWEndor.Actors.Models
{
  /// <summary>
  /// A light implementation of an Armor system
  /// </summary>
  internal struct ArmorModel
  {
    private Registry<DamageType, float> _list;

    public ArmorModel(float all)
    {
      _list = new Registry<DamageType, float>();
      _list.Default = 1;
    }

    public void Init(ref ArmorData data)
    {
      if (_list == null)
        _list = new Registry<DamageType, float>();
      else
        _list.Clear();

      _list.Default = data.DefaultMult;
      foreach (DamageType d in data.Data.GetKeys())
        _list.Add(d, data.Data[d]);
    }

    public float Get(DamageType dmgtype)
    {
      switch (dmgtype)
      {
        case DamageType.NONE:
          return 0;
        case DamageType.ALWAYS_100PERCENT:
          return 1;
        default:
          return _list.Get(dmgtype);
      }
    }

    public void Set(DamageType dmgtype, float value)
    {
      switch (dmgtype)
      {
        case DamageType.NONE:
        case DamageType.ALWAYS_100PERCENT:
          break;

        default:
          _list.Put(dmgtype, value);
          break;
      }
    }

    public void SetAll(float value)
    {
      _list.Clear();
      _list.Default = value;
    }
  }
}
