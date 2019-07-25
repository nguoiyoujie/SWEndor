using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;

namespace SWEndor.Actors.Data
{
  public interface Data
  {
    void Init(ActorTypeInfo type, ActorCreationInfo acinfo);
    void Reset();
  }

  public class DataCache<T> where T : Data
  {
    internal T[] list = new T[Globals.ActorLimit];
    public T this[int id] { get { return list[id % Globals.ActorLimit]; } }

    public void Init(int id, ActorTypeInfo type, ActorCreationInfo acinfo) { ((DoFunc<T, ActorTypeInfo, ActorCreationInfo>)((ref T d, ref ActorTypeInfo t, ref ActorCreationInfo ac) => { d.Init(t, ac); }))(ref list[id % Globals.ActorLimit], ref type, ref acinfo); }
    public void Reset(int id) { ((DoFunc<T>)((ref T d) => { d.Reset(); }))(ref list[id % Globals.ActorLimit]); }
    public void Do(int id, DoFunc<T> func) { func.Invoke(ref list[id % Globals.ActorLimit]); }
  }
}
