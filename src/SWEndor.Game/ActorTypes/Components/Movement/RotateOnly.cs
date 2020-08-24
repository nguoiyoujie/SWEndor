using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.Core;
using System;

namespace SWEndor.Game.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that rotates only
  /// </summary>
  internal static class RotateOnly
  {
    internal static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
    {
      float dx = data.XTurnAngle;
      float dy = data.YTurnAngle;

      // Steering destroyed
      if (actor.TypeInfo.SystemData.AllowSystemDamage && actor.GetStatus(SystemPart.SIDE_THRUSTERS) != SystemState.ACTIVE)
      {
        dx /= 5;
        dy /= 5;
        data.ZRoll /= 5;
      }

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
