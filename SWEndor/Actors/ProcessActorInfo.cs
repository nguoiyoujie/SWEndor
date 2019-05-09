using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.Player;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    internal static void Process(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      if (actor == null)
        return;

      if (!actor.ActorState.IsDead())
        actor.Update();

      actor.CycleInfo.Process();

      if (engine.MeshDataSet.Mesh_get(id) != null)
      {
        actor.CheckState(engine);
        if (!actor.ActorState.IsDead())
        {
          if (engine.MaskDataSet[id].Has(ComponentMask.CAN_BECOLLIDED)
          || actor.TypeInfo is ActorTypes.Groups.Projectile)
          {
            CollisionSystem.CheckCollision(engine, id);
          }
          actor.MoveComponent.Move(actor, ref actor.MoveData);
        }
      }
      actor.OnTickEvent();
    }

    internal static void ProcessAI(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      if (actor == null)
        return;

      if (engine.MaskDataSet[id].Has(ComponentMask.HAS_AI)
        || actor.CreationState != CreationState.ACTIVE
        || actor.ActorState.IsDyingOrDead()
        || (IsPlayer(engine, id) && !engine.PlayerInfo.PlayerAIEnabled)
        )
        return;

      engine.ActionManager.Run(id, actor.CurrentAction);
    }

    internal static void ProcessCollision(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);

      if (actor == null)
        return;

      CollisionSystem.TestCollision(engine, id);
    }

    internal static void Render(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      if (actor == null)
        return;

      if (engine.MaskDataSet[id].Has(ComponentMask.CAN_RENDER)
        && actor.CreationState == CreationState.ACTIVE
        && !IsAggregateMode(engine, id))
      {
        if (!IsPlayer(engine, id) || engine.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION)
        {
          if (!IsFarMode(engine, id))
          {
            MeshSystem.RenderMesh(engine, id);
          }
          else
          {
            MeshSystem.RenderFarMesh(engine, id);
          }
        }
      }
    }

    internal static void FireWeapon(Engine engine, int id, int targetActorID, string weapon)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      if (actor == null)
        return;

      if (!actor.ActorState.IsDyingOrDead())
        actor.TypeInfo.FireWeapon(id, targetActorID, weapon);
    }

    private void CheckState(Engine engine)
    {
      CombatSystem.Process(engine, ID);
      TypeInfo.ProcessState(this);

      if (ActorState == ActorState.DEAD)
        ActorInfo.Kill(engine, ID);
    }

    private void Update()
    {
      //PrevPosition = Position;

      if (Engine.MeshDataSet.Mesh_get(ID) == null)
        return;

      MeshSystem.Update(Engine, ID);

      if (CreationState == CreationState.GENERATED)
        CreationState = CreationState.ACTIVE;

      //if (ParticleSystem != null && !IsFarMode(Engine, ID))
      //  ParticleSystem.Update();
    }
    
  }
}
