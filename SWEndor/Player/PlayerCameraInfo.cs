using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System;

namespace SWEndor.Player
{
  public enum CameraMode
  {
    CUSTOM = -1,
    FIRSTPERSON = 0,
    THIRDPERSON, THIRDREAR, FREEROTATION, FREEMODE
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
          ret = Position + Utilities.GetRelativePositionXYZ(engine, tgt.GetPosition(), tgt.GetRotation(), PositionRelative.x, PositionRelative.y, PositionRelative.z);
          _lastPos = ret;//new TV_3DVECTOR(ret.x, ret.y, ret.z);
        }
        return new TV_3DVECTOR(_lastPos.x, _lastPos.y, _lastPos.z);
      }
      return ret;
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
    private DeathCameraInfo DeathCamera;
    private float RotationMultiplier;

    public void ResetPosition()
    {
      LookFrom.Position = default(TV_3DVECTOR);
      LookFrom.PositionRelative = default(TV_3DVECTOR);
      LookFrom.TargetActorID = -1;
    }

    public void SetPosition_Point(TV_3DVECTOR position)
    {
      LookFrom.Position = position;
      LookFrom.PositionRelative = default(TV_3DVECTOR);
      LookFrom.TargetActorID = -1;
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

    public void SetModeDeathCircle(DeathCameraInfo info)
    {
      Mode = CamMode.CIRCLE_AROUND_TARGET;
      DeathCamera = info;
    }

    public void Update(Engine engine, TVCamera cam, TV_3DVECTOR position, TV_3DVECTOR rotation)
    {
      TV_3DVECTOR pos = LookFrom.GetGlobalPosition(engine);
      TV_3DVECTOR tgt = LookTo.GetGlobalPosition(engine);

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
    public CameraMode prevCameraMode = CameraMode.FIRSTPERSON;
    public CameraLook Look = CameraLook.Default;
    public int LookActor { get; private set; } = -1;
    private int LookAtActor = -1;
    private TV_3DVECTOR LookAtPos = new TV_3DVECTOR();
    public TV_3DVECTOR Position { get; private set; }
    public TV_3DVECTOR Rotation { get; private set; }

    public float Shake = 0;
    private float prev_shake_displacement_x = 0;
    private float prev_shake_displacement_y = 0;


    public void Update()
    {
      UpdateMode();

      ActorInfo actor = Engine.PlayerInfo.Actor;
      if (Engine.PlayerInfo.Actor == null)
        actor = Engine.ActorFactory.Get(Look.GetPosition_Actor());

      if (actor != null && actor.Active)
      {
        UpdateFromActor(Engine, actor);
        Position = actor.GetPosition();
        Rotation = actor.GetRotation();
      }

      Look.Update(Engine, Camera, Position, Rotation);
      ApplyShake();
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
          location = new TV_3DVECTOR(0, 0, actor.TypeInfo.max_dimensions.z + 10);
          target = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDPERSON:
          location = new TV_3DVECTOR(0, actor.TypeInfo.max_dimensions.y * 5, actor.TypeInfo.min_dimensions.z * 8);
          target = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDREAR:
          location = new TV_3DVECTOR(0, actor.TypeInfo.max_dimensions.y * 3, actor.TypeInfo.max_dimensions.z * 8);
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
          Camera.SetRotation(rot.x, rot.y, rot.z);
        }
        else */

        ActorInfo a = pl.Actor;
        if (a != null && a.Active)
        {
          float maxT = a.TypeInfo.MaxTurnRate;
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
