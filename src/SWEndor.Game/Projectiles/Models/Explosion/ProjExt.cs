﻿using Primrose.Primitives;

namespace SWEndor.Game.Projectiles
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