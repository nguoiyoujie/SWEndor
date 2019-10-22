using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives.Extensions;
using System;

namespace SWEndor.Player
{
  public enum CameraMode
  {
    CUSTOM = -1,
    FIRSTPERSON = 0,
    THIRDPERSON = 1,
    THIRDREAR = 2,
    FREEROTATION = 3,
    FREEMODE = 4
  }

  public struct TargetPosition
  {
    public TV_3DVECTOR Position;
    public TV_3DVECTOR PositionRelative;
    public int TargetActorID;

    private TV_3DVECTOR _lastPos;
    public TV_3DVECTOR GetGlobalPosition(Engine engine)
    {
      if (TargetActorID >= 0)
      {
        ActorInfo tgt = engine.ActorFactory.Get(TargetActorID);
        if (tgt != null && tgt.Active)
          _lastPos = Position + Utilities.GetRelativePositionXYZ(engine, tgt.GetGlobalPosition(), tgt.GetGlobalRotation(), PositionRelative.x, PositionRelative.y, PositionRelative.z);

        return new TV_3DVECTOR(_lastPos.x, _lastPos.y, _lastPos.z);
      }
      else
      {
        return Position;
      }
    }

    public void ApproachPosition(Engine engine, TV_3DVECTOR pos, float distance)
    {
      if (distance == 0 || TargetActorID >= 0) // if locked to Actor, skip
        return;

      float dist = DistanceModel.GetDistance(pos, Position);

      if (dist != 0)
        Position = DistanceModel.Lerp(engine, Position, pos, (distance / dist).Clamp(-100, 1));
    }
  }

  public interface ICameraLook
  {
    TV_3DVECTOR GetPosition(Engine engine);
    void Update(Engine engine, TVCamera cam, float rotz);
  }

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

      int cammode = (int)cmode;  // (engine.GameScenarioManager.IsCutsceneMode) ? 0 : (int)cmode;

      if (cammode < actor.TypeInfo.Cameras.Length)
      {
        location = actor.TypeInfo.Cameras[cammode].LookFrom;
        target = actor.TypeInfo.Cameras[cammode].LookAt;
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

  public class DeathCameraLook : ICameraLook
  {
    private TargetPosition LookFrom;
    private DeathCameraData DeathCamera;

    public DeathCameraLook() { }

    public TV_3DVECTOR GetPosition(Engine engine) { return LookFrom.GetGlobalPosition(engine); }

    public void SetPosition_Actor(int actorID, DeathCameraData data)
    {
      LookFrom.Position = new TV_3DVECTOR();
      LookFrom.PositionRelative = new TV_3DVECTOR();
      LookFrom.TargetActorID = actorID;
      DeathCamera = data;
    }

    public void SetPosition_Point(TV_3DVECTOR position, DeathCameraData data)
    {
      LookFrom.Position = position;
      LookFrom.PositionRelative = new TV_3DVECTOR();
      LookFrom.TargetActorID = -1;
      DeathCamera = data;
    }

    public void Update(Engine engine, TVCamera cam, float rotz)
    {
      TV_3DVECTOR pos = LookFrom.GetGlobalPosition(engine);

      float angle = (engine.Game.GameTime % DeathCamera.Period) * (2 * Globals.PI / DeathCamera.Period);
      cam.SetPosition(pos.x + DeathCamera.Radius * (float)Math.Cos(angle)
                    , pos.y + DeathCamera.Height
                    , pos.z + DeathCamera.Radius * (float)Math.Sin(angle));

      cam.SetLookAt(pos.x, pos.y, pos.z);
    }
  }

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
      LookFrom.ApproachPosition(engine, tgt, ApproachSpeed * engine.Game.TimeSinceRender);
      TV_3DVECTOR pos = LookFrom.GetGlobalPosition(engine);

      cam.SetPosition(pos.x, pos.y, pos.z);
      cam.SetLookAt(tgt.x, tgt.y, tgt.z);
      cam.RotateZ(rotz * RotationMultiplier);
    }
  }

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
        if (Engine.PlayerInfo.StrengthFrac > 0)
        {
          int dispx = Engine.Random.Next(-(int)shake, (int)shake);
          int dispy = Engine.Random.Next(-(int)shake, (int)shake);
          Camera.MoveRelative(0, dispx - prev_shake_displacement_x, dispy - prev_shake_displacement_y, true);
          prev_shake_displacement_x = dispx;
          prev_shake_displacement_y = dispy;
        }
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
