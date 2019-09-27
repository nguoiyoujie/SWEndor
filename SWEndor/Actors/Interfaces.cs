using SWEndor.ActorTypes;
using SWEndor.Primitives;

namespace SWEndor.Actors
{
  public interface ILinked<T> //where T : Linked<T>
  {
    T Prev { get; set; }
    T Next { get; set; }
  }

  public interface IScoped
  {
    ScopeCounterManager.ScopeCounter Scope { get; }
  }

  public interface IActor
  {
    Engine Engine { get; }
    bool DisposingOrDisposed { get; }
    float CreationTime { get; }

    void Rebuild(int id, ActorCreationInfo acinfo);
    void Initialize();
    void Destroy();
    void Delete();
  }
}