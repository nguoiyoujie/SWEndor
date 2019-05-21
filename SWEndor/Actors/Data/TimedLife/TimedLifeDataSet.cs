using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
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
}
