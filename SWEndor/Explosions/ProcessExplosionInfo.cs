using SWEndor.Actors.Components;
using SWEndor.AI;
using SWEndor.Core;
using SWEndor.Primitives;
using SWEndor.Weapons;
using System;

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo
  {
    internal static void ProcessExp(Engine engine, ExplosionInfo expl)
    {
#if DEBUG
      if (expl == null)
        throw new ArgumentNullException("expl");
#endif

      using (ScopeCounterManager.Acquire(expl.Scope))
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
