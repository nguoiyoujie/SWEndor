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
  /// Implementation of Move behavior with full range of movement
  /// </summary>
  internal static class MoveNormal
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

      // Steering destroyed
      if (!actor.IsSystemOperational(SystemPart.SIDE_THRUSTERS))
      {
        dx /= 5;
        dy /= 5;
        data.ZRoll /= 5;
      }

      // Engine destroyed
      if (!actor.IsSystemOperational(SystemPart.ENGINE))
      {
        // speed decay
        data.Speed -= (data.MaxSpeed - data.MinSpeed) * time;
        data.Speed = data.Speed.Clamp(0, data.MaxSpeed);
      }
      else
      {
        // Control speed
        if (!data.FreeSpeed)
          data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);
      }

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
