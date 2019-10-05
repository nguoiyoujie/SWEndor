using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Core;
using System;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that rotates only
  /// </summary>
  public static class RotateOnly
  {
    public static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
    {
      // Control rotation
      if (data.ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.Rotation;
        if (actor.IsPlayer)
        {
          vec = Utilities.RotateXYZ(vec, data.XTurnAngle * time, data.YTurnAngle * time, data.ZRoll * time);
        }
        else
        {
          vec += new TV_3DVECTOR(data.XTurnAngle * time, data.YTurnAngle * time, (-data.YTurnAngle * data.ZTilt + data.ZRoll) * time);
          data.ZRoll = -vec.z;
        }
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
