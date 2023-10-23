using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Core;

namespace SWEndor.Game.Particles
{
  public partial class ParticleInfo
  {
    internal static void Process(Engine engine, ParticleInfo actor)
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

    private void Update()
    {
      if (TypeInfo.RenderData.ZEaseInTime > 0)
        Transform.ZSqueeze = ((Engine.Game.GameTime - CreationTime - TypeInfo.RenderData.ZEaseInDelay) / TypeInfo.RenderData.ZEaseInTime).Clamp(0, 1);

      if (Generated)
        SetActivated();
    }
  }
}