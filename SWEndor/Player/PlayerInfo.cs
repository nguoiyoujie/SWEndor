using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
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
    public ActorInfo Actor { get { return Engine.ActorFactory.Get(ActorID)?.Value ?? null; } }
    //public ActorInfo TempActor { get { return Engine.ActorFactory.Get(TempActorID)?.Value ?? null; } }

    public bool Exists { get { return Engine.ActorFactory.Contains(ActorID); } }
    public int TempActorID;

    private float m_LowAlarmSoundTime = 0;
    public float StrengthFrac
    {
      get
      {
        using (var v = Engine.ActorFactory.Get(ActorID))
          return v?.Value.Health.Frac ?? 0;
      }
    }
    public TV_COLOR StrengthColor
    {
      get
      {
        using (var v = Engine.ActorFactory.Get(ActorID))
          return v?.Value.Health.Color ?? new TV_COLOR(1, 1, 1, 1);
      }
    }
    public TV_COLOR FactionColor
    {
      get
      {
        using (var v = Engine.ActorFactory.Get(ActorID))
          return v?.Value?.Faction.Color ?? new TV_COLOR(1, 1, 1, 1);
      }
    }

    public int Lives = 3;
    public float ScorePerLife = 50000;
    public float ScoreForNextLife = 50000;

    // should set as configurable?
    public string StrengthLowSound = "shieldlow";
    public string[] DamagedReportSound = new string[] { }; // "r25", "r24", "r23", "r22", "r21", "r20" };

    // weapons
    private List<string> PrimaryWeaponModes = new List<string>();
    private List<string> SecondaryWeaponModes = new List<string>();
    public string PrimaryWeapon { get { return (PrimaryWeaponModes.Count > PrimaryWeaponN) ? PrimaryWeaponModes[PrimaryWeaponN].Trim() : null; } }
    public string SecondaryWeapon { get { return (SecondaryWeaponModes.Count > SecondaryWeaponN) ? SecondaryWeaponModes[SecondaryWeaponN].Trim() : null; } }
    private int PrimaryWeaponN = 0;
    private int SecondaryWeaponN = 0;

    private int PrimaryWeaponNs = 0;
    private int SecondaryWeaponNs = 0;

    public void Update()
    {
      using (var v = Engine.ActorFactory.Get(ActorID))
        if (v != null)
        {
          UpdatePosition(v.Value);
          SnapToBounds(v.Value);
        }

      UpdateStats();
    }

    private void UpdatePosition(ActorInfo a)
    {
      Position = a.GetGlobalPosition();
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

    private void SnapToBounds(ActorInfo a) // change to use ActorInfo as a parameter
    {
      bool announceOutOfBounds = false;

      TV_3DVECTOR pos = a.GetGlobalPosition();
      if (pos.x < Engine.GameScenarioManager.MinBounds.x)
      {
        a.Transform.Position = new TV_3DVECTOR(Engine.GameScenarioManager.MinBounds.x, pos.y, pos.z);
        announceOutOfBounds = true;
      }
      else if (pos.x > Engine.GameScenarioManager.MaxBounds.x)
      {
        a.Transform.Position = new TV_3DVECTOR(Engine.GameScenarioManager.MaxBounds.x, pos.y, pos.z);
        announceOutOfBounds = true;
      }

      if (pos.y < Engine.GameScenarioManager.MinBounds.y)
        a.Transform.Position = new TV_3DVECTOR(pos.x, Engine.GameScenarioManager.MinBounds.y, pos.z);
      else if (pos.y > Engine.GameScenarioManager.MaxBounds.y)
        a.Transform.Position = new TV_3DVECTOR(pos.x, Engine.GameScenarioManager.MaxBounds.y, pos.z);

      if (pos.z < Engine.GameScenarioManager.MinBounds.z)
      {
        a.Transform.Position = new TV_3DVECTOR(pos.x, pos.y, Engine.GameScenarioManager.MinBounds.z);
        announceOutOfBounds = true;
      }
      else if (pos.z > Engine.GameScenarioManager.MaxBounds.z)
      {
        a.Transform.Position = new TV_3DVECTOR(pos.x, pos.y, Engine.GameScenarioManager.MaxBounds.z);
        announceOutOfBounds = true;
      }

      if (announceOutOfBounds)
        Engine.Screen2D.MessageText(TextLocalization.Get(TextLocalKeys.PLAYER_OUTOFBOUNDS), 5, new TV_COLOR(1, 1, 1, 1), 99);
    }

    public void ChangeSpeed(float frac)
    {
      if (frac == 0)
        return;

      if (IsMovementControlsEnabled && !PlayerAIEnabled)
        using (var v = Engine.ActorFactory.Get(ActorID))
          if (v != null)
          {
            ActorInfo a = v.Value;
            a.MoveData.Speed += frac * a.TypeInfo.MaxSpeedChangeRate * Engine.Game.TimeSinceRender;
            a.MoveData.Speed = a.MoveData.Speed.Clamp(a.MoveData.MinSpeed, a.MoveData.MaxSpeed);
          }
    }

    public void FirePrimaryWeapon()
    {
      FireWeapon(PrimaryWeapon);
    }

    public void FireSecondaryWeapon()
    {
      FireWeapon(SecondaryWeapon);
    }

    public void FireWeapon(string weapon)
    {
      if (IsMovementControlsEnabled && !PlayerAIEnabled)
        using (var v = Engine.ActorFactory.Get(ActorID))
        using (var vtgt = Engine.ActorFactory.Get(AimTargetID))
          v?.Value.FireWeapon(vtgt?.Value, weapon);
    }

    private void ParseWeapons()
    {
      PrimaryWeaponModes.Clear();
      SecondaryWeaponModes.Clear();

      using (var v = Engine.ActorFactory.Get(ActorID))
        if (v != null)
        {
          ActorInfo a = v.Value;
          PrimaryWeaponModes.AddRange(a.WeaponSystemInfo.PrimaryWeapons);
          SecondaryWeaponModes.AddRange(a.WeaponSystemInfo.SecondaryWeapons);

          if (PrimaryWeapon == null)
            ResetPrimaryWeapon();

          if (SecondaryWeapon == null)
            ResetSecondaryWeapon();
        }
        else
        {
          PrimaryWeaponN = 0;
          SecondaryWeaponN = 0;
        }
    }

    public void ResetPrimaryWeapon()
    {
      PrimaryWeaponN = 0;
    }

    public void ResetSecondaryWeapon()
    {
      SecondaryWeaponN = 0;
    }

    public void SaveWeapons()
    {
      PrimaryWeaponNs = PrimaryWeaponN;
      SecondaryWeaponNs = SecondaryWeaponN;
    }

    public void LoadWeapons()
    {
      PrimaryWeaponN = PrimaryWeaponNs;
      SecondaryWeaponN = SecondaryWeaponNs;
    }

    public void NextPrimaryWeapon()
    {
      PrimaryWeaponN++;
      if (PrimaryWeaponModes.Count <= PrimaryWeaponN)
        PrimaryWeaponN = 0;
    }

    public void PrevPrimaryWeapon()
    {
      PrimaryWeaponN--;
      if (PrimaryWeaponN < 0)
        PrimaryWeaponN = PrimaryWeaponModes.Count - 1;
    }

    public void NextSecondaryWeapon()
    {
      SecondaryWeaponN++;
      if (SecondaryWeaponModes.Count <= SecondaryWeaponN)
        SecondaryWeaponN = 0;
    }

    public void PrevSecondaryWeapon()
    {
      SecondaryWeaponN--;
      if (SecondaryWeaponN < 0)
        SecondaryWeaponN = SecondaryWeaponModes.Count - 1;
    }


    public void FlashHit()
    {
      using (ScopedManager<ActorInfo>.ScopedItem v = Engine.ActorFactory.Get(ActorID))
      {
        if (v != null)
        {
          ActorInfo a = v.Value;
          if (a.TypeInfo is ActorTypes.Groups.Fighter)
          {
            Engine.SoundManager.SetSound("hit");
            TV_COLOR color = StrengthColor;
            Engine.TrueVision.TVGraphicEffect.Flash(color.r, color.g, color.b, 200);

            if (a.Health.HP > 0 && DamagedReportSound != null && DamagedReportSound.Length > 0)
            {
              double r = Engine.Random.NextDouble();
              int dmgnum = DamagedReportSound.Length;

              int dmgst = (int)((dmgnum + 1) * a.Health.Frac);
              if (dmgst < DamagedReportSound.Length)
                if (r < 0.25f * (dmgnum - dmgst) / dmgnum)
                  Engine.SoundManager.SetSound(DamagedReportSound[dmgst], false);
            }
          }
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
