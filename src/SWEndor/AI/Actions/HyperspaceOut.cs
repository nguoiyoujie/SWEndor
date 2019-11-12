﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class HyperspaceOut : ActionInfo
  {
    public HyperspaceOut() : base("HyperspaceOut")
    {
      CanInterrupt = false;
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