using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public enum CameraMode { FIRSTPERSON, THIRDPERSON, THIRDREAR }

  public class PlayerInfo
  {
    private static PlayerInfo _instance;
    public static PlayerInfo Instance()
    {
      if (_instance == null) { _instance = new PlayerInfo(); }
      return _instance;
    }

    private PlayerInfo()
    {
      //PlayerCam = Engine.Instance().TVEngine.GetCamera();
      Camera.SetCamera(0, 0, 0, 0, 0, 100);
      Camera.SetViewFrustum(90, 65000);
      Position = new TV_3DVECTOR();
      Direction = new TV_3DVECTOR(0, 0, 1);
    }

    public string Name = "Luke";
    public ActorTypeInfo ActorType = XWingATI.Instance();

    private ActorInfo prevActor;
    private ActorInfo _actor = null;
    public ActorInfo Actor
    {
      get { return _actor; }
      set
      {
        _actor = value;
        if (prevActor != _actor)
        {
          PrimaryWeapon = "";
          SecondaryWeapon = "";
        }
        /*
        if (value != null && value.Camera != null)
        {
          PlayerInfo.Instance().Camera = value.Camera;
        }
        */
      }
    }

    private TVCamera prevCamera;
    public TVCamera Camera
    {
      get { return Engine.Instance().TVEngine.GetCamera(); }
      set
      {
        if (value != null)
          Engine.Instance().TVEngine.SetCamera(value);
      }
    }

    public CameraMode CameraMode = CameraMode.FIRSTPERSON;

    private float m_LowAlarmSoundTime = 0;
    public float StrengthFrac
    {
      get
      {
        if (Actor != null)
        {
          float frac = Actor.Strength / Actor.TypeInfo.MaxStrength;
          if (frac <= 0)
            frac = 0;
          else if (frac < 0.1f)
          {
            if (m_LowAlarmSoundTime < Game.Instance().GameTime)
            {
              SoundManager.Instance().SetSound("shieldlow");
              m_LowAlarmSoundTime = Game.Instance().GameTime + 0.5f;
            }
          }
          return frac;
        }
        else
          return 0;
      }
    }

    public TV_COLOR HealthColor
    {
      get
      {
        double quad = 1.6708;
        float sr = StrengthFrac;
        float r = (float)Math.Cos(sr * quad);
        float g = (float)Math.Sin(sr * quad);
        float b = 0;
        if (r < 0) r = 0;
        if (g < 0) g = 0;
        if (b < 0) b = 0;
        return new TV_COLOR(r, g, b, 1);
      }
    }

    public int Lives = 3;
    public float ScorePerLife = 50000;
    public float ScoreForNextLife = 50000;

    public float enginesmvol = 0;
    public float enginefcvol = 0;
    public float enginelgvol = 0;
    public float enginetevol = 0;
    public float exp_cazavol = 0;
    public float exp_navevol = 0;
    public float exp_restvol = 0;
    public float shake_displacement = 0;
    private float prev_shake_displacement_x = 0;
    private float prev_shake_displacement_y = 0;

    private List<string> PrimaryWeaponModes = new List<string>();
    private List<string> SecondaryWeaponModes = new List<string>();
    public string PrimaryWeapon { get; set; }
    public string SecondaryWeapon { get; set; }
    private int PrimaryWeaponN = 0;
    private int SecondaryWeaponN = 0;

    public void Update()
    {
      TV_3DVECTOR tpos = new TV_3DVECTOR();
      if (Actor == null)
      {

      }
      else
      {
        Position = Actor.GetPosition();
        if ((Camera != null && (prevCamera == null || Camera.GetIndex() != prevCamera.GetIndex()))
          || (Actor != null && (prevActor == null || Actor.ID != prevActor.ID)))
        {
          Camera.SetPosition(Position.x, Position.y, Position.z);
          prevCamera = Camera;
          prevActor = Actor;
        }
      }

      while (Score.Score > ScoreForNextLife)
      {
        Lives++;
        ScoreForNextLife += ScorePerLife;
        SoundManager.Instance().SetSound("Button_4");
      }
      
      //Bounds

      IsOutOfBounds = false;
      if (Actor != null && !(Actor.TypeInfo is InvisibleCameraATI) && !(Actor.TypeInfo is DeathCameraATI))
      {
        TV_3DVECTOR pos = Actor.GetPosition();
        if (pos.x < GameScenarioManager.Instance().MinBounds.x)
        {
          Actor.SetLocalPosition(GameScenarioManager.Instance().MinBounds.x, pos.y, pos.z);
          IsOutOfBounds = true;
        }
        else if (pos.x > GameScenarioManager.Instance().MaxBounds.x)
        {
          Actor.SetLocalPosition(GameScenarioManager.Instance().MaxBounds.x, pos.y, pos.z);
          IsOutOfBounds = true;
        }

        if (pos.y < GameScenarioManager.Instance().MinBounds.y)
        {
          Actor.SetLocalPosition(pos.x, GameScenarioManager.Instance().MinBounds.y, pos.z);
          //IsOutOfBounds = true;
        }
        else if (pos.y > GameScenarioManager.Instance().MaxBounds.y)
        {
          Actor.SetLocalPosition(pos.x, GameScenarioManager.Instance().MaxBounds.y, pos.z);
          //IsOutOfBounds = true;
        }

        if (pos.z < GameScenarioManager.Instance().MinBounds.z)
        {
          Actor.SetLocalPosition(pos.x, pos.y, GameScenarioManager.Instance().MinBounds.z);
          IsOutOfBounds = true;
        }
        else if (pos.z > GameScenarioManager.Instance().MaxBounds.z)
        {
          Actor.SetLocalPosition(pos.x, pos.y, GameScenarioManager.Instance().MaxBounds.z);
          IsOutOfBounds = true;
        }

        if (IsOutOfBounds)
          Screen2D.Instance().UpdateText("You are going out of bounds! Return to the battle!", Game.Instance().GameTime + 5f, new TV_COLOR(1, 1, 1, 1));
      }

      //sounds
      if (!(GameScenarioManager.Instance().Scenario is Scenarios.GSMainMenu))
      {
        if (enginesmvol > 0)
        {
          SoundManager.Instance().SetSound("xwing_engine", false, enginesmvol, true);
          enginesmvol = 0;
        }

        if (enginetevol > 0)
        {
          SoundManager.Instance().SetSound("engine_tie", false, enginetevol, true);
          enginetevol = 0;
        }

        if (enginefcvol > 0)
        {
          SoundManager.Instance().SetSound("falcon_engine", false, enginefcvol, true);
          enginefcvol = 0;
        }

        if (enginelgvol > 0)
        {
          SoundManager.Instance().SetSound("Engine_big", false, enginelgvol, true);
          enginelgvol = 0;
        }

        if (enginesmvol > 0)
        {
          SoundManager.Instance().SetSound("exp_caza", false, exp_cazavol, true);
          exp_cazavol = 0;
        }

        if (exp_navevol > 0)
        {
          if (SoundManager.Instance().SetSound("exp_nave", false, exp_navevol, true))
          {
            shake_displacement = 50 * exp_navevol;
          }
          exp_navevol = 0;
        }

        if (exp_restvol > 0)
        {
          if (SoundManager.Instance().SetSound("exp_resto", false, exp_restvol, true))
          {
            shake_displacement = 5 * exp_restvol;
          }
          exp_restvol = 0;
        }
      }

      // Shake
      if (shake_displacement > 1 && StrengthFrac > 0)
      {
        int dispx = Engine.Instance().Random.Next(-(int)shake_displacement, (int)shake_displacement);
        int dispy = Engine.Instance().Random.Next(-(int)shake_displacement, (int)shake_displacement);
        Camera.MoveRelative(0, dispx - prev_shake_displacement_x, dispy - prev_shake_displacement_y, true);
        prev_shake_displacement_x = dispx;
        prev_shake_displacement_y = dispy;
      }
      shake_displacement *= 0.95f;

      //Weapons
      ParseWeapons();

      //AI Monitor
      if (PlayerAIEnabled)
      {





      }
    }

    public void ChangeSpeed(float frac)
    {
      if (IsMovementControlsEnabled && !PlayerAIEnabled && Actor != null)
      {
        Actor.Speed += frac * Actor.TypeInfo.MaxSpeedChangeRate * Game.Instance().TimeSinceRender;
        if (Actor.Speed < 0)
        {
          Actor.Speed = 0;
        }
      }
    }

    public void RotateCam(float aX, float aY)
    {
      if (IsMovementControlsEnabled && !PlayerAIEnabled)
      {
        angleX = aY * SteeringSensitivity;
        angleY = aX * SteeringSensitivity;

        if (PlayerInfo.Instance().Actor != null && PlayerInfo.Instance().Actor.CreationState != CreationState.DISPOSED)
        {
          angleX *= PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate;
          angleY *= PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate;
        }

        Direction.x = (float)System.Math.Cos(angleY);
        Direction.y = (float)System.Math.Tan(angleX);
        Direction.z = (float)System.Math.Sin(angleY);

        if (PlayerInfo.Instance().Actor != null && PlayerInfo.Instance().Actor.TypeInfo != null)
        {
          if (angleX > PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate)
            angleX = PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate;
          else if (angleX < -PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate)
            angleX = -PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate;

          if (angleY > PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate)
            angleY = PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate;
          else if (angleY < -PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate)
            angleY = -PlayerInfo.Instance().Actor.TypeInfo.MaxTurnRate;

          PlayerInfo.Instance().Actor.XTurnAngle = angleX;
          PlayerInfo.Instance().Actor.YTurnAngle = angleY;
        }
      }
    }

    public void FireWeapon(bool isSecondary)
    {
      if (Actor != null && IsMovementControlsEnabled && !PlayerAIEnabled)
      {
        if (!isSecondary)
        {
          Actor.FireWeapon(AimTarget, PrimaryWeapon);
        }
        else
        {
          Actor.FireWeapon(AimTarget, SecondaryWeapon);
        }
      }
    }

    private void ParseWeapons()
    {
      PrimaryWeaponModes.Clear();
      SecondaryWeaponModes.Clear();

      if (Actor != null) 
      {
        PrimaryWeaponModes.AddRange(Actor.PrimaryWeapons);
        SecondaryWeaponModes.AddRange(Actor.SecondaryWeapons);
        //PrimaryWeaponModes.AddRange(Actor.GetStateS("PrimaryWeaponModes").Split(','));
        //SecondaryWeaponModes.AddRange(Actor.GetStateS("SecondaryWeaponModes").Split(','));

        if (PrimaryWeapon == null || PrimaryWeapon.Length == 0)
          ResetPrimaryWeapon();

        if (SecondaryWeapon == null || SecondaryWeapon.Length == 0)
          ResetSecondaryWeapon();

        IsTorpedoMode = (PrimaryWeapon.Contains("torp") || SecondaryWeapon.Contains("torp"));
      }
      else
      {
        PrimaryWeaponN = 0;
        SecondaryWeaponN = 0;
        PrimaryWeapon = "none";
        SecondaryWeapon = "none";
      }
    }

    public void ResetPrimaryWeapon()
    {
      PrimaryWeaponN = 0;
      PrimaryWeapon = (PrimaryWeaponModes.Count > PrimaryWeaponN) ? PrimaryWeaponModes[PrimaryWeaponN].Trim() : "none";
    }

    public void ResetSecondaryWeapon()
    {
      SecondaryWeaponN = 0;
      SecondaryWeapon = (SecondaryWeaponModes.Count > SecondaryWeaponN) ? SecondaryWeaponModes[SecondaryWeaponN].Trim() : "none";
    }

    public void NextPrimaryWeapon()
    {
      PrimaryWeaponN++;
      if (PrimaryWeaponModes.Count <= PrimaryWeaponN)
        PrimaryWeaponN = 0;
      PrimaryWeapon = (PrimaryWeaponModes.Count > PrimaryWeaponN) ? PrimaryWeaponModes[PrimaryWeaponN].Trim() : "none";
    }

    public void PrevPrimaryWeapon()
    {
      PrimaryWeaponN--;
      if (PrimaryWeaponN < 0)
        PrimaryWeaponN = PrimaryWeaponModes.Count - 1;
      PrimaryWeapon = (PrimaryWeaponModes.Count > PrimaryWeaponN) ? PrimaryWeaponModes[PrimaryWeaponN].Trim() : "none";
    }

    public void NextSecondaryWeapon()
    {
      SecondaryWeaponN++;
      if (SecondaryWeaponModes.Count <= SecondaryWeaponN)
        SecondaryWeaponN = 0;
      SecondaryWeapon = (SecondaryWeaponModes.Count > SecondaryWeaponN) ? SecondaryWeaponModes[SecondaryWeaponN].Trim() : "none";
    }

    public void PrevSecondaryWeapon()
    {
      SecondaryWeaponN--;
      if (SecondaryWeaponN < 0)
        SecondaryWeaponN = SecondaryWeaponModes.Count- 1;
      SecondaryWeapon = (SecondaryWeaponModes.Count > SecondaryWeaponN) ? SecondaryWeaponModes[SecondaryWeaponN].Trim() : "none";
    }


    public void FlashHit(TV_COLOR color)
    {
      if (Actor.TypeInfo is FighterGroup)
      {
        SoundManager.Instance().SetSound("hit");
        Engine.Instance().TVGraphicEffect.Flash(color.r, color.g, color.b, 200);

        double r = Engine.Instance().Random.NextDouble();
        if (Actor.Strength > 0)
        {
          if (r < 0.05f)
          {
            SoundManager.Instance().SetSound("r20", false);
          }
          else if (r < 0.1f && Actor.Strength < 0.6f)
          {
            SoundManager.Instance().SetSound("r21", false);
          }
          else if (r < 0.15f && Actor.Strength < 0.4f)
          {
            SoundManager.Instance().SetSound("r22", false);
          }
          else if (r < 0.2f && Actor.Strength < 0.35f)
          {
            SoundManager.Instance().SetSound("r23", false);
          }
          else if (r < 0.25f && Actor.Strength < 0.3f)
          {
            SoundManager.Instance().SetSound("r24", false);
          }
          else if (r < 0.3f && Actor.Strength < 0.25f)
          {
            SoundManager.Instance().SetSound("r25", false);
          }
        }
      }
    }

    public float angleX;
    public float angleY;
    public float SteeringSensitivity = 1.5f;

    public TV_3DVECTOR Position = new TV_3DVECTOR();
    public TV_3DVECTOR Direction = new TV_3DVECTOR();
    public ScoreInfo Score = new ScoreInfo();

    public bool IsMovementControlsEnabled = true;
    public bool IsTorpedoMode = false;
    public bool ShowUI = true;
    public bool ShowStatus = true;
    public bool ShowRadar = true;
    public bool ShowScore = true;
    public ActorInfo AimTarget = null;

    public bool IsOutOfBounds = false;
    public bool PlayerAIEnabled = false;

  }
}
