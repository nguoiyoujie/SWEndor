using SWEndor.Game.Core;
using Primrose.Primitives;
using System;

namespace SWEndor.Game.Explosions
{
  public partial class ExplosionInfo
  {
    internal static void ProcessExp(Engine engine, ExplosionInfo expl)
    {
#if DEBUG
      if (expl == null)
        throw new ArgumentNullException("expl");
#endif

      using (ScopeCounters.Acquire(expl.Scope))
      {
        if (!expl.Active)
          return;

        if (!expl.IsDead)
          expl.Update();

        expl.Tick(engine, engine.Game.TimeSinceRender);
      }
    }

    private void Update()
    {
      Meshes.Update(this);

      if (Generated)
        SetActivated();
    }
  }
}
