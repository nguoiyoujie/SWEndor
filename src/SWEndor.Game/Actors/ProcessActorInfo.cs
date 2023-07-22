using SWEndor.Game.AI;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives;
using SWEndor.Game.Weapons;
using System;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Actors
{
  public partial class ActorInfo
  {
    internal static void Process(Engine engine, ActorInfo actor)
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

    internal static void ProcessAI(Engine engine, ActorInfo actor)
    {
#if DEBUG
      if (actor == null)
        throw new ArgumentNullException("actor");
#endif

      using (ScopeCounters.Acquire(actor.Scope))
      {
        if (!actor.Mask.Has(ComponentMask.HAS_AI)
        || !actor.Active
        || actor.IsDyingOrDead
        || (actor.IsPlayer && !engine.PlayerInfo.PlayerAIEnabled)
        )
          return;


        actor.Squad.Process(engine);
        actor.Run();
      }
    }

    internal static void ProcessCollision(Engine engine, ActorInfo actor)
    {
#if DEBUG
      if (actor == null)
        throw new ArgumentNullException("actor");
#endif

      using (ScopeCounters.Acquire(actor.Scope))
      {
        actor.CollisionData.TestCollision(engine, actor);
        actor.CollisionData.TestProspectiveCollision(engine, actor);
      }
    }

    internal static void FireWeapon(Engine engine, ActorInfo actor, ActorInfo target, WeaponShotInfo weapon)
    {
      if (actor == null)
        return;

      if (!actor.IsDyingOrDead)
        actor.TypeInfo.FireWeapon(engine, actor, target, weapon);
    }

    private void Update()
    {
      if (TypeInfo.RenderData.ZEaseInTime > 0)
        Transform.ZSqueeze = ((Engine.Game.GameTime - CreationTime - TypeInfo.RenderData.ZEaseInDelay) / TypeInfo.RenderData.ZEaseInTime).Clamp(0, 1);

      Meshes.Update(this);

      if (Generated || Reserved_Generated)
        SetActivated();

      Engine.GameScenarioManager.Scenario.State.Octree.Update(this);
    }
  }
}
