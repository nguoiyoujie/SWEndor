using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.Game.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior with full range of movement, but reacts erratically when ENGINE or SIDE_THRUSTERS are disabled
  /// </summary>
  internal static class MoveMissile
  {
    internal static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
    {
      // Hyperspace special: AI loop may not be in sync
      if (actor.CurrentAction is HyperspaceIn)
      {
        ((HyperspaceIn)actor.CurrentAction).ApplyMove(engine, actor);
        actor.MoveRelative(data.Speed * time);
        return;
      }
      else if (actor.CurrentAction is HyperspaceOut)
      {
        ((HyperspaceOut)actor.CurrentAction).ApplyMove(engine, actor);
        actor.MoveRelative(data.Speed * time);
        return;
      }

      float dx = data.XTurnAngle;
      float dy = data.YTurnAngle;

      // Steering haywire
      if (!actor.IsSystemOperational(SystemPart.SIDE_THRUSTERS))
      {
        // Wild movement
        float ebb = actor.StunDurationRemaining.Max(1) * 2.5f;
        dx = ((float)engine.Random.NextDouble() - 0.5f) * ebb * data.MaxTurnRate;
        dy = ((float)engine.Random.NextDouble() - 0.5f) * ebb * actor.StunDurationRemaining * data.MaxTurnRate;
        data.ApplyZBalance = false;
        //data.ZRoll = ((float)engine.Random.NextDouble() - 0.5f) * 2.5f * data.MaxTurnRate / 5;
      }

      // A missile typically maintains full speed regardless
      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      // move
      actor.MoveRelative(data.Speed * time, 0, 0);

      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.Rotation;
        if (actor.IsPlayer)
        {
          vec = Utilities.RotateXYZ(engine, vec, dx * time, dy * time, data.ZRoll * time);
        }
        else
        {
          vec += new TV_3DVECTOR(dx * time, dy * time, (-dy * data.ZTilt + data.ZRoll) * time);
          data.ZRoll = -vec.z;
        }
        data.ZRoll *= (float)(Math.Pow(1 - data.ZNormFrac, time / 0.01f));
        actor.Rotation = vec;
      }
      else
      {
        TV_3DVECTOR vec = actor.Rotation;
        vec = Utilities.RotateXY(engine, vec, dx * time, dy * time);
        actor.Rotation = vec;
      }
    }
  }
}
