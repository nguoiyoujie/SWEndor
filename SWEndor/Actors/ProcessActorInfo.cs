using SWEndor.Actors.Components;
using SWEndor.AI;
using SWEndor.Primitives;
using SWEndor.Weapons;
using System;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    internal static void Process(Engine engine, ActorInfo actor)
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

        actor.Tick(engine.Game.TimeSinceRender);
      }
    }

    internal static void ProcessAI(Engine engine, ActorInfo actor)
    {
#if DEBUG
      if (actor == null)
        throw new ArgumentNullException("actor");
#endif

      using (ScopeCounterManager.Acquire(actor.Scope))
      {
        if (!engine.MaskDataSet[actor].Has(ComponentMask.HAS_AI)
        || !actor.Active
        || actor.IsDyingOrDead
        || (actor.IsPlayer && !engine.PlayerInfo.PlayerAIEnabled)
        )
          return;

        actor.Squad.Process(engine);
        actor.Run(actor.CurrentAction);
      }
    }

    internal static void ProcessCollision(Engine engine, ActorInfo actor)
    {
#if DEBUG
      if (actor == null)
        throw new ArgumentNullException("actor");
#endif

      using (ScopeCounterManager.Acquire(actor.Scope))
        CollisionSystem.TestCollision(engine, actor);
    }

    internal static void FireWeapon(Engine engine, ActorInfo actor, ActorInfo target, WeaponShotInfo weapon)
    {
      if (actor == null)
        return;

      if (!actor.IsDyingOrDead)
        actor.TypeInfo.FireWeapon(actor, target, weapon);
    }

    private void Update()
    {
      Meshes.Update(this);

      if (Generated)
        SetActivated();
    }
  }
}
