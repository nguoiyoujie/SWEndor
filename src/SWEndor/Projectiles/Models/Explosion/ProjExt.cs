using Primrose.Primitives;

namespace SWEndor.Projectiles
{
  public partial class ProjectileInfo
  {
    /// <summary>Processes the explosions once</summary>
    public void TickExplosions()
    {
      using (ScopeCounters.Acquire(Scope))
        Explosions.Tick(this, Engine.Game.GameTime);
    }
  }
}
