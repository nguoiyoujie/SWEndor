using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Player
{
  public class SceneCameraLook : ICameraLook
  {
    private TargetPosition LookFrom;
    private TargetPosition LookTo;
    private float RotationMultiplier = 0;
    private float ApproachSpeed = 0;

    public SceneCameraLook() { }

    public TV_3DVECTOR GetPosition(Engine engine) { return LookFrom.GetGlobalPosition(engine); }

    public void SetPosition_Point(TV_3DVECTOR position, float approach_speed = 0)
    {
      LookFrom.Position = position;
      LookFrom.PositionRelative = default(TV_3DVECTOR);
      LookFrom.TargetActorID = -1;
      ApproachSpeed = approach_speed;
    }

    public void SetPosition_Actor(int posActorID, TV_3DVECTOR displacementXYZ = default(TV_3DVECTOR), TV_3DVECTOR displacementRelative = default(TV_3DVECTOR))
    {
      LookFrom.Position = displacementXYZ;
      LookFrom.PositionRelative = displacementRelative;
      LookFrom.TargetActorID = posActorID;
    }

    public void SetRotationMult(float rotationMult)
    {
      RotationMultiplier = rotationMult;
    }

    public void ResetTarget()
    {
      LookTo.Position = default(TV_3DVECTOR);
      LookTo.PositionRelative = default(TV_3DVECTOR);
      LookTo.TargetActorID = -1;
    }

    public void SetTarget_LookAtPoint(TV_3DVECTOR position)
    {
      LookTo.Position = position;
      LookTo.PositionRelative = default(TV_3DVECTOR);
      LookTo.TargetActorID = -1;
    }

    public void SetTarget_LookAtActor(int tgtActorID, TV_3DVECTOR displacementXYZ = default(TV_3DVECTOR), TV_3DVECTOR displacementRelative = default(TV_3DVECTOR))
    {
      LookTo.Position = displacementXYZ;
      LookTo.PositionRelative = displacementRelative;
      LookTo.TargetActorID = tgtActorID;
    }

    public void Update(Engine engine, TVCamera cam, float rotz)
    {
      TV_3DVECTOR tgt = LookTo.GetGlobalPosition(engine);
      LookFrom.ApproachPosition(engine.TrueVision.TVMathLibrary, tgt, ApproachSpeed * engine.Game.TimeSinceRender);
      TV_3DVECTOR pos = LookFrom.GetGlobalPosition(engine);

      cam.SetPosition(pos.x, pos.y, pos.z);
      cam.SetLookAt(tgt.x, tgt.y, tgt.z);
      cam.RotateZ(rotz * RotationMultiplier);
    }
  }
}
