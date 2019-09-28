using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.ExplosionTypes;
using SWEndor.Primitives;

namespace SWEndor.Actors
{
  public interface ILinked<T>
  {
    T Prev { get; set; }
    T Next { get; set; }
  }

  public interface IScoped
  {
    ScopeCounterManager.ScopeCounter Scope { get; }
  }

  public interface ICreate<T>
  { }

  public interface IActor
  {
    Engine Engine { get; }
    bool DisposingOrDisposed { get; }
    float CreationTime { get; }

    void Rebuild(Engine engine, int id, ActorCreationInfo acinfo);
    void Initialize(Engine engine);
    void Destroy();
    void Delete();
  }

  public interface IExplosion
  {
    Engine Engine { get; }
    bool DisposingOrDisposed { get; }
    float CreationTime { get; }

    void Rebuild(Engine engine, int id, ExplosionCreationInfo acinfo);
    void Initialize(Engine engine);
    void Destroy();
    void Delete();
  }
}