using SWEndor.Actors.Components;
using SWEndor.Actors.Traits;
using SWEndor.AI;
using SWEndor.Player;
using System;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    internal static void Process(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      if (!actor.Active)
        return;

      IStateModel s = actor.StateModel;
      if (!s.IsDead)
        using (engine.PerfManager.Create("actor_update"))
          actor.Update();

      using (engine.PerfManager.Create("actor_cycleinfo"))
        actor.CycleInfo.Process();

      foreach (ITick t in actor.TraitsImplementing<ITick>())
        using (engine.PerfManager.Create("itick"))
          t.Tick(actor, engine.Game.TimeSinceRender);

      actor.CheckState(engine);
      if (!s.IsDead)
      {
        if (actor.StateModel.ComponentMask.Has(ComponentMask.CAN_BECOLLIDED) || actor.TypeInfo is ActorTypes.Groups.Projectile)
          using (engine.PerfManager.Create("actor_collisioncheck"))
            CollisionSystem.CheckCollision(engine, actor);

        using (engine.PerfManager.Create("actor_move"))
          actor.MoveComponent.Move(actor, ref actor.MoveData);
      }
      using (engine.PerfManager.Create("actor_OnTickEvent()"))
        actor.OnTickEvent();
    }

    /*
    internal static void PostFrameProcess(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      if (!actor.Active)
        return;

      foreach (Action<ActorInfo> a in actor.PostFrameActions.ToArray()) // or consider ConcurrentQueue?
        using (engine.PerfManager.Create("actor_postframeaction"))
          a.Invoke(actor);
    }
    */

    internal static void ProcessAI(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      if (!actor.StateModel.ComponentMask.Has(ComponentMask.HAS_AI))
        return;

      if (!actor.Active)
        return;

      if (actor.StateModel.IsDyingOrDead)
        return;

      if (actor.IsPlayer && !engine.PlayerInfo.PlayerAIEnabled)
        return;

      using (engine.PerfManager.Create("actor_ai"))
        actor.Run(actor.CurrentAction);
    }

    internal static void ProcessCollision(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      using (engine.PerfManager.Create("actor_collision"))
        CollisionSystem.TestCollision(engine, actor);
    }

    internal static void Render(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      if (!actor.StateModel.ComponentMask.Has(ComponentMask.CAN_RENDER))
        return;

      if (!actor.Active)
        return;

      if (actor.IsAggregateMode)
        return;

      if (!actor.IsPlayer || engine.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION)
        using (engine.PerfManager.Create("actor_rendermesh"))
          actor.TraitOrDefault<IMeshModel>()?.Render(actor.IsFarMode);
    }

    internal void FireWeapon(ActorInfo target, string weapon) // actor, target not null
    {
      if (StateModel != null && !StateModel.IsDyingOrDead)
        TypeInfo.FireWeapon(this, target, weapon);
    }

    private void CheckState(Engine engine)
    {
      TypeInfo.ProcessState(this);

      if (StateModel.IsDead)
        Kill();
    }

    private void Update()
    {
      TraitOrDefault<IMeshModel>()?.Update(this);

      if (StateModel != null && StateModel.CreationState == CreationState.GENERATED)
        StateModel.SetActivated();
    }

  }
}
