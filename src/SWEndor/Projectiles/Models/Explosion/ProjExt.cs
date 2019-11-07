using Primrose.Primitives;

namespace SWEndor.Projectiles
{
  public partial class ProjectileInfo
  {
    public void TickExplosions()
    {
      using (ScopeCounterManager.Acquire(Scope))
        Explosions.Tick(this, Game.GameTime);
    }
  }
}
