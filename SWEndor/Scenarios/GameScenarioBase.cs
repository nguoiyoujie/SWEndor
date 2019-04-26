using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.UI.Menu.Pages;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GameScenarioBase
  {
    public string Name = "Untitled Scenario";
    public string Description = "";
    public List<ActorTypeInfo> AllowedWings = new List<ActorTypeInfo>();
    public List<string> AllowedDifficulties = new List<string> { "normal" };

    public string Difficulty { get; set; }
    public int StageNumber { get; set; }

    public DeathCamMode DeathCamMode = DeathCamMode.CIRCLE;
    private CameraMode[] m_PlayerCameraModes = new CameraMode[] { CameraMode.FIRSTPERSON };
    private int m_PlayerModeNum = 0;
    public CameraMode[] PlayerCameraModes
    {
      get { return m_PlayerCameraModes; }
      set
      {
        m_PlayerCameraModes = value;
        if (m_PlayerCameraModes == null || m_PlayerCameraModes.Length == 0)
          Manager.Engine.PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;
        else
        {
          for (int i = 0; i < m_PlayerCameraModes.Length; i++)
            if (m_PlayerCameraModes[i] == Manager.Engine.PlayerCameraInfo.CameraMode)
            {
              m_PlayerModeNum = i;
              return;
            }
          Manager.Engine.PlayerCameraInfo.CameraMode = m_PlayerCameraModes[0];
        }
      }
    }

    public CameraMode NextCameraMode()
    {
      if (m_PlayerCameraModes == null || m_PlayerCameraModes.Length == 0)
        return CameraMode.FIRSTPERSON;

      m_PlayerModeNum++;
      if (m_PlayerModeNum >= m_PlayerCameraModes.Length)
        m_PlayerModeNum = 0;

      return m_PlayerCameraModes[m_PlayerModeNum];
    }

    public CameraMode PrevCameraMode()
    {
      if (m_PlayerCameraModes == null || m_PlayerCameraModes.Length == 0)
        return CameraMode.FIRSTPERSON;

      m_PlayerModeNum--;
      if (m_PlayerModeNum < 0)
        m_PlayerModeNum = m_PlayerCameraModes.Length - 1;

      return m_PlayerCameraModes[m_PlayerModeNum];
    }

    public GameEvent MakePlayer;
    public float TimeSinceLostWing = -100;
    public float TimeSinceLostShip = -100;
    public float TimeSinceLostStructure = -100;
    public bool Launched = false;

    public FactionInfo MainAllyFaction = FactionInfo.Neutral;
    public FactionInfo MainEnemyFaction = FactionInfo.Neutral;

    public readonly GameScenarioManager Manager;

    public GameScenarioBase(GameScenarioManager manager)
    {
      Manager = manager;
    }

    public virtual void Load(ActorTypeInfo wing, string difficulty)
    {
      Difficulty = difficulty;
      StageNumber = 0;
      this.GetEngine().PlayerInfo.ActorType = wing;
    }

    public virtual void Launch()
    {
      Manager.Scenario = this;
      this.GetEngine().PlayerInfo.Score.Reset();
      LoadFactions();
      LoadScene();
      Launched = true;
    }

    public virtual void LoadFactions()
    {
      FactionInfo.Factory.Clear();
      MainAllyFaction = FactionInfo.Neutral;
      MainEnemyFaction = FactionInfo.Neutral;
    }

    public virtual void LoadScene()
    {
      this.GetEngine().TrueVision.TVGraphicEffect.FadeIn();
    }

    public virtual void GameTick()
    {
    }

    public virtual void Unload()
    {
      Launched = false;
      Manager.Scenario = null;

      // Full reset
      this.GetEngine().ActorFactory.Reset();

      Manager.ClearGameStates();
      Manager.ClearEvents();
      this.GetEngine().Screen2D.ClearText();
      this.GetEngine().PlayerInfo.RequestSpawn = false;

      // clear sounds
      this.GetEngine().SoundManager.SetSoundStopAll();

      this.GetEngine().Screen2D.OverrideTargetingRadar = false;
      this.GetEngine().Screen2D.Box3D_Enable = false;

      this.GetEngine().LandInfo.Enabled = false;

      // deleted many things, and this function is called when the game is not active. Probably safe to force GC
      GC.Collect();
    }

    public void FadeOut(params object[] param)
    {
      this.GetEngine().TrueVision.TVGraphicEffect.FadeOut();
      Manager.AddEvent(this.GetEngine().Game.GameTime + 0.01f, FadeInterim);
    }

    public void FadeInterim(params object[] param)
    {
      if (this.GetEngine().TrueVision.TVGraphicEffect.IsFadeFinished())
      {
        this.GetEngine().TrueVision.TVScreen2DImmediate.Action_Begin2D();
        this.GetEngine().TrueVision.TVScreen2DImmediate.Draw_FilledBox(0, 0, this.GetEngine().ScreenWidth, this.GetEngine().ScreenHeight, new TV_COLOR(0, 0, 0, 1).GetIntColor());
        this.GetEngine().TrueVision.TVScreen2DImmediate.Action_End2D();

        if (Manager.GetGameStateB("GameOver"))
        {
          GameOver();
          return;
        }
        else if (Manager.GetGameStateF("PlayCutsceneSequence", -1) != -1)
        {
          PlayCutsceneSequence(new object[] { Manager.GetGameStateF("PlayCutsceneSequence", -1) });
          return;
        }
        else if (Manager.GetGameStateB("GameWon"))
        {
          GameWonSequence();
          return;
        }

        MakePlayer?.Invoke(null);

        FadeIn();
        Manager.IsCutsceneMode = false;
      }
      else
      {
         Manager.AddEvent(this.GetEngine().Game.GameTime + 0.01f, FadeInterim);
      }
    }

    public void FadeIn(params object[] param)
    {
      this.GetEngine().TrueVision.TVGraphicEffect.FadeIn();
    }

    public void GameOver(params object[] param)
    {
      this.GetEngine().TrueVision.TVGraphicEffect.FadeIn(2.5f);

      this.GetEngine().SoundManager.SetSoundStopAll();

      this.GetEngine().Screen2D.CurrentPage = new GameOver(this.GetEngine().Screen2D);
      this.GetEngine().Screen2D.ShowPage = true;
      this.GetEngine().Game.IsPaused = true;
      this.GetEngine().SoundManager.SetMusic("battle_3_2"); // make modifiable
    }

    public void LostWing()
    {
      float t = this.GetEngine().Game.GameTime;
      if (TimeSinceLostWing > this.GetEngine().Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > this.GetEngine().Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > this.GetEngine().Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < this.GetEngine().Game.GameTime + 3f)
      {
          Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostWing = this.GetEngine().Game.GameTime + 3f;
    }

    public void LostShip()
    {
      float t = this.GetEngine().Game.GameTime;
      if (TimeSinceLostWing > this.GetEngine().Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > this.GetEngine().Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > this.GetEngine().Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < this.GetEngine().Game.GameTime + 3f)
      {
        Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = this.GetEngine().Game.GameTime + 3f;
    }

    public void LostStructure()
    {
      float t = this.GetEngine().Game.GameTime;
      if (TimeSinceLostWing > this.GetEngine().Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > this.GetEngine().Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > this.GetEngine().Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < this.GetEngine().Game.GameTime + 3f)
      {
        Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = this.GetEngine().Game.GameTime + 3f;
    }

    public void LostSound(object[] param)
    {
      if (!Manager.IsCutsceneMode)
      {
        this.GetEngine().SoundManager.SetSound("beep-22", true);
      }
    }

    public Dictionary<string, ActorInfo> GetRegister(string key)
    {
      switch (key.ToLower())
      {
        //case "enemystructures":
        //  return Manager.EnemyStructures;
        //case "allyfighters":
        //  return Manager.AllyFighters;
        //case "allyships":
        //  return Manager.AllyShips;
        //case "allystructures":
        //  return Manager.AllyStructures;
        //case "enemyfighters":
        //  return Manager.EnemyFighters;
        //case "enemyships":
        //  return Manager.EnemyShips;

        case "criticalallies":
          return Manager.CriticalAllies;
        case "criticalenemies":
          return Manager.CriticalEnemies;
        default:
          return null;
      }
    }

    public void SpawnActor(params object[] param)
    {
      // Format: Object[]

      if (param == null 
        || param.GetLength(0) < 10
        || !(param[0] is ActorTypeInfo)
        || !(param[1] is string)
        || !(param[2] is string)
        || !(param[3] is string)
        || !(param[4] is float)
        || !(param[5] is FactionInfo)
        || !(param[6] is TV_3DVECTOR)
        || !(param[7] is TV_3DVECTOR)
        || !(param[8] is ActionInfo[])
        || !(param[9] is string[])
        )
        return;

      new ActorSpawnInfo
      {
        Type = (ActorTypeInfo)param[0],
        Name = (string)param[1],
        RegisterName = (string)param[2],
        SidebarName = (string)param[3],
        SpawnTime = (float)param[4],
        Faction = (FactionInfo)param[5],
        Position = (TV_3DVECTOR)param[6],
        Rotation = (TV_3DVECTOR)param[7],
        Actions = (ActionInfo[])param[8],
        Registries = (string[])param[9]
      }.Spawn(this);
    }

    public void RegisterEvents(ActorInfo actor)
    {
      actor.CreatedEvents += ProcessCreated;
      actor.DestroyedEvents += ProcessKilled;
      actor.HitEvents += ProcessHit;
      actor.ActorStateChangeEvents += ProcessStateChange;
      actor.TickEvents += ProcessTick; 
    }

    public virtual void ProcessCreated(params object[] param)
    {
    }

    public virtual void ProcessKilled(params object[] param)
    {
    }

    public virtual void ProcessPlayerDying(params object[] param)
    {
      ActorInfo ainfo = this.GetEngine().ActorFactory.Get((int)param[0]);
      if (ainfo != null)
      {
        this.GetEngine().PlayerInfo.TempActorID = ainfo.ID;

        if (this.GetEngine().PlayerInfo.Actor.TypeInfo is DeathCameraATI)
          if (this.GetEngine().PlayerInfo.Actor.CreationState == CreationState.ACTIVE)
            this.GetEngine().PlayerInfo.Actor.SetLocalPosition(ainfo.GetLocalPosition().x, ainfo.GetLocalPosition().y, ainfo.GetLocalPosition().z);
      }
    }

    public virtual void ProcessPlayerKilled(params object[] param)
    {
      Manager.IsCutsceneMode = true;
      Manager.AddEvent(this.GetEngine().Game.GameTime + 3f, FadeOut);
      if (this.GetEngine().PlayerInfo.Lives == 0)
        Manager.SetGameStateB("GameOver", true);
    }

    public virtual void ProcessTick(params object[] param)
    {
    }

    public virtual void ProcessStateChange(params object[] param)
    {
    }

    public virtual void ProcessHit(params object[] param)
    {
      ActorInfo av = this.GetEngine().ActorFactory.Get((int)param[0]);
      ActorInfo aa = this.GetEngine().ActorFactory.Get((int)param[1]);

      if (this.GetEngine().PlayerInfo.Actor != null
        && av.Faction != null
        && av.Faction.IsAlliedWith(this.GetEngine().PlayerInfo.Actor.Faction))
      {
        List<int> attackerfamily = aa.GetAllParents();
        foreach (int i in attackerfamily)
        {
          ActorInfo a = this.GetEngine().ActorFactory.Get(i);
          if (this.GetEngine().PlayerInfo.Actor == a)
          {
            this.GetEngine().Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", av.Name, this.GetEngine().PlayerInfo.Name)
                                                        , 5
                                                        , av.Faction.Color
                                                        , -1);
          }
        }
      }
    }

    public virtual void PlayCutsceneSequence(object[] param = null)
    {

    }

    public virtual void GameWonSequence(object[] param = null)
    {
      this.GetEngine().TrueVision.TVGraphicEffect.FadeIn(2.5f);

      this.GetEngine().SoundManager.SetSoundStopAll();

      this.GetEngine().Screen2D.CurrentPage = new GameWon(this.GetEngine().Screen2D);
      this.GetEngine().Screen2D.ShowPage = true;
      this.GetEngine().Game.IsPaused = true;
      this.GetEngine().SoundManager.SetMusic("finale_3_1");
      this.GetEngine().SoundManager.SetMusicLoop("credits_3_1");
    }
  }
}
