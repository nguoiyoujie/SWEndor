using MTV3D65;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Projectiles;
using System;

namespace SWEndor.Game.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior with full range of movement, but reacts erratically when ENGINE or SIDE_THRUSTERS are disabled
  /// </summary>
  internal static class MoveMissile
  {
    public static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time)
    {
      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      float dx = data.XTurnAngle;
      float dy = data.YTurnAngle;
      if (actor.IsStunned)
      {
        dx = ((float)engine.Random.NextDouble() * 2 - 1) * data.MaxTurnRate;
        dy = ((float)engine.Random.NextDouble() * 2 - 1) * data.MaxTurnRate;
        data.ZRoll = ((float)engine.Random.NextDouble() * 2 - 1) * data.MaxTurnRate / 5;
      }

      // move
      actor.MoveRelative(data.Speed * time, 0, 0);

      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.Rotation;
          vec += new TV_3DVECTOR(dx * time, dy * time, (-dy * data.ZTilt + data.ZRoll) * time);
          data.ZRoll = -vec.z;
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
