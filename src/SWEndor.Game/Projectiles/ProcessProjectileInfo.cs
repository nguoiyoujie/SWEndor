using SWEndor.Game.Core;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Projectiles
{
  public partial class ProjectileInfo
  {
    internal static void Process(Engine engine, ProjectileInfo actor)
    {
#if DEBUG
      if (actor == null)
        throw new ArgumentNullException("actor");
#endif

      using (ScopeCounters.Acquire(actor.Scope))
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

      using (ScopeCounters.Acquire(actor.Scope))
        actor.CollisionData.TestCollision(engine, actor);
    }

    private void Update()
    {
      if (TypeInfo.RenderData.ZEaseInTime > 0)
        Transform.ZSqueeze = ((Engine.Game.GameTime - CreationTime - TypeInfo.RenderData.ZEaseInDelay) / TypeInfo.RenderData.ZEaseInTime).Clamp(0, 1);

      Meshes.Update(this);

      if (Generated)
        SetActivated();
    }
  }
}
