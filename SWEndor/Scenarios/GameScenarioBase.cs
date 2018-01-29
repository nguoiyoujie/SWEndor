using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class GameScenarioBase
  {
    public string Name = "Untitled Scenario";
    public List<ActorTypeInfo> AllowedWings = new List<ActorTypeInfo> { XWingATI.Instance() };
    public List<string> AllowedDifficulties = new List<string> { "normal" };

    public float RebelSpawnTime = 0;
    public float TIESpawnTime = 0;
    public ScenarioEvent MakePlayer;
    public float TimeSinceLostWing = -100;
    public float TimeSinceLostShip = -100;

    public int RebelFighterLimit = 60;
    public int RebelFighersLost = 0; // do we need those?
    public int RebelShipsLost = 0; // do we need those?

    public virtual void Load(ActorTypeInfo wing, string difficulty)
    {
      GameScenarioManager.Instance().Scenario = this;
      GameScenarioManager.Instance().Difficulty = difficulty;
      PlayerInfo.Instance().ActorType = wing;

      LoadFactions();
      LoadScene();
    }

    public virtual void LoadFactions()
    {
      FactionInfo.Reset();
    }

    public virtual void LoadScene()
    {
      Engine.Instance().TVGraphicEffect.FadeIn();
    }

    public virtual void RegisterEvents()
    {
      GameEvent.ClearEvents();

      GameEvent.RegisterEvent("Common_FadeIn", FadeIn);
      GameEvent.RegisterEvent("Common_FadeInterim", FadeInterim);
      GameEvent.RegisterEvent("Common_FadeOut", FadeOut);
      GameEvent.RegisterEvent("Common_LostSound", LostSound);

      GameEvent.RegisterEvent("Common_SpawnActor", SpawnActor);

      GameEvent.RegisterEvent("Common_ProcessCreated", ProcessCreated);
      GameEvent.RegisterEvent("Common_ProcessKilled", ProcessKilled);
      GameEvent.RegisterEvent("Common_ProcessTick", ProcessTick);
      GameEvent.RegisterEvent("Common_ProcessStateChange", ProcessStateChange);
      GameEvent.RegisterEvent("Common_ProcessHit", ProcessHit);
      GameEvent.RegisterEvent("Common_ProcessPlayerDying", ProcessPlayerDying);
      GameEvent.RegisterEvent("Common_ProcessPlayerKilled", ProcessPlayerKilled);
    }

    public virtual void GameTick()
    {
      int lostwings = GameScenarioManager.Instance().UpdateActorLists(GameScenarioManager.Instance().AllyFighters);
      if (lostwings > 0)
      {
        RebelFighterLimit -= lostwings;
        RebelFighersLost+= lostwings;
        LostWing();
      }

      int lostships = GameScenarioManager.Instance().UpdateActorLists(GameScenarioManager.Instance().AllyShips);
      if (lostships > 0)
      {
        RebelShipsLost += lostships;
        LostShip();
      }

      if (RebelFighterLimit < GameScenarioManager.Instance().AllyFighters.Count)
        RebelFighterLimit = GameScenarioManager.Instance().AllyFighters.Count;
    }

    public virtual void Unload()
    {
      GameScenarioManager.Instance().Scenario = null;

      // Full reset
      foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
      {
        if (a != null)
          a.Destroy();
      }
      GameScenarioManager.Instance().ClearGameStates();
      GameScenarioManager.Instance().ClearEvents();
      Screen2D.Instance().ClearText();

      // clear sounds
      PlayerInfo.Instance().exp_cazavol = 0;
      PlayerInfo.Instance().exp_navevol = 0;
      PlayerInfo.Instance().exp_restvol = 0;
      PlayerInfo.Instance().enginefcvol = 0;
      PlayerInfo.Instance().enginesmvol = 0;
      PlayerInfo.Instance().enginelgvol = 0;
      PlayerInfo.Instance().enginetevol = 0;
      SoundManager.Instance().SetSoundStopAll();
      Screen2D.Instance().OverrideTargetingRadar = false;
    }

    public void FadeOut(object[] param = null)
    {
      Engine.Instance().TVGraphicEffect.FadeOut();
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.01f, "Common_FadeInterim");
    }

    public void FadeInterim(object[] param = null)
    {
      if (Engine.Instance().TVGraphicEffect.IsFadeFinished())
      {
        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(0, 0, Engine.Instance().ScreenWidth, Engine.Instance().ScreenHeight, new TV_COLOR(0, 0, 0, 1).GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Action_End2D();

        if (GameScenarioManager.Instance().GetGameStateB("GameOver"))
        {
          GameOver();
          return;
        }
        else if (GameScenarioManager.Instance().GetGameStateB("GameWon"))
        {
          GameWonSequence(null);
          return;
        }

        if (MakePlayer != null)
        {
          MakePlayer(null);
          GameScenarioManager.Instance().SetGameStateB("PendingPlayerSpawn", false);
        }
        FadeIn();
        GameScenarioManager.Instance().IsCutsceneMode = false;
      }
      else
      {
         GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.01f, "Common_FadeInterim");
      }
    }

    public void FadeIn(object[] param = null)
    {
      Engine.Instance().TVGraphicEffect.FadeIn();
    }

    public void GameOver(object[] param = null)
    {
      Engine.Instance().TVGraphicEffect.FadeIn(2.5f);
      Screen2D.Instance().CurrentPage = new UIPage_GameOver();
      Screen2D.Instance().ShowPage = true;
      Game.Instance().IsPaused = true;
      SoundManager.Instance().SetMusic("battle_3_2");
    }

    public void LostWing()
    {
      float t = Game.Instance().GameTime;
      if (TimeSinceLostWing > Game.Instance().GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.Instance().GameTime)
        t = TimeSinceLostShip;

      while (t < Game.Instance().GameTime + 3f)
      {
          GameScenarioManager.Instance().AddEvent(t, "Common_LostSound");
        t += 0.2f;
      }
      TimeSinceLostWing = Game.Instance().GameTime + 3f;
    }

    public void LostShip()
    {
      float t = Game.Instance().GameTime;
      if (TimeSinceLostWing > Game.Instance().GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.Instance().GameTime)
        t = TimeSinceLostShip;

      while (t < Game.Instance().GameTime + 3f)
      {
        GameScenarioManager.Instance().AddEvent(t, "Common_LostSound");
        t += 0.2f;
      }
      TimeSinceLostShip = Game.Instance().GameTime + 3f;
    }

    public void LostSound(object[] param)
    {
      if (!GameScenarioManager.Instance().IsCutsceneMode)
      {
        SoundManager.Instance().SetSound("shieldlow", true);
      }
    }

    public ActorInfo SpawnActor(ActorTypeInfo type, string unit_name, string register_name, string sidebar_name, float spawntime, FactionInfo faction, TV_3DVECTOR position, TV_3DVECTOR rotation, ActionInfo[] actions, Dictionary<string, ActorInfo>[] registries)
    {
      ActorCreationInfo acinfo;
      ActorInfo ainfo;

      acinfo = new ActorCreationInfo(type);
      if (unit_name != "")
        acinfo.Name = unit_name;
      acinfo.Faction = faction;
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = spawntime;
      acinfo.Position = position;
      acinfo.Rotation = rotation;
      ainfo = ActorInfo.Create(acinfo);
      ainfo.SideBarName = sidebar_name;

      if (actions != null)
      {
        foreach (ActionInfo act in actions)
        {
          ActionManager.QueueLast(ainfo, act);
        }
      }

      if (registries != null)
      {
        foreach (Dictionary<string, ActorInfo> reg in registries)
        {
          if (register_name != "")
            reg.Add(register_name, ainfo);
          else
            reg.Add(ainfo.Key, ainfo);
        }
      }

      RegisterEvents(ainfo);

      return ainfo;
    }


    public void SpawnActor(object[] param)
    {
      // Format: Object[]
      //  [0]: ActorType to spawn
      //  [1]: Spawn name
      //  [2]: Faction
      //  [3]: Spawn position
      //  [4]: Spawn rotation
      //  [5]: ActionInfo[]

      if (param == null 
        || param.GetLength(0) < 7 
        || !(param[0] is ActorTypeInfo)
        || !(param[1] is string)
        || !(param[2] is float)
        || !(param[3] is FactionInfo)
        || !(param[4] is TV_3DVECTOR)
        || !(param[5] is TV_3DVECTOR)
        || !(param[6] is ActionInfo[])
        )
        return;

      ActorCreationInfo acinfo;
      ActorInfo ainfo;
      ActorTypeInfo type = (ActorTypeInfo)param[0];
      string name = (string)param[1];
      float spawntime = (float)param[2];
      FactionInfo faction = (FactionInfo)param[3];
      TV_3DVECTOR position = (TV_3DVECTOR)param[4];
      TV_3DVECTOR rotation = (TV_3DVECTOR)param[5];
      ActionInfo[] actions = (ActionInfo[])param[6];

      acinfo = new ActorCreationInfo(type);
      acinfo.Name = name;
      acinfo.Faction = faction;
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = spawntime;
      acinfo.Position = position;
      acinfo.Rotation = new TV_3DVECTOR();
      ainfo = ActorInfo.Create(acinfo);

      foreach (ActionInfo act in actions)
      {
        ActionManager.QueueLast(ainfo, act);
      }
    }

    public void RegisterEvents(ActorInfo actor)
    {
      actor.CreatedEvents.Add("Common_ProcessCreated");
      actor.DestroyedEvents.Add("Common_ProcessKilled");
      actor.HitEvents.Add("Common_ProcessHit");
      actor.ActorStateChangeEvents.Add("Common_ProcessStateChange");
      actor.TickEvents.Add("Common_ProcessTick");
    }

    public void ProcessCreated(object[] param)
    {
    }

    public void ProcessKilled(object[] param)
    {
      //if (param.GetLength(0) < 1 || param[0] == null)
        //return;

      //ActorInfo av = (ActorInfo)param[0];

      //if (PlayerInfo.Instance().Actor == av)
      //{
      //  GameScenarioManager.Instance().AddEvent(Game.Instance().Time + 3f, "Common_FadeOut");
      //}
    }


    public void ProcessPlayerDying(object[] param)
    {
      if (param.GetLength(0) < 1 || param[0] == null)
        return;

      ActorInfo ainfo = (ActorInfo)param[0];

      if (PlayerInfo.Instance().Actor.TypeInfo is DeathCameraATI)
      {
        if (PlayerInfo.Instance().Actor.CreationState == CreationState.ACTIVE)
        {
          PlayerInfo.Instance().Actor.SetLocalPosition(ainfo.GetLocalPosition().x, ainfo.GetLocalPosition().y, ainfo.GetLocalPosition().z);
        }
      }
    }

    public void ProcessPlayerKilled(object[] param)
    {
      GameScenarioManager.Instance().IsCutsceneMode = true;

      GameScenarioManager.Instance().SetGameStateB("PendingPlayerSpawn", true);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3f, "Common_FadeOut");
      if (PlayerInfo.Instance().Lives == 0)
        GameScenarioManager.Instance().SetGameStateB("GameOver", true);
    }

    public void ProcessTick(object[] param)
    {


    }

    public void ProcessStateChange(object[] param)
    {
      /*
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      ActorInfo av = (ActorInfo)param[0];
      ActorState st = (ActorState)param[1];

      if (PlayerInfo.Instance().Actor == av)
      {
        if (st != ActorState.DYING && (av.ActorState == ActorState.DYING || av.ActorState == ActorState.DEAD))
        {

        }
      }
      */
    }

    public void ProcessHit(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      ActorInfo av = (ActorInfo)param[0];
      ActorInfo aa = (ActorInfo)param[1];

      if (PlayerInfo.Instance().Actor != null
        && av.Faction != null
        && av.Faction.IsAlliedWith(PlayerInfo.Instance().Actor.Faction))
      {
        List<ActorInfo> attackerfamily = aa.GetAllParents();
        foreach (ActorInfo a in attackerfamily)
        {
          if (PlayerInfo.Instance().Actor == a)
          {
            Screen2D.Instance().UpdateText(string.Format("{0}: {1}, watch your fire!", av.Name, PlayerInfo.Instance().Name)
                                                        , Game.Instance().GameTime + 5f
                                                        , av.Faction.Color);
          }
        }
      }
    }

    public virtual void GameWonSequence(object[] param)
    {

    }
  }
}
