using SWEndor.Actors.Components;
using SWEndor.AI;
using SWEndor.Player;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    internal static void Process(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      if (!actor.IsDead)
        actor.Update();

      actor.CycleInfo.Process();

      actor.CheckState(engine);
      if (!actor.IsDead)
      {
        if (engine.MaskDataSet[actor].Has(ComponentMask.CAN_BECOLLIDED)
        || actor.TypeInfo is ActorTypes.Groups.Projectile)
        {
          CollisionSystem.CheckCollision(engine, actor);
        }
        actor.MoveComponent.Move(actor, ref actor.MoveData);
      }
      actor.OnTickEvent();
    }

    internal static void ProcessAI(Engine engine, ActorInfo actor)
    {
      if (actor == null)
        return;

      if (!engine.MaskDataSet[actor].Has(ComponentMask.HAS_AI)
        || !actor.Active
        || actor.IsDyingOrDead
        || (actor.IsPlayer && !engine.PlayerInfo.PlayerAIEnabled)
        )
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

      if (engine.MaskDataSet[actor].Has(ComponentMask.CAN_RENDER)
        //&& !engine.MaskDataSet[id].Has(ComponentMask.CAN_GLOW)
        && actor.Active
        && !actor.IsAggregateMode)
      {
        if (!actor.IsPlayer || engine.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION)
        {
          if (!actor.IsFarMode)
          {
            MeshSystem.RenderMesh(engine, actor);
          }
          else
          {
            MeshSystem.RenderFarMesh(engine, actor);
          }
        }
      }
    }

    internal static void FireWeapon(Engine engine, ActorInfo actor, ActorInfo target, string weapon)
    {
      if (actor == null)
        return;

      if (!actor.IsDyingOrDead)
        actor.TypeInfo.FireWeapon(actor, target, weapon);
    }

    private void CheckState(Engine engine)
    {
      CombatSystem.Process(engine, this);
      TypeInfo.ProcessState(this);

      if (IsDead)
        Kill();
    }

    private void Update()
    {
      MeshSystem.Update(Engine, this);

      if (Generated)
        SetActivated();
    }
    
  }
}
