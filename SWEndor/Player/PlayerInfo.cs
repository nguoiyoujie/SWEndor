using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using System;
using System.Collections.Generic;

namespace SWEndor.Player
{
  public class PlayerInfo
  {
    public readonly Engine Engine;
    internal PlayerInfo(Engine engine)
    {
      Engine = engine;
      Name = "Luke";
      Score = ScoreInfo.Player;
    }

    public string Name;
    public ActorTypeInfo ActorType;
    public bool RequestSpawn;

    private int _actorID = -1;
    public int ActorID
    {
      get { return _actorID; }
      set
      {
        if (_actorID != value)
        {
          _actorID = value;
          ParseWeapons();
          ResetPrimaryWeapon();
          ResetSecondaryWeapon();
        }
      }
    }
    public ActorInfo Actor { get { return Engine.ActorFactory.Get(ActorID); } }
    public int TempActorID;
    public ActorInfo TempActor { get { return Engine.ActorFactory.Get(TempActorID); } }
    
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
            if (m_LowAlarmSoundTime < Globals.Engine.Game.GameTime)
            {
              Globals.Engine.SoundManager.SetSound("shieldlow");
              m_LowAlarmSoundTime = Globals.Engine.Game.GameTime + 0.5f;
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
    public string[] DamagedReportSound = new string[] { }; // "r25", "r24", "r23", "r22", "r21", "r20" };

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
        Globals.Engine.SoundManager.SetSound("button_4");
      }
    }

    public void UpdateBounds()
    {
      bool announceOutOfBounds = false;
      if (Actor != null && !(Actor.TypeInfo is InvisibleCameraATI) && !(Actor.TypeInfo is DeathCameraATI))
      {
        TV_3DVECTOR pos = Actor.GetPosition();
        if (pos.x < Engine.GameScenarioManager.MinBounds.x)
        {
          Actor.SetLocalPosition(Engine.GameScenarioManager.MinBounds.x, pos.y, pos.z);
          announceOutOfBounds = true;
        }
        else if (pos.x > Engine.GameScenarioManager.MaxBounds.x)
        {
          Actor.SetLocalPosition(Engine.GameScenarioManager.MaxBounds.x, pos.y, pos.z);
          announceOutOfBounds = true;
        }

        if (pos.y < Engine.GameScenarioManager.MinBounds.y)
          Actor.SetLocalPosition(pos.x, Engine.GameScenarioManager.MinBounds.y, pos.z);
        else if (pos.y > Engine.GameScenarioManager.MaxBounds.y)
          Actor.SetLocalPosition(pos.x, Engine.GameScenarioManager.MaxBounds.y, pos.z);


        if (pos.z < Engine.GameScenarioManager.MinBounds.z)
        {
          Actor.SetLocalPosition(pos.x, pos.y, Engine.GameScenarioManager.MinBounds.z);
          announceOutOfBounds = true;
        }
        else if (pos.z > Engine.GameScenarioManager.MaxBounds.z)
        {
          Actor.SetLocalPosition(pos.x, pos.y, Engine.GameScenarioManager.MaxBounds.z);
          announceOutOfBounds = true;
        }

        if (announceOutOfBounds)
          Globals.Engine.Screen2D.MessageText("You are going out of bounds! Return to the battle!", 5, new TV_COLOR(1, 1, 1, 1), 99);
      }
    }

    public void ChangeSpeed(float frac)
    {
      if (frac == 0)
        return;
      if (IsMovementControlsEnabled && !PlayerAIEnabled && Actor != null)
      {
        Actor.MovementInfo.Speed += frac * Actor.TypeInfo.MaxSpeedChangeRate * Globals.Engine.Game.TimeSinceRender;
        Utilities.Clamp(ref Actor.MovementInfo.Speed, Actor.MovementInfo.MinSpeed, Actor.MovementInfo.MaxSpeed);
      }
    }

    public void FirePrimaryWeapon()
    {
      if (Actor != null && IsMovementControlsEnabled && !PlayerAIEnabled)
        Actor.FireWeapon(AimTargetID, PrimaryWeapon);
    }

    public void FireSecondaryWeapon()
    {
      if (Actor != null && IsMovementControlsEnabled && !PlayerAIEnabled)
        Actor.FireWeapon(AimTargetID, SecondaryWeapon);
    }

    private void ParseWeapons()
    {
      PrimaryWeaponModes.Clear();
      SecondaryWeaponModes.Clear();

      if (Actor != null) 
      {
        PrimaryWeaponModes.AddRange(Actor.WeaponSystemInfo.PrimaryWeapons);
        SecondaryWeaponModes.AddRange(Actor.WeaponSystemInfo.SecondaryWeapons);

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
      if (Actor.TypeInfo is ActorTypes.Groups.Fighter)
      {
        Globals.Engine.SoundManager.SetSound("hit");
        Globals.Engine.TrueVision.TVGraphicEffect.Flash(color.r, color.g, color.b, 200);

        if (Actor.CombatInfo.Strength > 0 && DamagedReportSound != null && DamagedReportSound.Length > 0)
        {
          double r = Globals.Engine.Random.NextDouble();
          int dmgnum = DamagedReportSound.Length;

          int dmgst = (int)(Actor.CombatInfo.Strength * (dmgnum + 1) / Actor.CombatInfo.MaxStrength);
          if (dmgst < DamagedReportSound.Length)
            if (r < 0.25f * (dmgnum - dmgst) / dmgnum)
              Globals.Engine.SoundManager.SetSound(DamagedReportSound[dmgst], false);
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
    public int AimTargetID = -1;

    public bool PlayerAIEnabled = false;
  }
}
