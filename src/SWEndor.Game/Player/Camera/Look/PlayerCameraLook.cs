using MTV3D65;
using Primrose.Primitives.ValueTypes;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Primitives.Extensions;

namespace SWEndor.Game.Player
{
  public class PlayerCameraLook : ICameraLook
  {
    private TargetPosition LookFrom;
    private TargetPosition LookTo;
    private float RotationMultiplier = 1;

    public PlayerCameraLook() { }

    public TV_3DVECTOR GetPosition(Engine engine) { return LookFrom.GetGlobalPosition(engine); }

    private void InnerUpdate(Engine engine)
    {
      CameraMode cmode = engine.PlayerCameraInfo.CameraMode;
      ActorInfo actor = engine.PlayerInfo.Actor;
      if (actor == null || cmode == CameraMode.CUSTOM)
        return;

      float3 location = float3.One;
      float3 target = float3.One;

      switch (cmode) // should replace
      {
        //case CameraMode.FREEROTATION:
        //case CameraMode.FREEMODE:
        case CameraMode.FIRSTPERSON:
          location = new float3(0, 0, actor.MaxDimensions.z + 10) * actor.Scale;
          target = new float3(0, 0, 20000);
          break;
        case CameraMode.THIRDPERSON:
          location = new float3(0, actor.MaxDimensions.y * 3, -actor.MaxDimensions.z * 8) * actor.Scale;
          target = new float3(0, 0, 20000);
          break;
        case CameraMode.THIRDREAR:
          location = new float3(0, actor.MaxDimensions.y * 3, actor.MaxDimensions.z * 8) * actor.Scale;
          target = new float3(0, 0, -20000);
          break;
      }

      int cammode = (int)cmode;  // (engine.GameScenarioIsCutsceneMode) ? 0 : (int)cmode;

      if (cammode < actor.TypeInfo.CameraData.Cameras.Length)
      {
        location = actor.TypeInfo.CameraData.Cameras[cammode].LookFrom * actor.Scale;
        target = actor.TypeInfo.CameraData.Cameras[cammode].LookAt * actor.Scale;
      }
      LookFrom.TargetActorID = actor.ID;
      LookFrom.PositionRelative = location.ToVec3();
      LookTo.TargetActorID = actor.ID;
      LookTo.PositionRelative = target.ToVec3();
    }

    public void Update(Engine engine, TVCamera cam, float rotz)
    {
      InnerUpdate(engine);
      TV_3DVECTOR tgt = LookTo.GetGlobalPosition(engine);
      TV_3DVECTOR pos = LookFrom.GetGlobalPosition(engine);

      cam.SetPosition(pos.x, pos.y, pos.z);
      cam.SetLookAt(tgt.x, tgt.y, tgt.z);
      cam.RotateZ(rotz * RotationMultiplier);
    }
  }
}
