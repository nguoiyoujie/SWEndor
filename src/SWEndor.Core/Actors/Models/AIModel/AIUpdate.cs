using MTV3D65;
using SWEndor.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Actors.Models
{
  internal static class AIUpdate
  {
    internal static float AdjustRotation(Engine engine, ActorInfo owner, ref TV_3DVECTOR tPos, float responsiveness = 10)
    {
      if (owner.TypeInfo.MoveLimitData.MaxTurnRate == 0)
        return 0;

      if (owner.TypeInfo.AIData.AlwaysAccurateRotation)
      {
        owner.LookAt(tPos);
        return 0;
      }

      TV_3DVECTOR tgtdir = tPos - owner.GetGlobalPosition();
      TV_3DVECTOR chgrot = tgtdir.ConvertDirToRot(engine.TrueVision.TVMathLibrary) - owner.GetGlobalRotation();

      chgrot.x = chgrot.x.Modulus(-180, 180);
      chgrot.y = chgrot.y.Modulus(-180, 180);

      TV_3DVECTOR truechg = new TV_3DVECTOR(chgrot.x, chgrot.y, chgrot.z);
      float maxturn = owner.TypeInfo.MoveLimitData.MaxTurnRate;
      chgrot *= responsiveness;
      chgrot.x = chgrot.x.Clamp(-maxturn, maxturn);
      chgrot.y = chgrot.y.Clamp(-maxturn, maxturn);

      // limit abrupt changes
      float limit = owner.TypeInfo.MoveLimitData.MaxTurnRate * owner.TypeInfo.MoveLimitData.MaxSecondOrderTurnRateFrac;
      owner.MoveData.XTurnAngle = owner.MoveData.XTurnAngle.Creep(chgrot.x, limit);
      owner.MoveData.YTurnAngle = owner.MoveData.YTurnAngle.Creep(chgrot.y, limit);

      TV_3DVECTOR vec = new TV_3DVECTOR();
      TV_3DVECTOR dir = owner.GetGlobalDirection();
      engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, tgtdir);
      return engine.TrueVision.TVMathLibrary.ACos(engine.TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));
    }

    internal static float AdjustSpeed(Engine engine, ActorInfo owner, ref float tSpd)
    {
      if (!owner.MoveData.FreeSpeed)
        tSpd = tSpd.Clamp(owner.MoveData.MinSpeed, owner.MoveData.MaxSpeed);

      float chg = owner.MoveData.MaxSpeedChangeRate * engine.Game.TimeSinceRender;
      owner.MoveData.Speed = owner.MoveData.Speed.Creep(tSpd, chg);

      //if (owner.MoveData.Speed > tSpd)
      //{
      //  owner.MoveData.Speed -= chg;
      //  if (owner.MoveData.Speed < tSpd)
      //    owner.MoveData.Speed = tSpd;
      //}
      //else
      //{
      //  owner.MoveData.Speed += chg;
      //  if (owner.MoveData.Speed > tSpd)
      //    owner.MoveData.Speed = tSpd;
      //}

      return owner.MoveData.Speed - tSpd;
    }
  }
}
