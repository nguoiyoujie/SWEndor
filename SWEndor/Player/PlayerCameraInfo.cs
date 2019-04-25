using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.Player
{
  public enum CameraMode { FIRSTPERSON, THIRDPERSON, THIRDREAR, FREEROTATION, FREEMODE }

  public class PlayerCameraInfo
  {
    private static PlayerCameraInfo _instance;
    public static PlayerCameraInfo Instance()
    {
      if (_instance == null) { _instance = new PlayerCameraInfo(); }
      return _instance;
    }

    private PlayerCameraInfo()
    {
      Camera.SetCamera(0, 0, 0, 0, 0, 100);
      Camera.SetViewFrustum(90, 65000);
    }

    public TVCamera Camera { get { return Globals.Engine.TrueVision.TVEngine.GetCamera(); } }
    public CameraMode CameraMode = CameraMode.FIRSTPERSON;
    private CameraMode prevCameraMode = CameraMode.FIRSTPERSON;

    public float Shake = 0;
    private float prev_shake_displacement_x = 0;
    private float prev_shake_displacement_y = 0;


    public void Update()
    {
      UpdateMode();

      if (Globals.Engine.PlayerInfo.Actor != null)
        Globals.Engine.PlayerInfo.Actor.TypeInfo.ChaseCamera(Globals.Engine.PlayerInfo.Actor);

      ShakeCam();
      ShakeDecay();
    }

    public void UpdateMode()
    {
      if (prevCameraMode != CameraMode)
      {
        prevCameraMode = CameraMode;
        if (Globals.Engine.PlayerInfo.Actor != null && !Globals.Engine.GameScenarioManager.IsCutsceneMode)
          Globals.Engine.Screen2D.MessageSecondaryText(string.Format("CAMERA: {0}", CameraMode)
                                                        , 2.5f
                                                        , new TV_COLOR(0.5f, 0.5f, 1, 1)
                                                        , 1);
      }
    }

    public void ShakeCam()
    {
      if (Shake > 1 && Globals.Engine.PlayerInfo.StrengthFrac > 0)
      {
        int dispx = Globals.Engine.Random.Next(-(int)Shake, (int)Shake);
        int dispy = Globals.Engine.Random.Next(-(int)Shake, (int)Shake);
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
      if (Globals.Engine.PlayerInfo.IsMovementControlsEnabled)
      {
        float angleX = aY * Settings.SteeringSensitivity;
        float angleY = aX * Settings.SteeringSensitivity;

        if (CameraMode == CameraMode.FREEMODE 
         || CameraMode == CameraMode.FREEROTATION)
        {
          float rate = Globals.Engine.Game.TimeControl.RenderInterval;
          angleX *= 100;
          angleY *= 100;

          Camera.RotateX(angleX * rate); //Globals.Engine.Game.TimeSinceRender / Globals.Engine.Game.TimeControl.SpeedModifier);
          Camera.RotateY(angleY * rate);

          TV_3DVECTOR rot = Camera.GetRotation();
          Utilities.Clamp(ref rot.x, -75, 75);
          Camera.SetRotation(rot.x, rot.y, rot.z);
        }
        else if (Globals.Engine.PlayerInfo.Actor != null && Globals.Engine.PlayerInfo.Actor.CreationState != CreationState.DISPOSED)
        {
          float maxT = Globals.Engine.PlayerInfo.Actor.TypeInfo.MaxTurnRate;
          angleX *= maxT;
          angleY *= maxT;

          if (!Globals.Engine.PlayerInfo.PlayerAIEnabled)
          {
            Utilities.Clamp(ref angleX, -maxT, maxT);
            Utilities.Clamp(ref angleY, -maxT, maxT);

            Globals.Engine.PlayerInfo.Actor.MovementInfo.XTurnAngle = angleX;
            Globals.Engine.PlayerInfo.Actor.MovementInfo.YTurnAngle = angleY;
          }
        }
      }
    }
  }
}
