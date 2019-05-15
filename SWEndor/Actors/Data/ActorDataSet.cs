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
    //internal TimedLifeData[] TimedLifeData = new TimedLifeData[Globals.ActorLimit];
    internal RegenData[] RegenData = new RegenData[Globals.ActorLimit];
  }

  public delegate void DoFunc<T>(ref T data);
  public delegate void DoFunc<T1, T2>(ref T1 d1, ref T2 d2);
  public delegate void DoFunc<T1, T2, T3>(ref T1 d1, ref T2 d2, ref T3 d3);
  public delegate V GetFunc<T, V>(ref T data);
  public delegate V GetFunc<T1, T2, V>(ref T1 data, ref T2 param);
  public delegate void SetFunc<T, V>(ref T data, V value);

  public class MaskDataSet
  {
    private ComponentMask[] list = new ComponentMask[Globals.ActorLimit];
    public ComponentMask this[int id] { get { return list[id % Globals.ActorLimit]; } set { list[id % Globals.ActorLimit] = value; } }

    public bool Contains(int id, ComponentMask match) { return (this[id] & match) == match; }
  }

  public class SysDataSet
  {
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

  public class TimedLifeDataSet
  {
    internal TimedLifeData[] list = new TimedLifeData[Globals.ActorLimit];
    public TimedLifeData this[int id] { get { return list[id % Globals.ActorLimit]; } }

    public void Init(int id, ActorTypeInfo type) { ((DoFunc<TimedLifeData, ActorTypeInfo>)((ref TimedLifeData d, ref ActorTypeInfo t) => { d.CopyFrom(type.TimedLifeData); }))(ref list[id % Globals.ActorLimit], ref type); }
    public void Reset(int id) { ((DoFunc<TimedLifeData>)((ref TimedLifeData d) => { d.Reset(); }))(ref list[id % Globals.ActorLimit]); }
    public void Do(int id, DoFunc<TimedLifeData> func) { func.Invoke(ref list[id % Globals.ActorLimit]); }

    // OnTimedLife { get; set; }
    public void OnTimedLife_set(int id, bool value) { ((SetFunc<TimedLifeData, bool>)((ref TimedLifeData d, bool v) => { d.OnTimedLife = v; }))(ref list[id % Globals.ActorLimit], value); }
    public bool OnTimedLife_get(int id) { return ((GetFunc<TimedLifeData, bool>)((ref TimedLifeData d) => { return d.OnTimedLife; }))(ref list[id % Globals.ActorLimit]); }

    // TimedLife { get; set; }
    public void TimedLife_set(int id, float value) { ((SetFunc<TimedLifeData, float>)((ref TimedLifeData d, float v) => { d.TimedLife = v; }))(ref list[id % Globals.ActorLimit], value); }
    public float TimedLife_get(int id) { return ((GetFunc<TimedLifeData, float>)((ref TimedLifeData d) => { return d.TimedLife; }))(ref list[id % Globals.ActorLimit]); }
  }


  public class MeshDataSet
  {
    internal MeshData[] list = new MeshData[Globals.ActorLimit];
    public MeshData this[int id] { get { return list[id % Globals.ActorLimit]; } }

    public void Init(int id, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<MeshData, ActorTypeInfo, ActorCreationInfo>)((ref MeshData d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref list[id % Globals.ActorLimit], ref type, ref acinfo); }
    public void Reset(int id) { ((DoFunc<MeshData>)((ref MeshData d) => { d.Reset(); }))(ref list[id % Globals.ActorLimit]); }
    public void Do(int id, DoFunc<MeshData> func) { func.Invoke(ref list[id % Globals.ActorLimit]); }

    public bool Mesh_getInitialized(int id) { return ((GetFunc<MeshData, bool>)((ref MeshData d) => { return d.Initialized; }))(ref list[id % Globals.ActorLimit]); }


    // Scale { get; set; }
    public void Scale_set(int id, float value) { ((SetFunc<MeshData, float>)((ref MeshData d, float v) => { d.Scale = v; }))(ref list[id % Globals.ActorLimit], value); }
    public float Scale_get(int id) { return ((GetFunc<MeshData, float>)((ref MeshData d) => { return d.Scale; }))(ref list[id % Globals.ActorLimit]); }

    // Mesh { get; functions; }
    public TVMesh Mesh_get(int id) { return ((GetFunc<MeshData, TVMesh>)((ref MeshData d) => { return d.Mesh; }))(ref list[id % Globals.ActorLimit]); }
    public void Mesh_generate(int id, ActorTypeInfo type) { ((DoFunc<MeshData, int, ActorTypeInfo>)((ref MeshData d, ref int i, ref ActorTypeInfo t) => { d.Generate(i, t); }))(ref list[id % Globals.ActorLimit], ref id, ref type); }
    public bool Mesh_getIsVisible(int id) { return ((GetFunc<MeshData, bool>)((ref MeshData d) => { return d.Mesh?.IsVisible() ?? false; }))(ref list[id % Globals.ActorLimit]); }

    public int Mesh_getVertexCount(int id) { return ((GetFunc<MeshData, int>)((ref MeshData d) => { return d.Mesh?.GetVertexCount() ?? 0; }))(ref list[id % Globals.ActorLimit]); }
    public TV_3DVECTOR Mesh_getVertex(int id, int vertexID)
    {
      return ((GetFunc<MeshData, int, TV_3DVECTOR>)((ref MeshData d, ref int vID) => {
      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;
      d.Mesh.GetVertex(vID, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
      return new TV_3DVECTOR(x, y, z); }))(ref list[id % Globals.ActorLimit], ref vertexID);
    }

    public void Mesh_destroy(int id) { ((DoFunc<MeshData>)((ref MeshData d) => { d.Mesh.Destroy(); }))(ref list[id % Globals.ActorLimit]); }

  }
}
