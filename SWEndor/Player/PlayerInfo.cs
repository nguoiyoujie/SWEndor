using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Primitives;
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
    public bool Exists { get { return Engine.ActorFactory.Contains(ActorID); } }
    public int TempActorID;
    public ActorInfo TempActor { get { return Engine.ActorFactory.Get(TempActorID); } }
    
    private float m_LowAlarmSoundTime = 0;
    public float StrengthFrac { get { return Actor?.HP_Frac ?? 0; } }
    public TV_COLOR StrengthColor { get { return Actor?.HP_Color ?? new TV_COLOR(1, 1, 1, 1); } }
    public TV_COLOR FactionColor { get { return Actor?.Faction?.Color ?? new TV_COLOR(1, 1, 1, 1); } }

    public int Lives = 3;
    public float ScorePerLife = 50000;
    public float ScoreForNextLife = 50000;

    // should set as configurable?
    public string StrengthLowSound = "shieldlow";
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
      UpdateStats();
      UpdateBounds();
    }

    private void UpdatePosition()
    {
      if (Actor != null)
        Position = Actor.GetGlobalPosition();
    }

    private void UpdateStats()
    {
      // score
      while (Score.Score > ScoreForNextLife)
      {
        Lives++;
        ScoreForNextLife += ScorePerLife;
        Engine.SoundManager.SetSound("button_4");
      }

      // this should be moved elsewhere
      if (StrengthFrac > 0 && StrengthFrac < 0.1f)
      {
        if (m_LowAlarmSoundTime < Engine.Game.GameTime)
        {
          Engine.SoundManager.SetSound("shieldlow");
          m_LowAlarmSoundTime = Engine.Game.GameTime + 0.5f;
        }
      }
    }

    private void UpdateBounds()
    {
      bool announceOutOfBounds = false;
      if (Actor != null)
      {
        TV_3DVECTOR pos = Actor.GetGlobalPosition();
        if (pos.x < Engine.GameScenarioManager.MinBounds.x)
        {
          Actor.Position = new TV_3DVECTOR(Engine.GameScenarioManager.MinBounds.x, pos.y, pos.z);
          announceOutOfBounds = true;
        }
        else if (pos.x > Engine.GameScenarioManager.MaxBounds.x)
        {
          Actor.Position = new TV_3DVECTOR(Engine.GameScenarioManager.MaxBounds.x, pos.y, pos.z);
          announceOutOfBounds = true;
        }

        if (pos.y < Engine.GameScenarioManager.MinBounds.y)
          Actor.Position = new TV_3DVECTOR(pos.x, Engine.GameScenarioManager.MinBounds.y, pos.z);
        else if (pos.y > Engine.GameScenarioManager.MaxBounds.y)
          Actor.Position = new TV_3DVECTOR(pos.x, Engine.GameScenarioManager.MaxBounds.y, pos.z);


        if (pos.z < Engine.GameScenarioManager.MinBounds.z)
        {
          Actor.Position = new TV_3DVECTOR(pos.x, pos.y, Engine.GameScenarioManager.MinBounds.z);
          announceOutOfBounds = true;
        }
        else if (pos.z > Engine.GameScenarioManager.MaxBounds.z)
        {
          Actor.Position = new TV_3DVECTOR(pos.x, pos.y, Engine.GameScenarioManager.MaxBounds.z);
          announceOutOfBounds = true;
        }

        if (announceOutOfBounds)
          Engine.Screen2D.MessageText("You are going out of bounds! Return to the battle!", 5, new TV_COLOR(1, 1, 1, 1), 99);
      }
    }

    public void ChangeSpeed(float frac)
    {
      if (frac == 0)
        return;
      if (IsMovementControlsEnabled && !PlayerAIEnabled && Actor != null)
      {
        Actor.MoveData.Speed += frac * Actor.TypeInfo.MaxSpeedChangeRate * Engine.Game.TimeSinceRender;
        Actor.MoveData.Speed = Actor.MoveData.Speed.Clamp(Actor.MoveData.MinSpeed, Actor.MoveData.MaxSpeed);
      }
    }
    /*
    public void FirePrimaryWeapon()
    {
      if (Actor != null && IsMovementControlsEnabled && !PlayerAIEnabled)
        ActorInfo.FireWeapon(Engine, ActorID, AimTargetID, PrimaryWeapon);
    }

    public void FireSecondaryWeapon()
    {
      if (Actor != null && IsMovementControlsEnabled && !PlayerAIEnabled)
        ActorInfo.FireWeapon(Engine, ActorID, AimTargetID, SecondaryWeapon);
    }
    */
    public void FirePrimaryWeapon()
    {
      FireWeapon(PrimaryWeapon);
    }

    public void FireSecondaryWeapon()
    {
      FireWeapon(SecondaryWeapon);
    }

    private void FireWeapon(string weapon)
    {
      if (IsMovementControlsEnabled && !PlayerAIEnabled)
      {
        ActorInfo.FireWeapon(Engine, Actor, AimTarget, weapon);
      }
      //using (var v = Engine.ActorFactory.Get(ActorID))
      //using (var vtgt = Engine.ActorFactory.Get(AimTargetID))
      //  v?.Value.FireWeapon(vtgt?.Value, weapon);
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

    public void SquadronAssist()
    {
      if (Actor == null || Actor.Squad.IsNull)
        return;

      if (Actor.Squad.Leader != Actor)
      {
        Engine.Screen2D.MessageSecondaryText("You are not the squad leader. You cannot command your squad to perform attacks!", 5, FactionColor);
        return;
      }

      if (AssistTarget != null)
      {
        Engine.Screen2D.MessageSecondaryText("Directing squad to assist {0}.".F(AssistTarget.TopParent.Name), 5, FactionColor);
        Actor.Squad.Mission = new AI.Squads.Missions.AssistActor(AssistTarget.TopParent);
      }

      if (AimTarget != null)
      {
        Engine.Screen2D.MessageSecondaryText("Directing squad to attack {0}.".F(AimTarget.Name), 5, FactionColor);
        Actor.Squad.Mission = new AI.Squads.Missions.AttackActor(AimTarget, 99999);
      }
    }

    public void SquadronFree()
    {
      if (Actor == null || Actor.Squad.IsNull)
        return;

      if (Actor.Squad.Leader != Actor)
      {
        Engine.Screen2D.MessageSecondaryText("You are not the squad leader. You cannot command your squad to perform attacks!", 5, FactionColor);
        return;
      }

      Engine.Screen2D.MessageSecondaryText("Discarding squad orders.", 5, FactionColor);
      Actor.Squad.Mission = null;
    }

    public void FlashHit(TV_COLOR color)
    {
      if (Actor.TypeInfo is ActorTypes.Groups.Fighter)
      {
        Engine.SoundManager.SetSound("hit");
        Engine.TrueVision.TVGraphicEffect.Flash(color.r, color.g, color.b, 200);

        if (Actor.HP > 0 && DamagedReportSound != null && DamagedReportSound.Length > 0)
        {
          double r = Engine.Random.NextDouble();
          int dmgnum = DamagedReportSound.Length;

          int dmgst = (int)((dmgnum + 1) * Actor.HP_Frac);
          if (dmgst < DamagedReportSound.Length)
            if (r < 0.25f * (dmgnum - dmgst) / dmgnum)
              Engine.SoundManager.SetSound(DamagedReportSound[dmgst], false);
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
    public ActorInfo AimTarget { get { return Engine.ActorFactory.Get(AimTargetID); } }

    public int AssistTargetID = -1;
    public ActorInfo AssistTarget { get { return Engine.ActorFactory.Get(AssistTargetID); } }

    public bool PlayerAIEnabled = false;
  }
}
