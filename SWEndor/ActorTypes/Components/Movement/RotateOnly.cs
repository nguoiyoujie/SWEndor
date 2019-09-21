﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that rotates only
  /// </summary>
  public static class RotateOnly
  {
    public static void Move(ActorInfo actor, ref MoveData data, float time)
    {
      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.Rotation;
        actor.Rotation = new TV_3DVECTOR(vec.x, vec.y, 0);
        data.ZRoll -= data.YTurnAngle * data.ZTilt * time;

        // Z rotation decay.
        data.Zdiv += time / 0.005f;
        while (data.Zdiv > 0 && !float.IsInfinity(data.Zdiv))
        {
          data.ZRoll *= 1 - data.ZNormFrac;
          data.Zdiv--;
        }

        float rotX2 = vec.x + data.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.MoveLimitData.XLimit, actor.TypeInfo.MoveLimitData.XLimit);
        float rotY2 = vec.y + data.YTurnAngle * time;

        actor.Rotation = new TV_3DVECTOR(rotX2, rotY2, data.ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = actor.Rotation;
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
