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
        actor.Update();

      actor.CycleInfo.Process();

      foreach (ITick t in actor.TraitsImplementing<ITick>())
        t.Tick(actor, engine.Game.TimeSinceRender);

      actor.CheckState(engine);
      if (!s.IsDead)
      {
        if (actor.StateModel.ComponentMask.Has(ComponentMask.CAN_BECOLLIDED) || actor.TypeInfo is ActorTypes.Groups.Projectile)
          CollisionSystem.CheckCollision(engine, actor);

        actor.MoveComponent.Move(actor, ref actor.MoveData);
      }
      actor.OnTickEvent();
    }

    internal static void PostFrameProcess(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      if (!actor.Active)
        return;

      foreach (Action<ActorInfo> a in actor.PostFrameActions.ToArray()) // or consider ConcurrentQueue?
        a.Invoke(actor);
    }

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

      actor.Run(actor.CurrentAction);
    }

    internal static void ProcessCollision(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

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
      {
        actor.TraitOrDefault<IMeshModel>()?.Render(actor.IsFarMode);
      }
    }

    internal static void FireWeapon(Engine engine, int id, int targetActorID, string weapon)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      ActorInfo target = engine.ActorFactory.Get(targetActorID);
      if (actor == null)
        return;

      IStateModel s = actor.StateModel;
      if (s != null && !s.IsDyingOrDead)
        actor.TypeInfo.FireWeapon(actor, target, weapon);
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
      //MeshSystem.Update(Engine, ID);

      if (StateModel != null && StateModel.CreationState == CreationState.GENERATED)
        StateModel.CreationState = CreationState.ACTIVE;
    }
    
  }
}
