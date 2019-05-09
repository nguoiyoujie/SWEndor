using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.Player
{
  public enum CameraMode { FIRSTPERSON, THIRDPERSON, THIRDREAR, FREEROTATION, FREEMODE }

  public class PlayerCameraInfo
  {
    public readonly Engine Engine;
    internal PlayerCameraInfo(Engine engine)
    {
      Engine = engine;
      Camera = Engine.TrueVision.TVEngine.GetCamera();
      Camera.SetCamera(0, 0, 0, 0, 0, 100);
      Camera.SetViewFrustum(90, 650000);
    }

    public readonly TVCamera Camera;
    public CameraMode CameraMode = CameraMode.FIRSTPERSON;
    private CameraMode prevCameraMode = CameraMode.FIRSTPERSON;

    public float Shake = 0;
    private float prev_shake_displacement_x = 0;
    private float prev_shake_displacement_y = 0;


    public void Update()
    {
      UpdateMode();

      if (Engine.PlayerInfo.Actor != null)
        Engine.PlayerInfo.Actor.TypeInfo.ChaseCamera(Engine.PlayerInfo.Actor);

      ApplyShake();
    }

    public void UpdateMode()
    {
      if (prevCameraMode != CameraMode)
      {
        prevCameraMode = CameraMode;
        if (Engine.PlayerInfo.Actor != null && !Engine.GameScenarioManager.IsCutsceneMode)
          Engine.Screen2D.MessageSecondaryText(string.Format("CAMERA: {0}", CameraMode)
                                                        , 2.5f
                                                        , new TV_COLOR(0.5f, 0.5f, 1, 1)
                                                        , 1);
      }
    }

    public void ApplyShake()
    {
      if (Shake > 1)
      {
        if (Engine.PlayerInfo.StrengthFrac > 0)
        {
          int dispx = Engine.Random.Next(-(int)Shake, (int)Shake);
          int dispy = Engine.Random.Next(-(int)Shake, (int)Shake);
          Camera.MoveRelative(0, dispx - prev_shake_displacement_x, dispy - prev_shake_displacement_y, true);
          prev_shake_displacement_x = dispx;
          prev_shake_displacement_y = dispy;
        }
        ShakeDecay();
      }
    }

    public void ShakeDecay()
    {
      Shake *= 0.95f; // decay
    }

    public void RotateCam(float aX, float aY)
    {
      if (Engine.PlayerInfo.IsMovementControlsEnabled)
      {
        float angleX = aY * Settings.SteeringSensitivity;
        float angleY = aX * Settings.SteeringSensitivity;

        if (CameraMode == CameraMode.FREEMODE 
         || CameraMode == CameraMode.FREEROTATION)
        {
          float rate = Engine.Game.TimeControl.RenderInterval;
          angleX *= 100;
          angleY *= 100;

          Camera.RotateX(angleX * rate); //Engine.Game.TimeSinceRender / Engine.Game.TimeControl.SpeedModifier);
          Camera.RotateY(angleY * rate);

          TV_3DVECTOR rot = Camera.GetRotation();
          rot.x = rot.x.Clamp(-75, 75);
          Camera.SetRotation(rot.x, rot.y, rot.z);
        }
        else if (Engine.PlayerInfo.Actor != null && Engine.PlayerInfo.Actor.CreationState != CreationState.DISPOSED)
        {
          float maxT = Engine.PlayerInfo.Actor.TypeInfo.MaxTurnRate;
          angleX *= maxT;
          angleY *= maxT;

          if (!Engine.PlayerInfo.PlayerAIEnabled)
          {
            angleX = angleX.Clamp(-maxT, maxT);
            angleY = angleY.Clamp(-maxT, maxT);

            Engine.PlayerInfo.Actor.MoveData.XTurnAngle = angleX;
            Engine.PlayerInfo.Actor.MoveData.YTurnAngle = angleY;
          }
        }
      }
    }
  }
}
