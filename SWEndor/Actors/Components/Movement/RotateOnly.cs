﻿using MTV3D65;
using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public struct RotateOnly : IMoveComponent
  {
    public static readonly RotateOnly Instance = new RotateOnly();

    public void Move(ActorInfo actor, ref MoveData data)
    {
      float time = actor.Game.TimeSinceRender;

      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.GetRotation();
        actor.SetLocalRotation(vec.x, vec.y, 0);
        data.ZRoll -= data.YTurnAngle * data.ZTilt * time;

        // Z rotation decay.
        data.Zdiv += time / 0.005f;
        while (data.Zdiv > 0 && !float.IsInfinity(data.Zdiv))
        {
          data.ZRoll *= 1 - data.ZNormFrac;
          data.Zdiv--;
        }

        float rotX2 = vec.x + data.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.XLimit, actor.TypeInfo.XLimit);
        float rotY2 = vec.y + data.YTurnAngle * time;

        actor.SetLocalRotation(rotX2, rotY2, data.ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = actor.GetRotation();
        data.ZRoll = vec.z;
        data.ZRoll -= data.YTurnAngle * data.ZTilt * time;
        float rotX2 = vec.x + data.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.XLimit, actor.TypeInfo.XLimit);
        float rotY2 = vec.y + data.YTurnAngle * time;
        actor.SetLocalRotation(rotX2, rotY2, data.ZRoll);
      }
    }
  }
}