using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using SWEndor.Scenarios;
using SWEndor.Sound;
using System;
using System.Collections.Generic;

namespace SWEndor
{
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
      Name = "Luke";
      Score = new ScoreInfo("(Player)");
    }

    public string Name;
    public ActorTypeInfo ActorType;
    public bool RequestSpawn;

    private ActorInfo prevActor;
    private ActorInfo _actor = null;
    public ActorInfo Actor
    {
      get { return _actor; }
      set
      {
        if (_actor != value)
        {
          if (_actor != null)
            _actor.Score = new ScoreInfo(_actor.Key);
          _actor = value;
          if (_actor != null)
            _actor.Score = Score;
        }
        if (prevActor != _actor)
        {
          ParseWeapons();
          ResetPrimaryWeapon();
          ResetSecondaryWeapon();
          prevActor = Actor;
        }
      }
    }

    public ActorInfo TempActor;

    private float m_LowAlarmSoundTime = 0;
    public float StrengthFrac
    {
      get
      {
        if (Actor != null)
        {
          float frac = Actor.CombatInfo.Strength / Actor.TypeInfo.MaxStrength;
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

    // should set as configurable?
    public string[] DamagedReportSound = new string[] { "r25", "r24", "r23", "r22", "r21", "r20" };

    // weapons
    private List<string> PrimaryWeaponModes = new List<string>();
    private List<string> SecondaryWeaponModes = new List<string>();
    public string PrimaryWeapon { get; set; }
    public string SecondaryWeapon { get; set; }
    private int PrimaryWeaponN = 0;
    private int SecondaryWeaponN = 0;

    public void Update()
    {
      UpdatePosition();
      UpdateScoreToLives();
      UpdateBounds();
    }

    public void UpdatePosition()
    {
      if (Actor != null)
        Position = Actor.GetPosition();
    }

    public void UpdateScoreToLives()
    {
      while (Score.Score > ScoreForNextLife)
      {
        Lives++;
        ScoreForNextLife += ScorePerLife;
        SoundManager.Instance().SetSound("button_4");
      }
    }

    public void UpdateBounds()
    {
      bool announceOutOfBounds = false;
      if (Actor != null && !(Actor.TypeInfo is InvisibleCameraATI) && !(Actor.TypeInfo is DeathCameraATI))
      {
        TV_3DVECTOR pos = Actor.GetPosition();
        if (pos.x < GameScenarioManager.Instance().MinBounds.x)
        {
          Actor.SetLocalPosition(GameScenarioManager.Instance().MinBounds.x, pos.y, pos.z);
          announceOutOfBounds = true;
        }
        else if (pos.x > GameScenarioManager.Instance().MaxBounds.x)
        {
          Actor.SetLocalPosition(GameScenarioManager.Instance().MaxBounds.x, pos.y, pos.z);
          announceOutOfBounds = true;
        }

        if (pos.y < GameScenarioManager.Instance().MinBounds.y)
          Actor.SetLocalPosition(pos.x, GameScenarioManager.Instance().MinBounds.y, pos.z);
        else if (pos.y > GameScenarioManager.Instance().MaxBounds.y)
          Actor.SetLocalPosition(pos.x, GameScenarioManager.Instance().MaxBounds.y, pos.z);


        if (pos.z < GameScenarioManager.Instance().MinBounds.z)
        {
          Actor.SetLocalPosition(pos.x, pos.y, GameScenarioManager.Instance().MinBounds.z);
          announceOutOfBounds = true;
        }
        else if (pos.z > GameScenarioManager.Instance().MaxBounds.z)
        {
          Actor.SetLocalPosition(pos.x, pos.y, GameScenarioManager.Instance().MaxBounds.z);
          announceOutOfBounds = true;
        }

        if (announceOutOfBounds)
          Screen2D.Instance().MessageText("You are going out of bounds! Return to the battle!", 5, new TV_COLOR(1, 1, 1, 1), 99);
      }
    }

    public void ChangeSpeed(float frac)
    {
      if (frac == 0)
        return;
      if (IsMovementControlsEnabled && !PlayerAIEnabled && Actor != null)
      {
        Actor.MovementInfo.Speed += frac * Actor.TypeInfo.MaxSpeedChangeRate * Game.Instance().TimeSinceRender;
        Utilities.Clamp(ref Actor.MovementInfo.Speed, Actor.MovementInfo.MinSpeed, Actor.MovementInfo.MaxSpeed);
      }
    }

    public void FirePrimaryWeapon()
    {
      if (Actor != null && IsMovementControlsEnabled && !PlayerAIEnabled)
        Actor.FireWeapon(AimTarget, PrimaryWeapon);
    }

    public void FireSecondaryWeapon()
    {
      if (Actor != null && IsMovementControlsEnabled && !PlayerAIEnabled)
        Actor.FireWeapon(AimTarget, SecondaryWeapon);
    }

    private void ParseWeapons()
    {
      PrimaryWeaponModes.Clear();
      SecondaryWeaponModes.Clear();

      if (Actor != null) 
      {
        PrimaryWeaponModes.AddRange(Actor.PrimaryWeapons);
        SecondaryWeaponModes.AddRange(Actor.SecondaryWeapons);

        if (PrimaryWeapon == null || PrimaryWeapon.Length == 0)
          ResetPrimaryWeapon();

        if (SecondaryWeapon == null || SecondaryWeapon.Length == 0)
          ResetSecondaryWeapon();
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

        if (Actor.CombatInfo.Strength > 0 && DamagedReportSound != null && DamagedReportSound.Length > 0)
        {
          double r = Engine.Instance().Random.NextDouble();
          int dmgnum = DamagedReportSound.Length;

          int dmgst = (int)(Actor.CombatInfo.Strength * (dmgnum + 1) / Actor.CombatInfo.MaxStrength);
          if (dmgst < DamagedReportSound.Length)
            if (r < 0.25f * (dmgnum - dmgst) / dmgnum)
              SoundManager.Instance().SetSound(DamagedReportSound[dmgst], false);
        }
      }
    }

    public TV_3DVECTOR Position = new TV_3DVECTOR();
    public ScoreInfo Score;

    public bool IsMovementControlsEnabled = true;
    public bool IsTorpedoMode
    {
      get
      {
        return (PrimaryWeapon != null && PrimaryWeapon.Contains("torp")) 
          || (SecondaryWeapon != null && SecondaryWeapon.Contains("torp"));
      }
    }
    public ActorInfo AimTarget = null;

    public bool PlayerAIEnabled = false;
  }
}
