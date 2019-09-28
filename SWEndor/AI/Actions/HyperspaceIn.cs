﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Primitives;

namespace SWEndor.AI.Actions
{
  public class HyperspaceIn : ActionInfo
  {
    public HyperspaceIn(TV_3DVECTOR target_position) : base("HyperspaceIn")
    {
      Target_Position = target_position;
      CanInterrupt = false;
    }

    // parameters
    private TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    private static float Max_Speed = 75000;
    private static float SpeedDistanceFactor = 15; //2.5f;
    private static float CloseEnoughDistance = 500;
    private bool hyperspace = false;
    private float prevdist = 9999999;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Utilities.ToString(Target_Position)
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor) { }

    public void ApplyMove(Engine engine, ActorInfo owner)
    {
      float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetGlobalPosition(), Target_Position);

      if (dist <= CloseEnoughDistance || prevdist < dist)
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

        owner.MoveData.Speed = owner.MoveData.MaxSpeed + dist * SpeedDistanceFactor;
        if (owner.MoveData.Speed > Max_Speed)
          owner.MoveData.Speed = Max_Speed;

      }
      prevdist = dist;
    }
  }
}
