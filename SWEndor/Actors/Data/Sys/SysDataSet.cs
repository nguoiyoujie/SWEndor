using MTV3D65;
using SWEndor.ActorTypes;
using System.Buffers;
using System.Runtime.InteropServices;

namespace SWEndor.Actors.Data
{
  public class SysDataSet
  {
    //private static int datasize = Marshal.SizeOf(typeof(SysData));
    //private IMemoryOwner<byte> mem;

    public SysDataSet(Engine engine)
    {
      //mem = engine.Memory.Pool.Rent(Globals.ActorLimit * datasize);
    }


    internal SysData[] list = new SysData[Globals.ActorLimit];
    public SysData this[int id] { get { return list[id % Globals.ActorLimit]; } }

    public void Init(int id, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<SysData, ActorTypeInfo, ActorCreationInfo>)((ref SysData d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref list[id % Globals.ActorLimit], ref type, ref acinfo); }
    public void Reset(int id) { ((DoFunc<SysData>)((ref SysData d) => { d.Reset(); }))(ref list[id % Globals.ActorLimit]); }
    public void Do(int id, DoFunc<SysData> func) { func.Invoke(ref list[id % Globals.ActorLimit]); }

    // Strength { get; set; }
    public void Strength_set(int id, float value) { ((SetFunc<SysData, float>)((ref SysData d, float v) => { d.Strength = v; }))(ref list[id % Globals.ActorLimit], value); }
    public float Strength_get(int id) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.Strength; }))(ref list[id % Globals.ActorLimit]); }

    // MaxStrength { get; set; }
    public void MaxStrength_set(int id, float value) { ((SetFunc<SysData, float>)((ref SysData d, float v) => { d.MaxStrength = v; }))(ref list[id % Globals.ActorLimit], value); }
    public float MaxStrength_get(int id) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.MaxStrength; }))(ref list[id % Globals.ActorLimit]); }

    // StrengthFrac { get; }
    public float StrengthFrac_get(int id) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.StrengthFrac; }))(ref list[id % Globals.ActorLimit]); }

    // StrengthColor { get; }
    public TV_COLOR StrengthColor_get(int id) { return ((GetFunc<SysData, TV_COLOR>)((ref SysData d) => { return d.StrengthColor; }))(ref list[id % Globals.ActorLimit]); }
  }

  /*  public unsafe class SysDataSet
  {
    private static int datasize = Marshal.SizeOf(typeof(SysData));
    private IMemoryOwner<byte> mem;

    public SysDataSet(Engine engine)
    {
      mem = engine.Memory.Pool.Rent(Globals.ActorLimit * datasize);
    }


    //internal SysData[] list = new SysData[Globals.ActorLimit];
    public SysData this[int id] { get { return Get(id); } }

    public SysData* Get(int id)
    {
      int index = id % Globals.ActorLimit;
      return MemoryMarshal.Cast<byte, SysData>(mem.Memory.Span.Slice(id * datasize, datasize)).;
    }

    public void Init(int id, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<SysData, ActorTypeInfo, ActorCreationInfo>)((ref SysData d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref Get(id), ref type, ref acinfo); }
    public void Reset(int id) { ((DoFunc<SysData>)((ref SysData d) => { d.Reset(); }))(ref Get(id)); }
    public void Do(int id, DoFunc<SysData> func) { func.Invoke(ref Get(id)); }

    // Strength { get; set; }
    public void Strength_set(int id, float value) { ((SetFunc<SysData, float>)((ref SysData d, float v) => { d.Strength = v; }))(ref Get(id), value); }
    public float Strength_get(int id) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.Strength; }))(ref Get(id)); }

    // MaxStrength { get; set; }
    public void MaxStrength_set(int id, float value) { ((SetFunc<SysData, float>)((ref SysData d, float v) => { d.MaxStrength = v; }))(ref Get(id), value); }
    public float MaxStrength_get(int id) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.MaxStrength; }))(ref Get(id)); }

    // StrengthFrac { get; }
    public float StrengthFrac_get(int id) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.StrengthFrac; }))(ref Get(id)); }

    // StrengthColor { get; }
    public TV_COLOR StrengthColor_get(int id) { return ((GetFunc<SysData, TV_COLOR>)((ref SysData d) => { return d.StrengthColor; }))(ref Get(id)); }
  }
  */
}
