using System;
using System.Collections.Generic;
using MTV3D65;
using SWEndor.Scenarios.Scripting;
using SWEndor.Actors;
using SWEndor.AI.Actions;
using SWEndor.AI;
using SWEndor.Scenarios;
using SWEndor.Sound;
using SWEndor.ActorTypes;
using SWEndor.Player;

namespace SWEndor.Evaluator
{
  public class SWFunctions : Functions
  {
    public void InitSW()
    {
      // Scene Management
      Add("SetMaxBounds".ToLowerInvariant(), new StaticFunction("SetMaxBounds", SetMaxBounds, 3, 3));
      Add("SetMinBounds".ToLowerInvariant(), new StaticFunction("SetMinBounds", SetMinBounds, 3, 3));
      Add("SetMaxAIBounds".ToLowerInvariant(), new StaticFunction("SetMaxAIBounds", SetMaxAIBounds, 3, 3));
      Add("SetMinAIBounds".ToLowerInvariant(), new StaticFunction("SetMinAIBounds", SetMinAIBounds, 3, 3));
      Add("FadeOut".ToLowerInvariant(), new StaticFunction("FadeOut", FadeOut, 0, 0));

      // Scene Camera Management
      //       GameScenarioManager.Instance().SceneCamera.SetLocalPosition(10, -20, 1500);
      Add("Actor_SetSceneCameraAsActive".ToLowerInvariant(), new StaticFunction("Actor_SetSceneCameraAsActive", Actor_SetSceneCameraAsActive, 0, 0));


      // Actor Management
      Add("Actor_Spawn".ToLowerInvariant(), new StaticFunction("Actor_Spawn", Actor_Spawn, 12, int.MaxValue));
      Add("Actor_SetActive".ToLowerInvariant(), new StaticFunction("Actor_SetActive", Actor_SetActive, 1, 1));
      Add("Actor_IsAlive".ToLowerInvariant(), new StaticFunction("Actor_IsAlive", Actor_IsAlive, 0, 0));
      Add("Actor_RegisterEvents".ToLowerInvariant(), new StaticFunction("Actor_RegisterEvents", Actor_RegisterEvents, 0, 0));
      Add("Actor_GetLocalPosition".ToLowerInvariant(), new StaticFunction("Actor_GetLocalPosition", Actor_GetLocalPosition, 0, 0));
      Add("Actor_SetLocalPosition".ToLowerInvariant(), new StaticFunction("Actor_SetLocalPosition", Actor_SetLocalPosition, 3, 3));
      Add("Actor_GetLocalRotation".ToLowerInvariant(), new StaticFunction("Actor_GetLocalRotation", Actor_GetLocalRotation, 0, 0));
      Add("Actor_SetLocalRotation".ToLowerInvariant(), new StaticFunction("Actor_SetLocalRotation", Actor_SetLocalRotation, 3, 3));
      Add("Actor_GetLocalDirection".ToLowerInvariant(), new StaticFunction("Actor_GetLocalDirection", Actor_GetLocalDirection, 0, 0));
      Add("Actor_SetLocalDirection".ToLowerInvariant(), new StaticFunction("Actor_SetLocalDirection", Actor_SetLocalDirection, 3, 3));
      Add("Actor_GetPosition".ToLowerInvariant(), new StaticFunction("Actor_GetPosition", Actor_GetPosition, 0, 0));
      Add("Actor_GetRotation".ToLowerInvariant(), new StaticFunction("Actor_GetRotation", Actor_GetRotation, 0, 0));
      Add("Actor_SetRotation".ToLowerInvariant(), new StaticFunction("Actor_SetRotation", Actor_SetRotation, 3, 3));
      Add("Actor_GetDirection".ToLowerInvariant(), new StaticFunction("Actor_GetDirection", Actor_GetDirection, 0, 0));
      Add("Actor_SetDirection".ToLowerInvariant(), new StaticFunction("Actor_SetDirection", Actor_SetDirection, 3, 3));
      Add("Actor_GetStateB".ToLowerInvariant(), new StaticFunction("Actor_GetStateB", Actor_GetStateB, 1, 2));
      Add("Actor_SetStateB".ToLowerInvariant(), new StaticFunction("Actor_SetStateB", Actor_SetStateB, 2, 2));
      Add("Actor_GetStateF".ToLowerInvariant(), new StaticFunction("Actor_GetStateF", Actor_GetStateF, 1, 2));
      Add("Actor_SetStateF".ToLowerInvariant(), new StaticFunction("Actor_SetStateF", Actor_SetStateF, 2, 2));
      Add("Actor_GetStateS".ToLowerInvariant(), new StaticFunction("Actor_GetStateS", Actor_GetStateS, 1, 2));
      Add("Actor_SetStateS".ToLowerInvariant(), new StaticFunction("Actor_SetStateS", Actor_SetStateS, 2, 2));
      Add("Actor_LookAtPoint".ToLowerInvariant(), new StaticFunction("Actor_LookAtPoint", Actor_LookAtPoint, 3, 4));
      Add("Actor_GetProperty".ToLowerInvariant(), new StaticFunction("Actor_GetProperty", Actor_GetProperty, 1, 1));
      Add("Actor_SetProperty".ToLowerInvariant(), new StaticFunction("Actor_SetProperty", Actor_SetProperty, 2, 2));

      // Message Box
      Add("Message".ToLowerInvariant(), new StaticFunction("Message", Message, 6, int.MaxValue));

      // Action Management
      Add("Actor_QueueFirst".ToLowerInvariant(), new StaticFunction("Actor_QueueFirst", Actor_QueueFirst, 1, int.MaxValue));
      Add("Actor_QueueNext".ToLowerInvariant(), new StaticFunction("Actor_QueueNext", Actor_QueueNext, 1, int.MaxValue));
      Add("Actor_QueueLast".ToLowerInvariant(), new StaticFunction("Actor_QueueLast", Actor_QueueLast, 1, int.MaxValue));
      Add("Actor_UnlockActor".ToLowerInvariant(), new StaticFunction("Actor_UnlockActor", Actor_UnlockActor, 0, 0));
      Add("Actor_ClearQueue".ToLowerInvariant(), new StaticFunction("Actor_ClearQueue", Actor_ClearQueue, 0, 0));
      Add("Actor_ForceClearQueue".ToLowerInvariant(), new StaticFunction("Actor_ForceClearQueue", Actor_ForceClearQueue, 0, 0));

      // Game states
      Add("GetGameTime".ToLowerInvariant(), new StaticFunction("GetGameTime", GetGameTime, 0, 0));
      Add("GetLastFrameTime".ToLowerInvariant(), new StaticFunction("GetLastFrameTime", GetLastFrameTime, 0, 0));
      Add("GetDifficulty".ToLowerInvariant(), new StaticFunction("GetDifficulty", GetDifficulty, 0, 0));
      Add("GetPlayerActorType".ToLowerInvariant(), new StaticFunction("GetPlayerActorType", GetPlayerActorType, 0, 0));
      Add("GetStageNumber".ToLowerInvariant(), new StaticFunction("GetStageNumber", GetStageNumber, 0, 0));
      Add("SetStageNumber".ToLowerInvariant(), new StaticFunction("SetStageNumber", SetStageNumber, 1, 1));
      Add("GetGameStateB".ToLowerInvariant(), new StaticFunction("GetGameStateB", GetGameStateB, 1, 2));
      Add("SetGameStateB".ToLowerInvariant(), new StaticFunction("SetGameStateB", SetGameStateB, 2, 2));
      Add("GetGameStateF".ToLowerInvariant(), new StaticFunction("GetGameStateF", GetGameStateF, 1, 2));
      Add("SetGameStateF".ToLowerInvariant(), new StaticFunction("SetGameStateF", SetGameStateF, 2, 2));
      Add("GetGameStateS".ToLowerInvariant(), new StaticFunction("GetGameStateS", GetGameStateS, 1, 2));
      Add("SetGameStateS".ToLowerInvariant(), new StaticFunction("SetGameStateS", SetGameStateS, 2, 2));
      Add("GetRegisterCount".ToLowerInvariant(), new StaticFunction("GetRegisterCount", GetRegisterCount, 1, 1));
      Add("GetTimeSinceLostWing".ToLowerInvariant(), new StaticFunction("GetTimeSinceLostWing", GetTimeSinceLostWing, 0, 0));
      Add("GetTimeSinceLostShip".ToLowerInvariant(), new StaticFunction("GetTimeSinceLostShip", GetTimeSinceLostShip, 0, 0));
      Add("AddEvent".ToLowerInvariant(), new StaticFunction("AddEvent", AddEvent, 2, 2));

      // Player Management
      Add("Player_AssignActor".ToLowerInvariant(), new StaticFunction("Player_AssignActor", Player_AssignActor, 0, 1));
      Add("Player_SetMovementEnabled".ToLowerInvariant(), new StaticFunction("Player_SetMovementEnabled", Player_SetMovementEnabled, 1, 1));
      Add("Player_SetAI".ToLowerInvariant(), new StaticFunction("Player_SetAI", Player_SetAI, 1, 1));
      Add("Player_SetLives".ToLowerInvariant(), new StaticFunction("Player_SetLives", Player_SetLives, 1, 1));
      Add("Player_SetScorePerLife".ToLowerInvariant(), new StaticFunction("Player_SetScorePerLife", Player_SetScorePerLife, 1, 1));
      Add("Player_SetScoreForNextLife".ToLowerInvariant(), new StaticFunction("Player_SetScoreForNextLife", Player_SetScoreForNextLife, 1, 1));
      Add("Player_ResetScore".ToLowerInvariant(), new StaticFunction("Player_ResetScore", Player_ResetScore, 0, 0));

      // Faction
      Add("AddFaction".ToLowerInvariant(), new StaticFunction("AddFaction", AddFaction, 4, 4));
      Add("Faction_SetAsMainAllyFaction".ToLowerInvariant(), new StaticFunction("Faction_SetAsMainAllyFaction", Faction_SetAsMainAllyFaction, 1, 1));
      Add("Faction_SetAsMainEnemyFaction".ToLowerInvariant(), new StaticFunction("Faction_SetAsMainEnemyFaction", Faction_SetAsMainEnemyFaction, 1, 1));
      // Set ally / unset ally
      Add("Faction_MakeAlly".ToLowerInvariant(), new StaticFunction("Faction_MakeAlly", Faction_MakeAlly, 2, 2));
      Add("Faction_MakeEnemy".ToLowerInvariant(), new StaticFunction("Faction_MakeEnemy", Faction_MakeEnemy, 2, 2));

      // get Wing/Ship/Structure count
      Add("Faction_GetWingCount".ToLowerInvariant(), new StaticFunction("Faction_GetWingCount", Faction_GetWingCount, 1, 1));
      Add("Faction_GetShipCount".ToLowerInvariant(), new StaticFunction("Faction_GetShipCount", Faction_GetShipCount, 1, 1));
      Add("Faction_GetStructureCount".ToLowerInvariant(), new StaticFunction("Faction_GetStructureCount", Faction_GetStructureCount, 1, 1));
      // get Wing/Ship/Structure limit
      Add("Faction_GetWingLimit".ToLowerInvariant(), new StaticFunction("Faction_GetWingLimit", Faction_GetWingLimit, 1, 1));
      Add("Faction_GetShipLimit".ToLowerInvariant(), new StaticFunction("Faction_GetShipLimit", Faction_GetShipLimit, 1, 1));
      Add("Faction_GetStructureLimit".ToLowerInvariant(), new StaticFunction("Faction_GetStructureLimit", Faction_GetStructureLimit, 1, 1));
      // set Wing/Ship/Structure limit
      // get Wing/Ship/Structure spawn limit
      // set Wing/Ship/Structure spawn limit


      // Sounds and Music
      Add("SetMusic".ToLowerInvariant(), new StaticFunction("SetMusic", SetMusic, 1, 3));
      Add("SetMusicLoop".ToLowerInvariant(), new StaticFunction("SetMusicLoop", SetMusicLoop, 1, 2));
      Add("SetMusicPause".ToLowerInvariant(), new StaticFunction("SetMusicPause", SetMusicPause, 0, 0));
      Add("SetMusicResume".ToLowerInvariant(), new StaticFunction("SetMusicResume", SetMusicResume, 0, 0));
      Add("SetMusicStop".ToLowerInvariant(), new StaticFunction("SetMusicStop", SetMusicStop, 0, 0));

      // UI
      Add("SetUILine1Color".ToLowerInvariant(), new StaticFunction("SetUILine1Color", SetUILine1Color, 3, 3));
      Add("SetUILine2Color".ToLowerInvariant(), new StaticFunction("SetUILine2Color", SetUILine2Color, 3, 3));
      Add("SetUILine3Color".ToLowerInvariant(), new StaticFunction("SetUILine3Color", SetUILine3Color, 3, 3));
      Add("SetUILine1Text".ToLowerInvariant(), new StaticFunction("SetUILine1Text", SetUILine1Text, 1, 1));
      Add("SetUILine2Text".ToLowerInvariant(), new StaticFunction("SetUILine2Text", SetUILine2Text, 1, 1));
      Add("SetUILine3Text".ToLowerInvariant(), new StaticFunction("SetUILine3Text", SetUILine3Text, 1, 1));

      // Script flow
      Add("CallScript".ToLowerInvariant(), new StaticFunction("CallScript", CallScript, 1, 1));

      // Misc
      Add("IsNull".ToLowerInvariant(), new StaticFunction("IsNull", delegate (object[] ps) { return ps[0] == null; }, 1, 1));
      Add("GetArrayElement".ToLowerInvariant(), new StaticFunction("GetArrayElement", GetArrayElement, 2, 2));
    }

    #region Scene Management

    private static object SetMaxBounds(object[] ps)
    {
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object SetMinBounds(object[] ps)
    {
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object SetMaxAIBounds(object[] ps)
    {
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object SetMinAIBounds(object[] ps)
    {
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object FadeOut(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return false;

      GameScenarioManager.Instance().Scenario.FadeOut();
      return true;
    }

    #endregion

    #region Scene Camera Management

    private static object Actor_SetSceneCameraAsActive(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor = GameScenarioManager.Instance().SceneCamera;
      return true;
    }

    #endregion

    #region Actor Management

    private static object Actor_Spawn(object[] ps)
    {
      GameScenarioBase gscenario = GameScenarioManager.Instance().Scenario;
      if (gscenario == null)
        return "-1";

      ActorTypeInfo atype = ActorTypeInfo.Factory.Get(ps[0].ToString());
      string unitname = ps[1].ToString();
      string regname = ps[2].ToString();
      string sidebarname = ps[3].ToString();
      float spawntime = Convert.ToSingle(ps[4]);
      FactionInfo faction = FactionInfo.Factory.Get(ps[5].ToString());
      TV_3DVECTOR position = new TV_3DVECTOR(Convert.ToSingle(ps[6]), Convert.ToSingle(ps[7]), Convert.ToSingle(ps[8]));
      TV_3DVECTOR rotation = new TV_3DVECTOR(Convert.ToSingle(ps[9]), Convert.ToSingle(ps[10]), Convert.ToSingle(ps[11]));
      List<string> registries = new List<string>();

      for (int i = 12; i < ps.Length; i++)
      {
        registries.Add(ps[i].ToString());
      }

      ActorInfo res = gscenario.SpawnActor(atype, unitname, regname, sidebarname, spawntime, faction, position, rotation, null, registries.ToArray());

      if (gscenario == null)
        return "-1";
      return res.ID;
    }

    private static object Actor_SetActive(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return false;

      int id = Convert.ToInt32(ps[0].ToString());
      GameScenarioManager.Instance().Scenario.ActiveActor = ActorInfo.Factory.GetActor(id);
      return true;
    }

    private static object Actor_IsAlive(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      return GameScenarioManager.Instance().Scenario.ActiveActor.ActorState != ActorState.DEAD;
    }

    private static object Actor_RegisterEvents(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.RegisterEvents(GameScenarioManager.Instance().Scenario.ActiveActor);
      return true;
    }

    private static object Actor_GetLocalPosition(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetLocalPosition();
      return new float[] { vec.x, vec.y, vec.z };
    }

    private static object Actor_GetLocalRotation(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetLocalRotation();
      return new float[] { vec.x, vec.y, vec.z };
    }

    private static object Actor_GetLocalDirection(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetLocalDirection();
      return new float[] { vec.x, vec.y, vec.z };
    }

    private static object Actor_GetPosition(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetPosition();
      return new float[] { vec.x, vec.y, vec.z };
    }

    private static object Actor_GetRotation(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetRotation();
      return new float[] { vec.x, vec.y, vec.z };
    }

    private static object Actor_GetDirection(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return new float[] { 0, 0, 0 };

      TV_3DVECTOR vec = GameScenarioManager.Instance().Scenario.ActiveActor.GetDirection();
      return new float[] { vec.x, vec.y, vec.z };
    }

    private static object Actor_SetLocalPosition(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetLocalPosition(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object Actor_SetLocalRotation(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetLocalRotation(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object Actor_SetLocalDirection(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetLocalDirection(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object Actor_SetRotation(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetRotation(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object Actor_SetDirection(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetDirection(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      return true;
    }

    private static object Actor_GetStateB(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      if (ps.Length == 1)
        return GameScenarioManager.Instance().Scenario.ActiveActor.GetStateB(ps[0].ToString());
      else
        return GameScenarioManager.Instance().Scenario.ActiveActor.GetStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
    }

    private static object Actor_GetStateF(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      if (ps.Length == 1)
        return GameScenarioManager.Instance().Scenario.ActiveActor.GetStateF(ps[0].ToString());
      else
        return GameScenarioManager.Instance().Scenario.ActiveActor.GetStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
    }

    private static object Actor_GetStateS(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      if (ps.Length == 1)
        return GameScenarioManager.Instance().Scenario.ActiveActor.GetStateS(ps[0].ToString());
      else
        return GameScenarioManager.Instance().Scenario.ActiveActor.GetStateS(ps[0].ToString(), ps[1].ToString());
    }

    private static object Actor_SetStateB(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetStateB(ps[1].ToString(), Convert.ToBoolean(ps[2].ToString()));
      return true;
    }

    private static object Actor_SetStateF(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetStateF(ps[1].ToString(), Convert.ToInt32(ps[2].ToString()));
      return true;
    }

    private static object Actor_SetStateS(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      GameScenarioManager.Instance().Scenario.ActiveActor.SetStateS(ps[1].ToString(), ps[2].ToString());
      return true;
    }

    private static object Actor_LookAtPoint(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      TV_3DVECTOR vec = new TV_3DVECTOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()));
      if (ps.Length == 3)
        GameScenarioManager.Instance().Scenario.ActiveActor.LookAtPoint(vec);
      else
        GameScenarioManager.Instance().Scenario.ActiveActor.LookAtPoint(vec, Convert.ToBoolean(ps[3].ToString()));

      return true;
    }

    private static object Actor_GetProperty(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return null;

      object result = null;
      ConfigureActorProperty(GameScenarioManager.Instance().Scenario.ActiveActor, ps[0].ToString(), false, ref result);
      return result;
    }

    private static object Actor_SetProperty(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return null;

      ConfigureActorProperty(GameScenarioManager.Instance().Scenario.ActiveActor, ps[0].ToString(), true, ref ps[1]);
      return ps[1];
    }

    #endregion

    #region Message

    private static object Message(object[] ps)
    {
      string text = ps[0].ToString();
      float expiretime = Convert.ToSingle(ps[1].ToString());
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()), Convert.ToSingle(ps[5].ToString()));

      if (ps.Length <= 6)
        Screen2D.Instance().MessageText(text, expiretime, color);
      else
        Screen2D.Instance().MessageText(text, expiretime, color, Convert.ToInt32(ps[6].ToString()));
      return true;
    }

    #endregion

    #region Action and AI Management

    private static object Actor_QueueFirst(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      ActionManager.QueueFirst(GameScenarioManager.Instance().Scenario.ActiveActor, action);
      return true;
    }

    private static object Actor_QueueNext(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      ActionManager.QueueNext(GameScenarioManager.Instance().Scenario.ActiveActor, action);
      return true;
    }

    private static object Actor_QueueLast(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      ActionManager.QueueLast(GameScenarioManager.Instance().Scenario.ActiveActor, action);
      return true;
    }

    private static object Actor_UnlockActor(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionManager.Unlock(GameScenarioManager.Instance().Scenario.ActiveActor);
      return true;
    }

    private static object Actor_ClearQueue(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionManager.ClearQueue(GameScenarioManager.Instance().Scenario.ActiveActor);
      return true;
    }

    private static object Actor_ForceClearQueue(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionManager.ForceClearQueue(GameScenarioManager.Instance().Scenario.ActiveActor);
      return true;
    }

    #endregion

    #region Player Management

    private static object Player_AssignActor(object[] ps)
    {
      if (ps.Length == 0)
      {
        PlayerInfo.Instance().Actor = GameScenarioManager.Instance().Scenario.ActiveActor;
        return true;
      }
      
      int id = Convert.ToInt32(ps[0].ToString());
      if (id < 0)
      {
        PlayerInfo.Instance().Actor = null;
        return false;
      }

      ActorInfo a = ActorInfo.Factory.GetActor(id);
      if (a == null)
        return false;

      PlayerInfo.Instance().Actor = a;
      return true;
    }

    private static object Player_SetMovementEnabled(object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      PlayerInfo.Instance().IsMovementControlsEnabled = enabled;
      return true;
    }

    private static object Player_SetAI(object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      PlayerInfo.Instance().PlayerAIEnabled = enabled;
      return true;
    }

    private static object Player_SetLives(object[] ps)
    {
      PlayerInfo.Instance().Lives = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    private static object Player_SetScorePerLife(object[] ps)
    {
      PlayerInfo.Instance().ScorePerLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    private static object Player_SetScoreForNextLife(object[] ps)
    {
      PlayerInfo.Instance().ScoreForNextLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    private static object Player_ResetScore(object[] ps)
    {
      PlayerInfo.Instance().Score.Reset();
      return true;
    }

    #endregion

    #region Factions

    private static object AddFaction(object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), 1);
      FactionInfo.Factory.Add(ps[0].ToString(), color);
      return true;
    }

    private static object Faction_SetAsMainAllyFaction(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null && GameScenarioManager.Instance().Scenario != null)
      {
        GameScenarioManager.Instance().Scenario.MainAllyFaction = f;
        return true;
      }
      return false;
    }

    private static object Faction_SetAsMainEnemyFaction(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null && GameScenarioManager.Instance().Scenario != null)
      {
        GameScenarioManager.Instance().Scenario.MainEnemyFaction = f;
        return true;
      }
      return false;
    }

    private static object Faction_MakeAlly(object[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get(ps[0].ToString());
      FactionInfo f2 = FactionInfo.Factory.Get(ps[1].ToString());
      if (f1 != null && f2 != null)
      {
        if (!f1.Allies.Contains(f2))
          f1.Allies.Add(f2);

        if (!f2.Allies.Contains(f1))
          f2.Allies.Add(f1);
        return true;
      }
      return false;
    }

    private static object Faction_MakeEnemy(object[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get(ps[0].ToString());
      FactionInfo f2 = FactionInfo.Factory.Get(ps[1].ToString());
      if (f1 != null && f2 != null)
      {
        if (f1.Allies.Contains(f2))
          f1.Allies.Remove(f2);

        if (f2.Allies.Contains(f1))
          f2.Allies.Remove(f1);
        return true;
      }
      return false;
    }

    private static object Faction_GetWingCount(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.GetWings().Count : 0;
    }

    private static object Faction_GetShipCount(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.GetShips().Count : 0;
    }

    private static object Faction_GetStructureCount(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.GetStructures().Count : 0;
    }

    private static object Faction_GetWingLimit(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.WingLimit : 0;
    }

    private static object Faction_GetShipLimit(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.ShipLimit : 0;
    }

    private static object Faction_GetStructureLimit(object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.StructureLimit : 0;
    }

    #endregion

    #region Game States

    private static object GetGameStateB(object[] ps)
    {
      if (ps.Length == 1)
        return GameScenarioManager.Instance().GetGameStateB(ps[0].ToString());
      else
        return GameScenarioManager.Instance().GetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
    }

    private static object SetGameStateB(object[] ps)
    {
      GameScenarioManager.Instance().SetGameStateB(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
      return true;
    }

    private static object GetGameStateF(object[] ps)
    {
      if (ps.Length == 1)
        return GameScenarioManager.Instance().GetGameStateF(ps[0].ToString());
      else
        return GameScenarioManager.Instance().GetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
    }

    private static object SetGameStateF(object[] ps)
    {
      GameScenarioManager.Instance().SetGameStateF(ps[0].ToString(), Convert.ToSingle(ps[1].ToString()));
      return true;
    }

    private static object GetGameStateS(object[] ps)
    {
      if (ps.Length == 1)
        return GameScenarioManager.Instance().GetGameStateS(ps[0].ToString());
      else
        return GameScenarioManager.Instance().GetGameStateS(ps[0].ToString(), ps[1].ToString());
    }

    private static object SetGameStateS(object[] ps)
    {
      GameScenarioManager.Instance().SetGameStateS(ps[0].ToString(), ps[1].ToString());
      return true;
    }

    private static object GetGameTime(object[] ps)
    {
      return Game.Instance().GameTime;
    }

    private static object GetLastFrameTime(object[] ps)
    {
      return Game.Instance().TimeSinceRender;
    }

    private static object GetDifficulty(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.Difficulty;
    }

    private static object GetPlayerActorType(object[] ps)
    {
      return PlayerInfo.Instance().ActorType.Name;
    }

    private static object GetStageNumber(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.StageNumber;
    }

    private static object SetStageNumber(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return false;

      GameScenarioManager.Instance().Scenario.StageNumber = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    private static object GetRegisterCount(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      Dictionary<string, ActorInfo> reg = GameScenarioManager.Instance().Scenario.GetRegister(ps[0].ToString());
      if (reg == null)
        return 0;
      return reg.Count;
    }

    private static object GetTimeSinceLostWing(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.TimeSinceLostWing;
    }

    private static object GetTimeSinceLostShip(object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null)
        return 0;

      return GameScenarioManager.Instance().Scenario.TimeSinceLostShip;
    }

    private static object AddEvent(object[] ps)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + Convert.ToInt32(ps[0].ToString()), ps[1].ToString());
      return true;
    }

    #endregion

    #region Sound and Music

    private static object SetMusic(object[] ps)
    {
      if (ps.Length == 1)
        return SoundManager.Instance().SetMusic(ps[0].ToString());
      else if (ps.Length == 2)
        return SoundManager.Instance().SetMusic(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()));
      else
        return SoundManager.Instance().SetMusic(ps[0].ToString(), Convert.ToBoolean(ps[1].ToString()), Convert.ToUInt32(ps[2].ToString()));
    }

    private static object SetMusicLoop(object[] ps)
    {
      if (ps.Length == 1)
        SoundManager.Instance().SetMusicLoop(ps[0].ToString());
      else
        SoundManager.Instance().SetMusicLoop(ps[0].ToString(), Convert.ToUInt32(ps[1].ToString()));
      return true;
    }

    private static object SetMusicStop(object[] ps)
    {
      SoundManager.Instance().SetMusicStop();
      return null;
    }

    private static object SetMusicPause(object[] ps)
    {
      SoundManager.Instance().SetMusicPause();
      return null;
    }

    private static object SetMusicResume(object[] ps)
    {
      SoundManager.Instance().SetMusicResume();
      return null;
    }

    #endregion

    #region UI

    private static object SetUILine1Color(object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), 1);
      GameScenarioManager.Instance().Line1Color = color;
      return true;
    }

    private static object SetUILine2Color(object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), 1);
      GameScenarioManager.Instance().Line2Color = color;
      return true;
    }

    private static object SetUILine3Color(object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[0].ToString()), Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), 1);
      GameScenarioManager.Instance().Line3Color = color;
      return true;
    }

    private static object SetUILine1Text(object[] ps)
    {
      GameScenarioManager.Instance().Line1Text = ps[0].ToString();
      return true;
    }

    private static object SetUILine2Text(object[] ps)
    {
      GameScenarioManager.Instance().Line2Text = ps[0].ToString();
      return true;
    }

    private static object SetUILine3Text(object[] ps)
    {
      GameScenarioManager.Instance().Line3Text = ps[0].ToString();
      return true;
    }
    #endregion

    #region Scripts

    private static object CallScript(object[] ps)
    {
      Script scr = ScriptFactory.GetScript(ps[0].ToString());
      if (scr != null)
      {
        scr.Run();
        return true;
      }
      return false;
    }

    #endregion

    #region Misc

    private static object GetArrayElement(object[] ps)
    {
      if (!(ps[0] is Array))
        return null;

      return ((Array)ps[0]).GetValue(Convert.ToInt32(ps[1].ToString()));
    }

    #endregion

    #region Helpers

    private static ActionInfo ParseAction(object[] ps)
    {
      ActorInfo tgt = null;
      ActionInfo action = null;
      switch (ps[0].ToString().ToLower())
      {
        case "idle":
          action = new Idle();
          break;

        case "hunt":
          action = new Hunt();
          break;

        case "selfdestruct":
          action = new SelfDestruct();
          break;

        case "delete":
          action = new Delete();
          break;

        case "lock":
          action = new Lock();
          break;

        case "wait":
          if (ps.Length <= 1)
            action = new Wait();
          else
            action = new Wait(Convert.ToSingle(ps[1].ToString()));
          break;

        case "evade":
          if (ps.Length <= 1)
            action = new Evade();
          else
            action = new Evade(Convert.ToSingle(ps[1].ToString()));
          break;

        case "move":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            float speed = Convert.ToSingle(ps[4].ToString());

            switch (ps.Length)
            {
              case 5:
                action = new Move(dest, speed);
                break;
              case 6:
                action = new Move(dest, speed, Convert.ToSingle(ps[5].ToString()));
                break;
              default:
              case 7:
                action = new Move(dest, speed, Convert.ToSingle(ps[5].ToString()), Convert.ToBoolean(ps[6].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 5, ps.Length));
          break;

        case "forcedmove":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            float speed = Convert.ToSingle(ps[4].ToString());

            switch (ps.Length)
            {
              case 5:
                action = new ForcedMove(dest, speed);
                break;
              case 6:
                action = new ForcedMove(dest, speed, Convert.ToSingle(ps[5].ToString()));
                break;
              default:
              case 7:
                action = new ForcedMove(dest, speed, Convert.ToSingle(ps[5].ToString()), Convert.ToSingle(ps[6].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 5, ps.Length));
          break;

        case "rotate":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            float speed = Convert.ToSingle(ps[4].ToString());

            switch (ps.Length)
            {
              case 5:
                action = new Rotate(dest, speed);
                break;
              case 6:
                action = new Rotate(dest, speed, Convert.ToSingle(ps[5].ToString()));
                break;
              default:
              case 7:
                action = new Rotate(dest, speed, Convert.ToSingle(ps[5].ToString()), Convert.ToBoolean(ps[6].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 5, ps.Length));
          break;

        case "hyperspacein":
          if (ps.Length >= 4)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            action = new HyperspaceIn(dest);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 4, ps.Length));
          break;

        case "hyperspaceout":
          action = new HyperspaceOut();
          break;

        case "attackactor":
          if (ps.Length >= 2)
          {
            tgt = ActorInfo.Factory.GetActor(Convert.ToInt32(ps[1].ToString()));
            if (tgt == null)
              throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[0].ToString().ToLower(), ps[1].ToString().ToLower()));

            switch (ps.Length)
            {
              case 2:
                action = new AttackActor(tgt);
                break;
              case 3:
                action = new AttackActor(tgt, Convert.ToSingle(ps[2].ToString()));
                break;
              case 4:
                action = new AttackActor(tgt, Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
                break;
              case 5:
                action = new AttackActor(tgt, Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToBoolean(ps[4].ToString()));
                break;
              default:
              case 6:
                action = new AttackActor(tgt, Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToBoolean(ps[4].ToString()), Convert.ToSingle(ps[5].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 2, ps.Length));
          break;

        case "followactor":
          if (ps.Length >= 2)
          {
            tgt = ActorInfo.Factory.GetActor(Convert.ToInt32(ps[1].ToString()));
            if (tgt == null)
              throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[0].ToString().ToLower(), ps[1].ToString().ToLower()));

            switch (ps.Length)
            {
              case 2:
                action = new FollowActor(tgt);
                break;
              case 3:
                action = new FollowActor(tgt, Convert.ToSingle(ps[2].ToString()));
                break;
              default:
              case 4:
                action = new FollowActor(tgt, Convert.ToSingle(ps[2].ToString()), Convert.ToBoolean(ps[3].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 2, ps.Length));
          break;

        case "avoidcollisionrotate":
          if (ps.Length >= 7)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            TV_3DVECTOR rot = new TV_3DVECTOR(Convert.ToSingle(ps[4].ToString()), Convert.ToSingle(ps[5].ToString()), Convert.ToSingle(ps[6].ToString()));

            switch (ps.Length)
            {
              case 7:
                action = new AvoidCollisionRotate(pos, rot);
                break;
              default:
              case 8:
                action = new AvoidCollisionRotate(pos, rot, Convert.ToSingle(ps[7].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 7, ps.Length));
          break;

        case "setgamestateb":
          if (ps.Length >= 3)
          {
            action = new SetGameStateB(ps[1].ToString(), Convert.ToBoolean(ps[2].ToString()));
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 3, ps.Length));
          break;
          
      }
      return action;
    }

    private static void ConfigureActorProperty(ActorInfo actor, string key, bool setValue, ref object newValue)
    {
      switch (key)
      {
        //case "ActorState":
        //  if (setValue)
        //    actor.ActorState = (ActorState)Enum.Parse(typeof(ActorState), newValue.ToString());
        //  else
        //    newValue = actor.ActorState;
        //  return;
        case "AllowRegen":
          if (setValue)
            actor.RegenerationInfo.AllowRegen = Convert.ToBoolean(newValue);
          else
            newValue = actor.RegenerationInfo.AllowRegen;
          return;
        case "ApplyZBalance":
          if (setValue)
            actor.MovementInfo.ApplyZBalance = Convert.ToBoolean(newValue);
          else
            newValue = actor.MovementInfo.ApplyZBalance;
          return;
        case "CamDeathCircleHeight":
          if (setValue)
            actor.CamDeathCircleHeight = Convert.ToSingle(newValue);
          else
            newValue = actor.CamDeathCircleHeight;
          return;
        case "CamDeathCirclePeriod":
          if (setValue)
            actor.CamDeathCirclePeriod = Convert.ToSingle(newValue);
          else
            newValue = actor.CamDeathCirclePeriod;
          return;
        case "CamDeathCircleRadius":
          if (setValue)
            actor.CamDeathCircleRadius = Convert.ToSingle(newValue);
          else
            newValue = actor.CamDeathCircleRadius;
          return;
        case "CanEvade":
          if (setValue)
            actor.CanEvade = Convert.ToBoolean(newValue);
          else
            newValue = actor.CanEvade;
          return;
        case "CanRetaliate":
          if (setValue)
            actor.CanRetaliate = Convert.ToBoolean(newValue);
          else
            newValue = actor.CanRetaliate;
          return;
        case "ChildRegenRate":
          if (setValue)
            actor.RegenerationInfo.ChildRegenRate = Convert.ToSingle(newValue);
          else
            newValue = actor.RegenerationInfo.ChildRegenRate;
          return;
        case "DamageModifier":
          if (setValue)
            actor.CombatInfo.DamageModifier = Convert.ToSingle(newValue);
          else
            newValue = actor.CombatInfo.DamageModifier;
          return;
        case "DeathExplosionSize":
          if (setValue)
            actor.ExplosionInfo.DeathExplosionSize = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.DeathExplosionSize;
          return;
        case "DeathExplosionType":
          if (setValue)
            actor.ExplosionInfo.DeathExplosionType = newValue.ToString();
          else
            newValue = actor.ExplosionInfo.DeathExplosionType;
          return;
        case "EnableDeathExplosion":
          if (setValue)
            actor.ExplosionInfo.EnableDeathExplosion = Convert.ToBoolean(newValue);
          else
            newValue = actor.ExplosionInfo.EnableDeathExplosion;
          return;
          /*
        case "EnableExplosions":
          if (setValue)
            actor.ExplosionInfo.EnableExplosions = Convert.ToBoolean(newValue);
          else
            newValue = actor.ExplosionInfo.EnableExplosions;
          return;
          */
        case "ExplosionCooldown":
          if (setValue)
            actor.ExplosionInfo.ExplosionCooldown = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.ExplosionCooldown;
          return;
        case "ExplosionRate":
          if (setValue)
            actor.ExplosionInfo.ExplosionRate = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.ExplosionRate;
          return;
        case "ExplosionSize":
          if (setValue)
            actor.ExplosionInfo.ExplosionSize = Convert.ToSingle(newValue);
          else
            newValue = actor.ExplosionInfo.ExplosionSize;
          return;
        case "ExplosionType":
          if (setValue)
            actor.ExplosionInfo.ExplosionType = newValue.ToString();
          else
            newValue = actor.ExplosionInfo.ExplosionType;
          return;
        case "HuntWeight":
          if (setValue)
            actor.HuntWeight = Convert.ToInt32(newValue);
          else
            newValue = actor.HuntWeight;
          return;
        case "IsCombatObject":
          if (setValue)
            actor.CombatInfo.IsCombatObject = Convert.ToBoolean(newValue);
          else
            newValue = actor.CombatInfo.IsCombatObject;
          return;
        case "MaxSecondOrderTurnRateFrac":
          if (setValue)
            actor.MovementInfo.MaxSecondOrderTurnRateFrac = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxSecondOrderTurnRateFrac;
          return;
        case "MaxSpeed":
          if (setValue)
            actor.MovementInfo.MaxSpeed = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxSpeed;
          return;
        case "MaxSpeedChangeRate":
          if (setValue)
            actor.MovementInfo.MaxSpeedChangeRate = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxSpeedChangeRate;
          return;
        case "MaxStrength":
          if (setValue)
            actor.CombatInfo.MaxStrength = Convert.ToSingle(newValue);
          else
            newValue = actor.CombatInfo.MaxStrength;
          return;
        case "MaxTurnRate":
          if (setValue)
            actor.MovementInfo.MaxTurnRate = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MaxTurnRate;
          return;
        case "MinSpeed":
          if (setValue)
            actor.MovementInfo.MinSpeed = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.MinSpeed;
          return;
        case "SideBarName":
          if (setValue)
            actor.SideBarName = newValue.ToString();
          else
            newValue = actor.SideBarName;
          return;
        case "ZNormFrac":
          if (setValue)
            actor.MovementInfo.ZNormFrac = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.ZNormFrac;
          return;
        case "ZTilt":
          if (setValue)
            actor.MovementInfo.ZTilt = Convert.ToSingle(newValue);
          else
            newValue = actor.MovementInfo.ZTilt;
          return;
      }
    }

    #endregion
  }
}
