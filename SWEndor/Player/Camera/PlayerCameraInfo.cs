using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives.Extensions;

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
      Look = PlayerLook;
    }

    public readonly TVCamera Camera;
    public CameraMode CameraMode = CameraMode.FIRSTPERSON;
    public readonly PlayerCameraLook PlayerLook = new PlayerCameraLook();
    public readonly DeathCameraLook DeathLook = new DeathCameraLook();
    public readonly SceneCameraLook SceneLook = new SceneCameraLook();

    private ICameraLook Look;

    public void SetPlayerLook() { Look = PlayerLook; }
    public void SetDeathLook() { Look = DeathLook; }
    public void SetSceneLook() { Look = SceneLook; }

    private TV_3DVECTOR LookAtPos = new TV_3DVECTOR();
    public TV_3DVECTOR Position { get; private set; }
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

      int cammode = (engine.GameScenarioManager.IsCutsceneMode) ? 0 : (int)CameraMode;

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
      float dist = DistanceModel.GetDistance(Position, origin);
      if (dist <= decayDistance)
        shake = maxValue * (decayDistance - dist) / decayDistance;
    }

    private void ApplyShake()
    {
      if (shake > 1)
      {
        //if (Engine.PlayerInfo.StrengthFrac > 0)
        //{
          int dispx = Engine.Random.Next(-(int)shake, (int)shake);
          int dispy = Engine.Random.Next(-(int)shake, (int)shake);
          Camera.MoveRelative(0, dispx - prev_shake_displacement_x, dispy - prev_shake_displacement_y, true);
          prev_shake_displacement_x = dispx;
          prev_shake_displacement_y = dispy;
        //}
        ShakeDecay();
      }
    }

    private void ShakeDecay()
    {
      shake *= 0.95f; // decay
    }

    internal void RotateCam(float aX, float aY, int aZ)
    {
      PlayerInfo pl = Engine.PlayerInfo;
      if (pl.IsMovementControlsEnabled && !pl.PlayerAIEnabled)
      {
        float angleX = aY * Settings.SteeringSensitivity;
        float angleY = aX * Settings.SteeringSensitivity;
        float angleZ = aZ * Settings.SteeringSensitivity / 20;

        /*
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
          Camera.Rotation = new TV_3DVECTOR(rot.x, rot.y, rot.z);
        }
        else */

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
