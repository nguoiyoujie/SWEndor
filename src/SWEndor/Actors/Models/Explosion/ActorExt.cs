using Primrose.Primitives;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    /// <summary>Processes the explosions once</summary>
    public void TickExplosions()
    {
      using (ScopeCounters.Acquire(Scope))
        Explosions.Tick(this, Game.GameTime);
    }
  }
}
