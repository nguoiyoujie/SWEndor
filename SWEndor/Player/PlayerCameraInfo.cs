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
      Camera.SetCamera(0, 0, 0, 0, 0, 100);
      Camera.SetViewFrustum(90, 65000);
    }

    public TVCamera Camera { get { return Engine.TrueVision.TVEngine.GetCamera(); } }
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

      ShakeCam();
      ShakeDecay();
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

    public void ShakeCam()
    {
      if (Shake > 1 && Engine.PlayerInfo.StrengthFrac > 0)
      {
        int dispx = Engine.Random.Next(-(int)Shake, (int)Shake);
        int dispy = Engine.Random.Next(-(int)Shake, (int)Shake);
        Camera.MoveRelative(0, dispx - prev_shake_displacement_x, dispy - prev_shake_displacement_y, true);
        prev_shake_displacement_x = dispx;
        prev_shake_displacement_y = dispy;
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
          Utilities.Clamp(ref rot.x, -75, 75);
          Camera.SetRotation(rot.x, rot.y, rot.z);
        }
        else if (Engine.PlayerInfo.Actor != null && Engine.PlayerInfo.Actor.CreationState != CreationState.DISPOSED)
        {
          float maxT = Engine.PlayerInfo.Actor.TypeInfo.MaxTurnRate;
          angleX *= maxT;
          angleY *= maxT;

          if (!Engine.PlayerInfo.PlayerAIEnabled)
          {
            Utilities.Clamp(ref angleX, -maxT, maxT);
            Utilities.Clamp(ref angleY, -maxT, maxT);

            Engine.PlayerInfo.Actor.MovementInfo.XTurnAngle = angleX;
            Engine.PlayerInfo.Actor.MovementInfo.YTurnAngle = angleY;
          }
        }
      }
    }
  }
}
