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

      if (actor.TypeInfo.NoProcess)
        return;

      if (!actor.ActorState.IsDead())
        actor.Update();

      actor.CycleInfo.Process();

      if (actor.Mesh != null)
      {
        actor.CheckState(engine);
        if (!actor.ActorState.IsDead())
        {
          if (actor.TypeInfo.CollisionEnabled
          || actor.TypeInfo is ActorTypes.Group.Projectile)
          {
            actor.CollisionInfo.CheckCollision();
          }
          actor.MovementInfo.Move(actor);
        }
      }
      actor.OnTickEvent(engine, new object[] { actor, actor.ActorState });
    }

    internal static void ProcessAI(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      if (actor == null)
        return;

      if (actor.TypeInfo.NoProcess
        || actor.TypeInfo.NoAI
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

      actor.CollisionInfo.TestCollision();
    }

    internal static void Render(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      if (actor == null)
        return;

      if (actor.TypeInfo.NoRender
        || actor.CreationState != CreationState.ACTIVE)
      {
        return;
      }

      if (!IsAggregateMode(engine, id))
      {
        if (!IsPlayer(engine, id) || engine.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION)
        {
          if (!IsFarMode(engine, id))
          {
            if (actor.Mesh != null && actor.Mesh.IsVisible())
              actor.Mesh.Render();

            if (actor.ParticleSystem != null)
              actor.ParticleSystem.Render();
          }
          else
          {
            if (actor.FarMesh != null && actor.FarMesh.IsVisible())
              actor.FarMesh.Render();
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
      CombatInfo.Process();
      TypeInfo.ProcessState(this);

      if (ActorState == ActorState.DEAD)
        ActorInfo.Kill(engine, ID);
    }

    private void Update()
    {
      PrevPosition = Position;

      if (Mesh == null)
        return;

      TV_3DVECTOR pos = GetPosition();
      TV_3DVECTOR rot = GetRotation();

      Mesh.SetPosition(pos.x, pos.y, pos.z);
      Mesh.SetRotation(rot.x, rot.y, rot.z);
      FarMesh.SetPosition(pos.x, pos.y, pos.z);
      FarMesh.SetRotation(rot.x, rot.y, rot.z);

      Mesh.SetCollisionEnable(!IsAggregateMode(Engine, ID) && TypeInfo.CollisionEnabled);
      FarMesh.SetCollisionEnable(false);

      if (CreationState == CreationState.GENERATED)
        CreationState = CreationState.ACTIVE;

      if (ParticleSystem != null && !IsFarMode(Engine, ID))
        ParticleSystem.Update();
    }
    
  }
}
