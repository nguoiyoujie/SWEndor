using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.AI.Actions;
using SWEndor.Core;
using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior with full range of movement
  /// </summary>
  public static class MoveNormal
  {
    public static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
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
      if (actor.TypeInfo.SystemData.AllowSystemDamage && actor.GetStatus(SystemPart.SIDE_THRUSTERS) != SystemState.ACTIVE)
      {
        dx /= 5;
        dy /= 5;
        data.ZRoll /= 5;
      }

      // Engine destroyed
      if (actor.TypeInfo.SystemData.AllowSystemDamage && actor.GetStatus(SystemPart.ENGINE) != SystemState.ACTIVE)
      {
        // speed decay
        data.Speed -= (data.MaxSpeed - data.MinSpeed) / 0.5f * time;
      }

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
          vec = Utilities.RotateXYZ(vec, dx * time, dy * time, data.ZRoll * time);
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
        vec = Utilities.RotateXY(vec, dx * time, dy * time);
        actor.Rotation = vec;
      }
    }
  }
}
