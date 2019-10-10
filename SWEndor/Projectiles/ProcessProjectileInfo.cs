using SWEndor.Core;
using SWEndor.Primitives;
using System;

namespace SWEndor.Projectiles
{
  public partial class ProjectileInfo
  {
    internal static void Process(Engine engine, ProjectileInfo actor)
    {
#if DEBUG
      if (actor == null)
        throw new ArgumentNullException("actor");
#endif

      using (ScopeCounterManager.Acquire(actor.Scope))
      {
        if (!actor.Active)
          return;

        if (!actor.IsDead)
          actor.Update();

        actor.Tick(engine, engine.Game.TimeSinceRender);
      }
    }

    internal static void ProcessCollision(Engine engine, ProjectileInfo actor)
    {
#if DEBUG
      if (actor == null)
        throw new ArgumentNullException("actor");
#endif

      using (ScopeCounterManager.Acquire(actor.Scope))
        actor.CollisionData.TestCollision(engine, actor);
    }

    private void Update()
    {
      Meshes.Update(this);

      if (Generated)
        SetActivated();
    }
  }
}
