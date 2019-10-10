using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives.Extensions;
using SWEndor.Sound;
using SWEndor.Weapons;

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

    public string[] DamagedReportSound = SoundGlobals.DmgSounds;

    // weapons
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

    public void Update()
    {
      //UpdatePosition();
      UpdateStats();
      UpdateBounds();
    }

    //private void UpdatePosition()
    //{
    //  if (Actor != null)
    //    Position = Actor.GetGlobalPosition();
    //}

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
      if (StrengthFrac > 0 && StrengthFrac < 0.1f)
      {
        if (m_LowAlarmSoundTime < Engine.Game.GameTime)
        {
          Engine.SoundManager.SetSound(SoundGlobals.LowHP);
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
        Actor.MoveData.Speed += frac * Actor.TypeInfo.MoveLimitData.MaxSpeedChangeRate * Engine.Game.TimeSinceRender;
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
        if (Actor.Faction.IsAlliedWith(t.Faction))
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

    public void FlashHit(TV_COLOR color)
    {
      if (Actor.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
      {
        Engine.SoundManager.SetSound(SoundGlobals.ExpHit);
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

    public ScoreInfo Score;

    public bool IsMovementControlsEnabled = true;
    public bool IsTorpedoMode
    {
      get
      {
        return (PrimaryWeapon.Weapon.Type == WeaponType.TORPEDO) 
          || (SecondaryWeapon.Weapon.Type == WeaponType.TORPEDO);
      }
    }
    public int TargetActorID = -1;
    public ActorInfo TargetActor { get { return Engine.ActorFactory.Get(TargetActorID); } }
    public bool LockTarget = false;

    public bool PlayerAIEnabled = false;
  }
}
