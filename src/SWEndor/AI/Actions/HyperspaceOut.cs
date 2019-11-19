using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class HyperspaceOut : ActionInfo
  {
    internal static int _count = 0;
    private static ObjectPool<HyperspaceOut> _pool;
    static HyperspaceOut() { _pool = ObjectPool<HyperspaceOut>.CreateStaticPool(() => { return new HyperspaceOut(); }, (a) => { a.Reset(); }); }

    private HyperspaceOut() : base("HyperspaceOut") { CanInterrupt = false; }

    public static HyperspaceOut GetOrCreate()
    {
      HyperspaceOut h = _pool.GetNew();
      _count++;
      h.IsDisposed = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
      Origin_Position = new TV_3DVECTOR();
      hyperspace = false;
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }

    // parameters
    private TV_3DVECTOR Origin_Position = new TV_3DVECTOR();
    private static float Incre_Speed = 125000; //2500;
    private static float FarEnoughDistance = 250000;
    private bool hyperspace = false;

    public override void Process(Engine engine, ActorInfo actor) { }

    public void ApplyMove(Engine engine, ActorInfo owner)
    {
      if (!hyperspace)
      {
        hyperspace = true;
        Origin_Position = owner.GetGlobalPosition();

        if (owner.IsScenePlayer)
        {
          ActorCreationInfo ac = new ActorCreationInfo(engine.ActorTypeFactory.Get("HYPER"));
          ActorInfo a = engine.ActorFactory.Create(ac);
          owner.AddChild(a);
          a.UseParentCoords = true;
        }
      }

      owner.MoveData.Speed += Incre_Speed * owner.Game.TimeSinceRender;

      float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetGlobalPosition(), Origin_Position);
      if (dist >= FarEnoughDistance)
        Complete = true;
    }
  }
}
