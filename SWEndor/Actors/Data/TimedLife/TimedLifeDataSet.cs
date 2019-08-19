using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public class TimedLifeDataSet
  {
    internal TimedLifeData[] list = new TimedLifeData[Globals.ActorLimit];
    public TimedLifeData this[ActorInfo actor] { get { return list[actor.dataID]; } }

    public void Init(ActorInfo actor, ActorTypeInfo type) { ((DoFunc<TimedLifeData, ActorTypeInfo>)((ref TimedLifeData d, ref ActorTypeInfo t) => { d.CopyFrom(type.TimedLifeData); }))(ref list[actor.dataID], ref type); }
    public void Reset(ActorInfo actor) { ((DoFunc<TimedLifeData>)((ref TimedLifeData d) => { d.Reset(); }))(ref list[actor.dataID]); }
    public void Do(ActorInfo actor, DoFunc<TimedLifeData> func) { func.Invoke(ref list[actor.dataID]); }

    // OnTimedLife { get; set; }
    public void OnTimedLife_set(ActorInfo actor, bool value) { ((SetFunc<TimedLifeData, bool>)((ref TimedLifeData d, bool v) => { d.OnTimedLife = v; }))(ref list[actor.dataID], value); }
    public bool OnTimedLife_get(ActorInfo actor) { return ((GetFunc<TimedLifeData, bool>)((ref TimedLifeData d) => { return d.OnTimedLife; }))(ref list[actor.dataID]); }

    // TimedLife { get; set; }
    public void TimedLife_set(ActorInfo actor, float value) { ((SetFunc<TimedLifeData, float>)((ref TimedLifeData d, float v) => { d.TimedLife = v; }))(ref list[actor.dataID], value); }
    public float TimedLife_get(ActorInfo actor) { return ((GetFunc<TimedLifeData, float>)((ref TimedLifeData d) => { return d.TimedLife; }))(ref list[actor.dataID]); }
  }
}
