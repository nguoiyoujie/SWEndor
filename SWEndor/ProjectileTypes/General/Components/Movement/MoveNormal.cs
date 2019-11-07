using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Projectiles;
using System;

namespace SWEndor.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior with full range of movement
  /// </summary>
  public static class MoveNormal
  {
    public static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time)
    {
      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      // move
      actor.MoveRelative(data.Speed * time, 0, 0);

      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.Rotation;
          vec += new TV_3DVECTOR(data.XTurnAngle * time, data.YTurnAngle * time, (-data.YTurnAngle * data.ZTilt + data.ZRoll) * time);
          data.ZRoll = -vec.z;
        data.ZRoll *= (float)(Math.Pow(1 - data.ZNormFrac, time / 0.01f));
        actor.Rotation = vec;
      }
      else
      {
        TV_3DVECTOR vec = actor.Rotation;
        vec = Utilities.RotateXY(vec, data.XTurnAngle * time, data.YTurnAngle * time);
        actor.Rotation = vec;
      }
    }
  }
}
