using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;
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

      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.Rotation;
        actor.Rotation = new TV_3DVECTOR(vec.x, vec.y, 0);
        actor.MoveRelative(data.Speed * time, 0, 0);
        data.ZRoll -= data.YTurnAngle * data.ZTilt * time;

        // Z rotation decay.
        data.ZRoll *= (float)(Math.Pow( 1 - data.ZNormFrac, time / 0.002f));

        float rotX2 = vec.x + data.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.MoveLimitData.XLimit, actor.TypeInfo.MoveLimitData.XLimit);
        float rotY2 = vec.y + data.YTurnAngle * time;

        actor.Rotation = new TV_3DVECTOR(rotX2, rotY2, data.ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = actor.Rotation;
        actor.MoveRelative(data.Speed * time, 0, 0);
        data.ZRoll = vec.z;
        data.ZRoll -= data.YTurnAngle * data.ZTilt * time;
        float rotX2 = vec.x + data.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.MoveLimitData.XLimit, actor.TypeInfo.MoveLimitData.XLimit);
        float rotY2 = vec.y + data.YTurnAngle * time;
        actor.Rotation = new TV_3DVECTOR(rotX2, rotY2, data.ZRoll);
      }
    }
  }
}
