using MTV3D65;
using SWEndor.Core;
using SWEndor.Primitives;

namespace SWEndor.Models
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

  public interface IActorDisposable
  {
    bool DisposingOrDisposed { get; }
    bool Disposed { get; }
    void Destroy();
    void Delete();
  }

  public interface IActorCreateable<T>
  {
    Engine Engine { get; }
    float CreationTime { get; }
    void Rebuild(Engine engine, int id, T acinfo);
    void Initialize(Engine engine);
  }

  public interface IParent<T>
  {
    T ParentForCoords { get; }
  }

  public interface INotify
  {
    void OnTickEvent();
    void OnStateChangeEvent();
    void OnCreatedEvent();
    void OnDestroyedEvent();
  }

  public interface ITransformable
  {
    TV_3DMATRIX GetWorldMatrix();
    TV_3DMATRIX GetPrevWorldMatrix();
  }
}