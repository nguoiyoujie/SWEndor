using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.AI;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Sound;
using SWEndor.Game.Weapons;
using SWEndor.Game.Actors.Components;

namespace SWEndor.Game.Player
{
  public class PlayerInfo
  {
    // Globals
    public readonly Engine Engine;

    public string Name;

    // Spawn
    public ActorTypeInfo ActorType;
    public bool RequestSpawn;

    // Actor
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
    public float Strength { get { return Actor?.HP ?? 0; } }
    public float StrengthFrac { get { return Actor?.HP_Frac ?? 0; } }
    public COLOR StrengthColor { get { return Actor?.HP_Color ?? ColorLocalization.Get(ColorLocalKeys.WHITE); } }
    public COLOR FactionColor { get { return Actor?.Faction?.Color ?? ColorLocalization.Get(ColorLocalKeys.WHITE); } }

    // Scenario
    public int Lives = 3;
    public int ScorePerLife = 50000;
    public int ScoreForNextLife = 50000;

    public string[] DamagedReportSound = SoundGlobals.DmgSounds;

    // Score
    internal ScoreInfo Score;

    // Movement
    public bool PlayerLockMovement = false;
    public bool SystemLockMovement = false;
    public bool IsMovementControlsEnabled { get { return !SystemLockMovement && !PlayerLockMovement; } }
    public bool IsTorpedoMode
    {
      get
      {
        return (PrimaryWeapon.Weapon.Proj.WeaponType == WeaponType.TORPEDO)
          || (SecondaryWeapon.Weapon.Proj.WeaponType == WeaponType.TORPEDO);
      }
    }

    // Targeting
    public int TargetActorID = -1;
    public ActorInfo TargetActor { get { return Engine.ActorFactory.Get(TargetActorID); } }
    public bool LockTarget = false;

    // AI
    public bool PlayerAIEnabled = false;

    // Weapons
    public WeaponShotInfo PrimaryWeapon
    {
      get
      {
        if (Actor == null)
          return WeaponShotInfo.Default;

        WeaponShotInfo[] w = Actor.WeaponDefinitions.PrimaryWeapons;
        if (PrimaryWeaponN >= w.Length)
          return WeaponShotInfo.Default;

        return w[PrimaryWeaponN];
      }
    }
    public WeaponShotInfo SecondaryWeapon
    {
      get
      {
        if (Actor == null)
          return WeaponShotInfo.Default;

        WeaponShotInfo[] w = Actor.WeaponDefinitions.SecondaryWeapons;
        if (SecondaryWeaponN >= w.Length)
          return WeaponShotInfo.Default;

        return w[SecondaryWeaponN];
      }
    }
    public int PrimaryWeaponN = 0;
    public int SecondaryWeaponN = 0;


    internal PlayerInfo(Engine engine)
    {
      Engine = engine;
      Name = "Luke";
      Score = ScoreInfo.Player;
    }

    public void Update()
    {
      UpdateStats();
      ClampBounds();
      ScanCargo();
    }


    private void UpdateStats()
    {
      // score
      while (Score.Score > ScoreForNextLife)
      {
        Lives++;
        ScoreForNextLife += ScorePerLife;
        Engine.SoundManager.SetSound(SoundGlobals.Button4);
      }

      // TO-DO: this should be moved elsewhere
      if (StrengthFrac > 0 && (Strength <= 1 || StrengthFrac < 0.1f))
      {
        if (m_LowAlarmSoundTime < Engine.Game.GameTime)
        {
          Engine.SoundManager.SetSound(SoundGlobals.LowHP);
          m_LowAlarmSoundTime = Engine.Game.GameTime + 0.5f;
        }
      }
      else
      {
        m_LowAlarmSoundTime = 0;
      }
    }

    private void ClampBounds()
    {
      bool announceOutOfBounds = false;
      ActorInfo player = Actor;
      if (player != null)
      {
        TV_3DVECTOR pos = player.Position;
        TV_3DVECTOR orig = pos;
        pos.x = pos.x.Clamp(Engine.GameScenarioManager.Scenario.State.MinBounds.x, Engine.GameScenarioManager.Scenario.State.MaxBounds.x);
        pos.y = pos.y.Clamp(Engine.GameScenarioManager.Scenario.State.MinBounds.y, Engine.GameScenarioManager.Scenario.State.MaxBounds.y);
        pos.z = pos.z.Clamp(Engine.GameScenarioManager.Scenario.State.MinBounds.z, Engine.GameScenarioManager.Scenario.State.MaxBounds.z);
        if (pos.x != orig.x || pos.z != orig.z)
        {
          announceOutOfBounds = true;
        }

        player.Position = pos;
        if (announceOutOfBounds)
          Engine.Screen2D.MessageSecondaryText("You are going out of bounds! Return to the battle!"
                                     , 5
                                     , ColorLocalization.Get(ColorLocalKeys.WHITE)
                                     , 99);
      }
    }

    private void ScanCargo()
    {
      ActorInfo player = Actor;
      ActorInfo target = TargetActor;
      if (CargoModel.ScanCargo(Engine, player, target) == CargoModel.CargoScanResult.NEW_SCAN)
      {
        // scan cargo
        target.Cargo.Scanned = true;
        target.OnCargoScannedEvent();
      }
    }

    public void ChangeSpeed(float frac)
    {
      if (frac == 0)
        return;
      if (IsMovementControlsEnabled && !PlayerAIEnabled && Actor != null)
      {
        float spd = Actor.TypeInfo.MoveLimitData.MaxSpeedChangeRate * Engine.Game.TimeSinceRender;
        Actor.MoveData.Speed += frac * spd;
        Actor.MoveData.Speed = Actor.MoveData.Speed.Clamp(Actor.MoveData.MinSpeed, Actor.MoveData.MaxSpeed);
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

    private void FireWeapon(WeaponShotInfo weapon)
    {
      if (IsMovementControlsEnabled && !PlayerAIEnabled)
      {
        ActorInfo.FireWeapon(Engine, Actor, TargetActor, weapon);
      }
    }

    private void ParseWeapons()
    {
      if (Actor != null)
      {
        if (PrimaryWeapon.IsNull)
          ResetPrimaryWeapon();

        if (SecondaryWeapon.IsNull)
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

    public void NextPrimaryWeapon()
    {
      if (Actor == null)
        return;

      PrimaryWeaponN++;
      if (Actor.WeaponDefinitions.PrimaryWeapons.Length <= PrimaryWeaponN)
        PrimaryWeaponN = 0;
    }

    public void PrevPrimaryWeapon()
    {
      if (Actor == null)
        return;

      PrimaryWeaponN--;
      if (PrimaryWeaponN < 0)
        PrimaryWeaponN = Actor.WeaponDefinitions.PrimaryWeapons.Length - 1;
    }

    public void NextSecondaryWeapon()
    {
      if (Actor == null)
        return;

      SecondaryWeaponN++;
      if (Actor.WeaponDefinitions.SecondaryWeapons.Length <= SecondaryWeaponN)
        SecondaryWeaponN = 0;
    }

    public void PrevSecondaryWeapon()
    {
      if (Actor == null)
        return;

      SecondaryWeaponN--;
      if (SecondaryWeaponN < 0)
        SecondaryWeaponN = Actor.WeaponDefinitions.SecondaryWeapons.Length - 1;
    }

    public void SquadronAssist()
    {
      if (Actor == null || Actor.Squad.IsNull)
        return;

      if (Actor.Squad.Leader != Actor)
      {
        Engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.SQUAD_NOACK), 5, FactionColor);
        return;
      }

      ActorInfo t = TargetActor;
      if (t != null)
      {
        if (Actor.IsAlliedWith(t))
        {
          Engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.SQUAD_DEFEND_ACK).F(t.TopParent.Name), 5, FactionColor);
          Actor.Squad.Mission = new AI.Squads.Missions.AssistActor(t.TopParent);
        }
        else
        {
          Engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.SQUAD_ATTACK_ACK).F(t.Name), 5, FactionColor);
          Actor.Squad.Mission = new AI.Squads.Missions.AttackActor(t, 99999);
          // add ons attack immediately
          foreach (ActorInfo c in Actor.Children)
          {
            if (c.UseParentCoords)
            {
              c.ClearQueue();
              c.QueueFirst(AttackActor.GetOrCreate(t.ID));
            }
          }
        }
      }
    }

    public void SquadronFree()
    {
      if (Actor == null || Actor.Squad.IsNull)
        return;

      if (Actor.Squad.Leader != Actor)
      {
        Engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.SQUAD_NOACK), 5, FactionColor);
        return;
      }

      Engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.SQUAD_CLEAR_ACK), 5, FactionColor);
      Actor.Squad.Mission = null;
    }

    public void FlashHit(COLOR color)
    {
      if (Actor.TargetType.Has(TargetType.FIGHTER))
      {
        Engine.SoundManager.SetSound(SoundGlobals.ExpHit);
        Engine.TrueVision.TVGraphicEffect.Flash(color.fR, color.fG, color.fB, 200);

        if (Actor.HP > 0 && DamagedReportSound != null && DamagedReportSound.Length > 0)
        {
          double r = Engine.Random.NextDouble();
          int dmgnum = DamagedReportSound.Length;

          int dmgst = (int)((dmgnum + 1) * Actor.HP_Frac);
          if (dmgst < DamagedReportSound.Length)
            if (r < 0.25f * (dmgnum - dmgst) / dmgnum)
              Engine.SoundManager.SetSound(DamagedReportSound[dmgst]);
        }
      }
    }
  }
}
