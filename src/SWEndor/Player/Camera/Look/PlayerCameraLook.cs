using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.Player
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

      TV_3DVECTOR location = new TV_3DVECTOR();
      TV_3DVECTOR target = new TV_3DVECTOR();

      switch (cmode) // should replace
      {
        case CameraMode.FREEROTATION:
        case CameraMode.FREEMODE:
        case CameraMode.FIRSTPERSON:
          location = new TV_3DVECTOR(0, 0, actor.MaxDimensions.z + 10) * actor.Scale;
          target = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDPERSON:
          location = new TV_3DVECTOR(0, actor.MaxDimensions.y * 3, -actor.MaxDimensions.z * 8) * actor.Scale;
          target = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDREAR:
          location = new TV_3DVECTOR(0, actor.MaxDimensions.y * 3, actor.MaxDimensions.z * 8) * actor.Scale;
          target = new TV_3DVECTOR(0, 0, -20000);
          break;
      }

      int cammode = (int)cmode;  // (engine.GameScenarioManager.IsCutsceneMode) ? 0 : (int)cmode;

      if (cammode < actor.TypeInfo.Cameras.Length)
      {
        location = actor.TypeInfo.Cameras[cammode].LookFrom * actor.Scale;
        target = actor.TypeInfo.Cameras[cammode].LookAt * actor.Scale;
      }
      LookFrom.TargetActorID = actor.ID;
      LookFrom.PositionRelative = location;
      LookTo.TargetActorID = actor.ID;
      LookTo.PositionRelative = target;
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
