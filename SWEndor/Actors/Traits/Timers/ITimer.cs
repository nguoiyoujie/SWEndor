using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  public interface ITimer<T> : ITrait
  {
    void Init(ActorTypeInfo type);
    T Start();
    T Pause();
    T Set(float time);
  }
}
