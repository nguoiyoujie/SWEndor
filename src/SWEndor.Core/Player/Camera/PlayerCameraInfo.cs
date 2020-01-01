using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives.Extensions;

namespace SWEndor.Player
{
  public class PlayerCameraInfo
  {
    public readonly Engine Engine;
    internal PlayerCameraInfo(Engine engine)
    {
      Engine = engine;
      Camera = Engine.TrueVision.TVEngine.GetCamera();
      Camera.SetCamera(0, 0, 0, 0, 0, 100);
      Camera.SetViewFrustum(60, 650000);
      _look = PlayerLook;
    }

    internal readonly TVCamera Camera;
    public CameraMode CameraMode = CameraMode.FIRSTPERSON;
    internal readonly FreeLook FreeLook = new FreeLook();
    internal readonly PlayerCameraLook PlayerLook = new PlayerCameraLook();
    internal readonly DeathCameraLook DeathLook = new DeathCameraLook();
    internal readonly SceneCameraLook SceneLook = new SceneCameraLook();
    internal bool IsFreeLook = false;

    internal CameraMode[] ScenarioModes = new CameraMode[] { CameraMode.FIRSTPERSON };
    internal int ScenarioModeNum = 0;

    private ICameraLook _look;
    internal ICameraLook Look { get { return IsFreeLook ? FreeLook : _look; } }

    /// <summary>
    /// Sets the view camera to use the player actor view
    /// </summary>
    public void SetPlayerLook() { _look = PlayerLook; IsFreeLook = false; }

    /// <summary>
    /// Sets the view camera to use the player death view
    /// </summary>
    public void SetDeathLook() { _look = DeathLook; IsFreeLook = false; }

    /// <summary>
    /// Sets the view camera to use the scene (no actor) view
    /// </summary>
    public void SetSceneLook() { _look = SceneLook; IsFreeLook = false; }

    /// <summary>
    /// Toggles the use of the free look mode
    /// </summary>
    public void SetSceneLook(bool enable)
    {
      IsFreeLook = enable;
    }

    private TV_3DVECTOR LookAtPos = new TV_3DVECTOR();

    /// <summary>
    /// The view camera position
    /// </summary>
    public TV_3DVECTOR Position { get; private set; }

    /// <summary>
    /// The view camera rotation
    /// </summary>
    public TV_3DVECTOR Rotation { get; private set; }

    private float shake = 0;
    private float prev_shake_displacement_x = 0;
    private float prev_shake_displacement_y = 0;

    private float spdf = 0; // avoid expensive calculations
    private float fov = 60; // avoid expensive calls
    private const float farplane = 650000; // const
    private void UpdateViewFrustum(ActorInfo actor)
    {
      float sf = 1;
      if (actor != null && actor.MoveData.MaxSpeed - actor.MoveData.MinSpeed > 0)
        sf = (actor.MoveData.Speed - actor.MoveData.MinSpeed) / (actor.MoveData.MaxSpeed - actor.MoveData.MinSpeed);

      if (!(sf - spdf < 0.001f && sf - spdf > 0.001f))
      {
        spdf = sf;
        float f = (Engine.TrueVision.TVMathLibrary.ATan(sf) / 45);
        f--;
        f /= 2;
        if (f > 0)
          f /= 2;
        f++;
        f *= 60;
        f = f.Clamp(45, 175);
        if (!(f - fov < 0.001f && f - fov > 0.001f))
        {
          fov = f;
          Camera.SetViewFrustum(fov, farplane);
        }
      }
    }

    internal void Update()
    {
      Update(Engine);
      ApplyShake();
    }

    private void Update(Engine engine)
    {
      if (IsFreeLook)
        return;

      ActorInfo actor = Engine.PlayerInfo.Actor; //?? Engine.ActorFactory.Get(Look.GetPosition_Actor());
      if (actor != null && actor.Active)
      {
        //Look = PlayerLook;
        Position = actor.GetGlobalPosition();
        Rotation = actor.GetGlobalRotation();
        Look.Update(Engine, Camera, Rotation.z);
        UpdateViewFrustum(actor);
      }
      else if (Engine.PlayerInfo.ActorID == -1)
      {
        //Look = SceneLook;
        Position = Look.GetPosition(engine);
        Rotation = default(TV_3DVECTOR);
        Look.Update(Engine, Camera, 0);
        UpdateViewFrustum(null);
      }
      else
      {
        Look.Update(Engine, Camera, Rotation.z);
        UpdateViewFrustum(null);
      }
    }

    private void UpdateFromActor(Engine engine, ActorInfo actor)
    {
      //if (CameraMode == CameraMode.CUSTOM)
      //  return;

      /*
      TV_3DVECTOR location = new TV_3DVECTOR();
      TV_3DVECTOR target = new TV_3DVECTOR();

      switch (CameraMode) // should replace
      {
        case CameraMode.FREEROTATION:
        case CameraMode.FREEMODE:
        case CameraMode.FIRSTPERSON:
          location = new TV_3DVECTOR(0, 0, actor.MaxDimensions.z + 10);
          target = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDPERSON:
          location = new TV_3DVECTOR(0, actor.MaxDimensions.y * 3, -actor.MaxDimensions.z * 8);
          target = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDREAR:
          location = new TV_3DVECTOR(0, actor.MaxDimensions.y * 3, actor.MaxDimensions.z * 8);
          target = new TV_3DVECTOR(0, 0, -20000);
          break;
      }

      int cammode = (engine.GameScenarioIsCutsceneMode) ? 0 : (int)CameraMode;

      if (cammode < actor.TypeInfo.Cameras.Length)
      {
        location = actor.TypeInfo.Cameras[cammode].LookFrom;
        target = actor.TypeInfo.Cameras[cammode].LookAt;
      }

      if (!actor.IsDyingOrDead)
      {
        if (actor.IsPlayer) // active view
        {
          Look.SetPosition_Actor(actor.ID, displacementRelative: location);
          Look.SetTarget_LookAtActor(actor.ID, displacementRelative: target);
          Look.SetRotationMult(1);
        }
        else // same as active view
        {
          Look.SetPosition_Actor(actor.ID, displacementRelative: location);
          Look.SetTarget_LookAtActor(actor.ID, displacementRelative: target);
        }
      }
      */

      //UpdateViewFrustum(actor);
    }

    public void Shake(float value)
    {
      shake = value;
    }

    public void ProximityShake(float maxValue, float decayDistance, TV_3DVECTOR origin)
    {
      float dist = DistanceModel.GetDistance(Engine.TrueVision.TVMathLibrary, Position, origin);
      if (dist <= decayDistance)
        shake = maxValue * (decayDistance - dist) / decayDistance;
    }

    private void ApplyShake()
    {
      if (shake > 1)
      {
        int dispx = Engine.Random.Next(-(int)shake, (int)shake);
        int dispy = Engine.Random.Next(-(int)shake, (int)shake);
        Camera.MoveRelative(0, dispx - prev_shake_displacement_x, dispy - prev_shake_displacement_y, true);
        prev_shake_displacement_x = dispx;
        prev_shake_displacement_y = dispy;
        ShakeDecay();
      }
    }

    private void ShakeDecay()
    {
      shake *= 0.95f; // decay
    }

    public CameraMode NextCameraMode()
    {
      if (ScenarioModes == null || ScenarioModes.Length == 0)
        return CameraMode.FIRSTPERSON;

      ScenarioModeNum++;
      if (ScenarioModeNum >= ScenarioModes.Length)
        ScenarioModeNum = 0;

      return ScenarioModes[ScenarioModeNum];
    }

    public CameraMode PrevCameraMode()
    {
      if (ScenarioModes == null || ScenarioModes.Length == 0)
        return CameraMode.FIRSTPERSON;

      ScenarioModeNum--;
      if (ScenarioModeNum < 0)
        ScenarioModeNum = ScenarioModes.Length - 1;

      return ScenarioModes[ScenarioModeNum];
    }

    internal void RotateCam(float aX, float aY, int aZ)
    {
      PlayerInfo pl = Engine.PlayerInfo;
      float angleX = aY * Settings.SteeringSensitivity;
      float angleY = aX * Settings.SteeringSensitivity;
      float angleZ = aZ * Settings.SteeringSensitivity / 20;

      if (IsFreeLook)
      {
        float rate = Engine.Game.TimeControl.UpdateInterval;
        angleX *= 100 * rate;
        angleY *= 100 * rate;
        angleZ *= rate;

        Camera.RotateX(angleX);
        Camera.RotateY(angleY);
        Camera.RotateZ(angleZ);
        return; // don't control the player craft when in free look
      }

      if (pl.IsMovementControlsEnabled && !pl.PlayerAIEnabled)
      {
        ActorInfo a = pl.Actor;
        if (a != null && a.Active)
        {
          float maxT = a.TypeInfo.MoveLimitData.MaxTurnRate;

          angleX *= maxT;
          angleY *= maxT;
          angleZ *= maxT;

          angleX = angleX.Clamp(-maxT, maxT);
          angleY = angleY.Clamp(-maxT, maxT);
          angleZ = angleZ.Clamp(-maxT / 20, maxT / 20);

          a.MoveData.XTurnAngle = angleX;
          a.MoveData.YTurnAngle = angleY;
          a.MoveData.ZRoll += angleZ;
        }
      }
    }
  }
}
