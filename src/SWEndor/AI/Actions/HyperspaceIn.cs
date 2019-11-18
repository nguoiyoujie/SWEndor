using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;

namespace SWEndor.AI.Actions
{
  internal class HyperspaceIn : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<HyperspaceIn> _pool = new ObjectPool<HyperspaceIn>(() => { return new HyperspaceIn(); }, (a) => { a.Reset(); });

    private HyperspaceIn() : base("HyperspaceIn") { CanInterrupt = false; }

    public static HyperspaceIn GetOrCreate(TV_3DVECTOR target_position)
    {
      HyperspaceIn h = _pool.GetNew();
      _count++;
      h.Target_Position = target_position;
      h.IsDisposed = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }

    // parameters
    private TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    internal static float Max_Speed = 75000;
    private static float SpeedDistanceFactor = 15;
    private static float CloseEnoughDistance = 500;
    private bool hyperspace = false;
    private float prevdist = 9999999;
    internal float distance;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Target_Position.Str()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor) { }

    public void ApplyMove(Engine engine, ActorInfo owner)
    {
      distance = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetGlobalPosition(), Target_Position);

      if (distance <= CloseEnoughDistance || prevdist < distance)
      {
        owner.MoveData.Speed = owner.MoveData.MaxSpeed;
        Complete = true;
      }
      else
      {
        if (!hyperspace)
        {
          hyperspace = true;
          owner.LookAt(Target_Position);
        }

        owner.MoveData.Speed = owner.MoveData.MaxSpeed + distance * SpeedDistanceFactor;
        if (owner.MoveData.Speed > Max_Speed)
          owner.MoveData.Speed = Max_Speed;

      }
      prevdist = distance;
    }
  }
}
