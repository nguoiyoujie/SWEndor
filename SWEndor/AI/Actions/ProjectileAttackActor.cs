using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primrose.Primitives.Factories;

namespace SWEndor.AI.Actions
{
  public class ProjectileAttackActor : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<ProjectileAttackActor> _pool = new ObjectPool<ProjectileAttackActor>(() => { return new ProjectileAttackActor(); }, (a) => { a.Reset(); });

    private ProjectileAttackActor() : base("ProjectileAttackActor") { }

    public static ProjectileAttackActor GetOrCreate(ActorInfo targetActor)
    {
      ProjectileAttackActor h = _pool.GetNew();
      _count++;
      h.Target_Actor = targetActor;
      h.ID = targetActor?.ID ?? -1;
      h.CanInterrupt = false;
      return h;
    }

    // parameters
    public ActorInfo Target_Actor = null;
    public int ID;
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , ID.ToString()
        , CanInterrupt.ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo target = Target_Actor;
      if (target == null || ID != target.ID)
      {
        Complete = true;
        return;
      }

      actor.AIData.SetTarget(engine, actor, target, true);
      actor.AIData.AdjustRotation(engine, actor);
      actor.AIData.SetTargetSpeed(actor.MoveData.MaxSpeed);
      actor.AIData.AdjustSpeed(actor);

      Complete |= (!target.Active || target.IsDyingOrDead);
    }

    public override void Reset()
    {
      base.Reset();
      Target_Actor = null;
      ID = -1;
      Target_Position = new TV_3DVECTOR();
    }

  public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }
  }
}
