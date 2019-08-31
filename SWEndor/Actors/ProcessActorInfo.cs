using SWEndor.Actors.Components;
using SWEndor.AI;
using SWEndor.Player;
using SWEndor.Weapons;

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

      actor.Tick(engine.Game.TimeSinceRender);
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

      actor.Squad.Process(engine);
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
        && actor.Active
        && !actor.IsAggregateMode)
      {
        if (!actor.IsPlayer || engine.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION)
        {
          actor.Render(actor.IsFarMode);
        }
      }
    }

    internal static void FireWeapon(Engine engine, ActorInfo actor, ActorInfo target, WeaponShotInfo weapon)
    {
      if (actor == null)
        return;

      if (!actor.IsDyingOrDead)
        actor.TypeInfo.FireWeapon(actor, target, weapon);
    }

    private void CheckState(Engine engine)
    {
      TypeInfo.ProcessState(this);

      if (IsDead)
        Delete();
    }

    private void Update()
    {
      Meshes.Update(this);

      if (Generated)
        SetActivated();
    }
  }
}
