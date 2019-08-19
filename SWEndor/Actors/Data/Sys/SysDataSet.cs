using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public class SysDataSet
  {
    public SysDataSet(Engine engine)
    {
    }


    internal SysData[] list = new SysData[Globals.ActorLimit];
    public SysData this[ActorInfo actor] { get { return list[actor.dataID]; } }

    public void Init(ActorInfo actor, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<SysData, ActorTypeInfo, ActorCreationInfo>)((ref SysData d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref list[actor.dataID], ref type, ref acinfo); }
    public void Reset(ActorInfo actor) { ((DoFunc<SysData>)((ref SysData d) => { d.Reset(); }))(ref list[actor.dataID]); }
    public void Do(ActorInfo actor, DoFunc<SysData> func) { func.Invoke(ref list[actor.dataID]); }

    // Strength { get; set; }
    public void Strength_set(ActorInfo actor, float value) { ((SetFunc<SysData, float>)((ref SysData d, float v) => { d.Strength = v; }))(ref list[actor.dataID], value); }
    public float Strength_get(ActorInfo actor) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.Strength; }))(ref list[actor.dataID]); }

    // MaxStrength { get; set; }
    public void MaxStrength_set(ActorInfo actor, float value) { ((SetFunc<SysData, float>)((ref SysData d, float v) => { d.MaxStrength = v; }))(ref list[actor.dataID], value); }
    public float MaxStrength_get(ActorInfo actor) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.MaxStrength; }))(ref list[actor.dataID]); }

    // StrengthFrac { get; }
    public float StrengthFrac_get(ActorInfo actor) { return ((GetFunc<SysData, float>)((ref SysData d) => { return d.StrengthFrac; }))(ref list[actor.dataID]); }

    // StrengthColor { get; }
    public TV_COLOR StrengthColor_get(ActorInfo actor) { return ((GetFunc<SysData, TV_COLOR>)((ref SysData d) => { return d.StrengthColor; }))(ref list[actor.dataID]); }
  }
}
