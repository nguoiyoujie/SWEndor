using MTV3D65;
using SWEndor.ActorTypes;
using System;

namespace SWEndor.Actors.Data
{
  public class ActorDataSet
  {
    //public CoordData[] CoordData = new CoordData[Globals.ActorLimit];

    internal CollisionData[] CollisionData = new CollisionData[Globals.ActorLimit];

    internal ExplodeData[] ExplodeData = new ExplodeData[Globals.ActorLimit];
    internal CombatData[] CombatData = new CombatData[Globals.ActorLimit];
    internal RegenData[] RegenData = new RegenData[Globals.ActorLimit];
  }

  public delegate void DoFunc<T>(ref T data);
  public delegate void DoFunc<T1, T2>(ref T1 d1, ref T2 d2);
  public delegate void DoFunc<T1, T2, T3>(ref T1 d1, ref T2 d2, ref T3 d3);
  public delegate void DoFunc<T1, T2, T3, T4>(ref T1 d1, ref T2 d2, ref T3 d3, ref T4 d4);
  public delegate V GetFunc<T, V>(ref T data);
  public delegate V GetFunc<T1, T2, V>(ref T1 data, ref T2 param);
  public delegate void SetFunc<T, V>(ref T data, V value);

  public class MaskDataSet
  {
    private ComponentMask[] list = new ComponentMask[Globals.ActorLimit];
    public ComponentMask this[int id] { get { return list[id % Globals.ActorLimit]; } set { list[id % Globals.ActorLimit] = value; } }

    public bool Contains(int id, ComponentMask match) { return (this[id] & match) == match; }
  }
}
