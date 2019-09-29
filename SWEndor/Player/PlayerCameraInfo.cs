using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
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
      TV_3DVECTOR ret = new TV_3DVECTOR(Position.x, Position.y, Position.z);
      if (TargetActorID > 0)
      {
        ActorInfo tgt = engine.ActorFactory.Get(TargetActorID);
        if (tgt != null && tgt.Active)
        {
          ret = Position + Utilities.GetRelativePositionXYZ(engine, tgt.GetGlobalPosition(), tgt.GetGlobalRotation(), PositionRelative.x, PositionRelative.y, PositionRelative.z);
          _lastPos = ret;//new TV_3DVECTOR(ret.x, ret.y, ret.z);
        }
        return new TV_3DVECTOR(_lastPos.x, _lastPos.y, _lastPos.z);
      }
      return ret;
    }

    public void ApproachPosition(TV_3DVECTOR pos, float distance)
    {
      if (distance == 0 || TargetActorID > 0) // if locked to Actor, skip
        return;

      float dist = ActorDistanceInfo.GetDistance(pos, Position);

      if (dist != 0)
        Position = ActorDistanceInfo.Lerp(Position, pos, (distance / dist).Clamp(-100, 1));
    }
  }

  public struct CameraLook
  {
    [Flags]
    private enum CamMode
    {
      NONE = 0,
      //POSITION_RELATIVE_TO_ACTOR = 0x1,
      //POSITION_RELATIVE_AXIS = 0x2,
      //TARGET_RELATIVE_TO_ACTOR = 0x4,
      //TARGET_RELATIVE_AXIS = 0x8,
      CIRCLE_AROUND_TARGET = 0x10,
    };

    public static CameraLook Default
    {
      get
      {
        CameraLook ret = new CameraLook();
        ret.SetPosition_Point(new TV_3DVECTOR());
        ret.SetTarget_LookAtPoint(new TV_3DVECTOR(0, 0, 20000));
        return ret;
      }
    }

    private CamMode Mode;
    private TargetPosition LookFrom;
    private TargetPosition LookTo;
    private DeathCameraData DeathCamera;
    private float RotationMultiplier;
    private float ApproachSpeed;

    public void ResetPosition()
    {
      LookFrom.Position = default(TV_3DVECTOR);
      LookFrom.PositionRelative = default(TV_3DVECTOR);
      LookFrom.TargetActorID = -1;
    }

    public void SetPosition_Point(TV_3DVECTOR position, float approach_speed = 0)
    {
      LookFrom.Position = position;
      LookFrom.PositionRelative = default(TV_3DVECTOR);
      LookFrom.TargetActorID = -1;
      ApproachSpeed = approach_speed;
    }

    public TV_3DVECTOR GetPosition_Point()
    {
      return LookFrom.Position;
    }

    public int GetPosition_Actor()
    {
      return LookFrom.TargetActorID;
    }

    public TV_3DVECTOR GetTarget_Point()
    {
      return LookTo.Position;
    }

    public int GetTarget_Actor()
    {
      return LookTo.TargetActorID;
    }

    public void SetPosition_Actor(int posActorID, TV_3DVECTOR displacementXYZ = default(TV_3DVECTOR), TV_3DVECTOR displacementRelative = default(TV_3DVECTOR))
    {
      LookFrom.Position = displacementXYZ;
      LookFrom.PositionRelative = displacementRelative;
      LookFrom.TargetActorID = posActorID;
    }

    public void ResetTarget()
    {
      Mode = CamMode.NONE;

      LookTo.Position = default(TV_3DVECTOR);
      LookTo.PositionRelative = default(TV_3DVECTOR);
      LookTo.TargetActorID = -1;
    }

    public void SetTarget_LookAtPoint(TV_3DVECTOR position)
    {
      Mode = CamMode.NONE;

      LookTo.Position = position;
      LookTo.PositionRelative = default(TV_3DVECTOR);
      LookTo.TargetActorID = -1;
    }

    public void SetTarget_LookAtActor(int tgtActorID, TV_3DVECTOR displacementXYZ = default(TV_3DVECTOR), TV_3DVECTOR displacementRelative = default(TV_3DVECTOR))
    {
      Mode = CamMode.NONE;

      LookTo.Position = displacementXYZ;
      LookTo.PositionRelative = displacementRelative;
      LookTo.TargetActorID = tgtActorID;
    }

    public void SetRotationMult(float rotationMult)
    {
      RotationMultiplier = rotationMult;
    }

    public void SetModeDeathCircle(DeathCameraData info)
    {
      Mode = CamMode.CIRCLE_AROUND_TARGET;
      DeathCamera = info;
    }

    public void Update(Engine engine, TVCamera cam, TV_3DVECTOR position, TV_3DVECTOR rotation)
    {
      TV_3DVECTOR tgt = LookTo.GetGlobalPosition(engine);
      LookFrom.ApproachPosition(tgt, ApproachSpeed * engine.Game.TimeSinceRender);
      TV_3DVECTOR pos = LookFrom.GetGlobalPosition(engine);

      if ((Mode & CamMode.CIRCLE_AROUND_TARGET) == CamMode.CIRCLE_AROUND_TARGET)
      {
        float angle = (engine.Game.GameTime % DeathCamera.Period) * (2 * Globals.PI / DeathCamera.Period);
        cam.SetPosition(pos.x + DeathCamera.Radius * (float)Math.Cos(angle)
                      , pos.y + DeathCamera.Height
                      , pos.z + DeathCamera.Radius * (float)Math.Sin(angle));

        cam.SetLookAt(pos.x, pos.y, pos.z);
      }
      else
      {
        cam.SetPosition(pos.x, pos.y, pos.z);
        cam.SetLookAt(tgt.x, tgt.y, tgt.z);

        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, rotation.z * RotationMultiplier);
      }
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
    }

    public readonly TVCamera Camera;
    public CameraMode CameraMode = CameraMode.FIRSTPERSON;
    public CameraLook Look = CameraLook.Default;
    public int LookActor { get; private set; } = -1;
    private int LookAtActor = -1;
    private TV_3DVECTOR LookAtPos = new TV_3DVECTOR();
    public TV_3DVECTOR Position { get; private set; }
    public TV_3DVECTOR Rotation { get; private set; }

    private float shake = 0;
    private float prev_shake_displacement_x = 0;
    private float prev_shake_displacement_y = 0;


    public void Update()
    {
      ActorInfo actor = Engine.PlayerInfo.Actor;
      if (Engine.PlayerInfo.Actor == null)
        actor = Engine.ActorFactory.Get(Look.GetPosition_Actor());

      if (actor != null && actor.Active)
      {
        UpdateFromActor(Engine, actor);
        Position = actor.GetGlobalPosition();
        Rotation = actor.GetGlobalRotation();
      }

      Look.Update(Engine, Camera, Position, Rotation);
      ApplyShake();
    }

    float spdf = 0; // avoid expensive calculations
    float fov = 60; // avoid expensive calls
    const float farplane = 650000; // const
    public void UpdateViewFrustum(ActorInfo actor)
    {
      float sf = actor.MoveData.Speed / actor.MoveData.MaxSpeed;
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

    public void UpdateFromActor(Engine engine, ActorInfo actor)
    {
      if (CameraMode == CameraMode.CUSTOM)
        return;

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
        else if (LookAtActor >= 0) // cutscene to actor view
        {
          Look.SetPosition_Actor(actor.ID, displacementRelative: location);
          Look.SetTarget_LookAtActor(LookAtActor);
          Look.SetRotationMult(0.5f);
        }
        else // same as active view
        {
          Look.SetPosition_Actor(actor.ID, displacementRelative: location);
          Look.SetTarget_LookAtActor(actor.ID, displacementRelative: target);
        }
      }

      UpdateViewFrustum(actor);
    }

    public void Shake(float value)
    {
      shake = value;
    }

    public void ProximityShake(float maxValue, float decayDistance, TV_3DVECTOR origin)
    {
      float dist = ActorDistanceInfo.GetDistance(Position, origin);
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

    public void ShakeDecay()
    {
      shake *= 0.95f; // decay
    }

    public void RotateCam(float aX, float aY)
    {
      PlayerInfo pl = Engine.PlayerInfo;
      if (pl.IsMovementControlsEnabled)
      {
        float angleX = aY * Settings.SteeringSensitivity;
        float angleY = aX * Settings.SteeringSensitivity;

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

          if (!Engine.PlayerInfo.PlayerAIEnabled)
          {
            angleX = angleX.Clamp(-maxT, maxT);
            angleY = angleY.Clamp(-maxT, maxT);

            a.MoveData.XTurnAngle = angleX;
            a.MoveData.YTurnAngle = angleY;
          }
        }
      }
    }
  }
}
