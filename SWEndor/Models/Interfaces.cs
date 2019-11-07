using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primrose.Primitives;

namespace SWEndor.Models
{
  public interface ICreationInfo<T, TType> where TType : ITypeInfo<T>
  {
    TType TypeInfo { get; }
  }

  public interface ITypeInfo<T>
  { }

  public interface ILinked<T>
  {
    T Prev { get; set; }
    T Next { get; set; }
  }

  public interface ITyped<TType>
  {
    TType TypeInfo { get; }
  }

  public interface IScoped
  {
    ScopeCounterManager.ScopeCounter Scope { get; }
  }

  public interface IActorDisposable
  {
    bool DisposingOrDisposed { get; }
    bool Disposed { get; }
    void Destroy();
    void Delete();
  }

  public interface IEngineObject
  {
    Engine Engine { get; }
    short ID { get; }
  }

  public interface IActorCreateable<T>
  {
    float CreationTime { get; }
    void Rebuild(Engine engine, short id, T acinfo);
    void Initialize(Engine engine);
  }

  public interface IActorState
  {
    ActorState ActorState { get; }
    void SetState_Dead();
  }

  public interface IParent<T>
  {
    T ParentForCoords { get; }
  }

  public interface INotify
  {
    void OnStateChangeEvent();
  }

  public interface IDyingTime
  {
    float DyingDuration { get; }
    float DyingTimeRemaining { get; }
  }

  public interface IMeshObject
  {
    int GetVertexCount();
    TV_3DVECTOR GetVertex(int vertID);
    float Scale { get; }
    TV_3DVECTOR MaxDimensions { get; }
    TV_3DVECTOR MinDimensions { get; }
  }

  public interface ITransformable
  {
    TV_3DVECTOR Position { get; }
    TV_3DMATRIX GetWorldMatrix();
    TV_3DMATRIX GetPrevWorldMatrix();
    TV_3DVECTOR GetGlobalPosition();
    TV_3DVECTOR GetPrevGlobalPosition();
    TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool local = false);
  }

  public interface ICollidable
  {
    bool CanCollide { get; }
    bool CanCollideWith(ActorInfo checkActor);
    void DoCollide(ActorInfo target, ref CollisionResultData data);
    void EnableCollision(bool enable);
  }
}