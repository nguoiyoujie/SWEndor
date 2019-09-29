using SWEndor.Primitives;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public void TickExplosions()
    {
      using (ScopeCounterManager.Acquire(Scope))
        Explosions.Tick(this, Game.GameTime);
    }
  }
}
