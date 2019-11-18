using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using Primrose.Primitives.Factories;

namespace SWEndor.AI.Actions
{
  internal class Evade : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<Evade> _pool = new ObjectPool<Evade>(() => { return new Evade(); }, (a) => { a.Reset(); });

    private Evade() : base("Evade") { }

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughAngle = 0.1f;
    private float WaitTime = 0;
    private float ResumeTime = 0;
    private bool poschecked = false;

    public static Evade GetOrCreate(float time = 2.5f)
    {
      Evade h = _pool.GetNew();
      _count++;
      h.WaitTime = time;
      h.CanInterrupt = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
      Target_Position = new TV_3DVECTOR();
      Target_Speed = 0;
      CloseEnoughAngle = 0.1f;
      WaitTime = 0;
      ResumeTime = 0;
      poschecked = false;
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , (ResumeTime - Globals.Engine.Game.GameTime).ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.MoveData.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (ResumeTime == 0)
      {
        ResumeTime = engine.Game.GameTime + WaitTime;
      }

      if (CheckBounds(actor))
      {
        if (!poschecked)
        {
          poschecked = true;
          TV_3DVECTOR minb = engine.GameScenarioManager.MinAIBounds;
          TV_3DVECTOR maxb = engine.GameScenarioManager.MaxAIBounds;

          if (actor.IsOutOfBounds(minb, maxb))
            // TO-DO: use the center of the Bound volume or a designated center point instead of origin.
            actor.AI.SetTarget(new TV_3DVECTOR());
          else
            actor.AI.SetTarget(actor.GetRelativePositionXYZ(1000, engine.Random.Next(-500, 500), engine.Random.Next(-500, 500)));
        }

        actor.AI.SetTargetSpeed(actor.MoveData.MaxSpeed);
        float delta_angle = actor.AI.AdjustRotation(engine, actor, 20);
        float delta_speed = actor.AI.AdjustSpeed(actor);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
        Complete |= (ResumeTime < engine.Game.GameTime);
      }

      if (CheckImminentCollision(actor))
        CreateAvoidAction(actor);
    }
  }
}
