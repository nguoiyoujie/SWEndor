﻿using MTV3D65;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.Core;

namespace SWEndor.Game.AI.Actions
{
  internal class HyperspaceOut : ActionInfo
  {

    private static ObjectPool<HyperspaceOut> _pool;
    static HyperspaceOut() { _pool = ObjectPool<HyperspaceOut>.CreateStaticPool(() => { return new HyperspaceOut(); }, (a) => { a.Reset(); }); }

    private HyperspaceOut() : base("HyperspaceOut") { CanInterrupt = false; }

    public static HyperspaceOut GetOrCreate()
    {
      HyperspaceOut h = _pool.GetNew();

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

    }

    // parameters
    private TV_3DVECTOR Origin_Position = new TV_3DVECTOR();
    private static float Incre_Speed = 75000; //125000; //2500;
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

      owner.MoveData.Speed += Incre_Speed * engine.Game.TimeSinceRender;

      float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetGlobalPosition(), Origin_Position);
      if (dist >= FarEnoughDistance)
      {
        owner.HyperspaceFactor = 1;
        Complete = true;
      }
      else
      {
        owner.HyperspaceFactor = (dist / Incre_Speed).Clamp(0, 1);
      }
    }
  }
}
