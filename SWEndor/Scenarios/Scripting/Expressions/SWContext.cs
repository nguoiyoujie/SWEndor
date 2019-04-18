using SWEndor.Scenarios.Scripting.Functions;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public class SWContext : Context
  {
    public static SWContext Instance = new SWContext();

    private SWContext()
    {
      AddFunctions();
    }

    public void Reset()
    {
      Variables.Clear();
    }

    public void AddFunctions()
    { 
      // Scene Management
      Functions.Add("SetMaxBounds".ToLowerInvariant(), SceneManagement.SetMaxBounds);
      Functions.Add("SetMinBounds".ToLowerInvariant(), SceneManagement.SetMinBounds);
      Functions.Add("SetMaxAIBounds".ToLowerInvariant(), SceneManagement.SetMaxAIBounds);
      Functions.Add("SetMinAIBounds".ToLowerInvariant(), SceneManagement.SetMinAIBounds);
      Functions.Add("FadeOut".ToLowerInvariant(), SceneManagement.FadeOut);

      // Scene Camera Management
      Functions.Add("Actor_SetSceneCameraAsActive".ToLowerInvariant(), SceneCameraManagement.Actor_SetSceneCameraAsActive);

      // Actor Management
      Functions.Add("Actor_Spawn".ToLowerInvariant(), ActorManagement.Actor_Spawn);
      Functions.Add("Actor_SetActive".ToLowerInvariant(), ActorManagement.Actor_SetActive);
      Functions.Add("Actor_IsAlive".ToLowerInvariant(), ActorManagement.Actor_IsAlive);
      Functions.Add("Actor_RegisterEvents".ToLowerInvariant(), ActorManagement.Actor_RegisterEvents);
      Functions.Add("Actor_GetLocalPosition".ToLowerInvariant(), ActorManagement.Actor_GetLocalPosition);
      Functions.Add("Actor_SetLocalPosition".ToLowerInvariant(), ActorManagement.Actor_SetLocalPosition);
      Functions.Add("Actor_GetLocalRotation".ToLowerInvariant(), ActorManagement.Actor_GetLocalRotation);
      Functions.Add("Actor_SetLocalRotation".ToLowerInvariant(), ActorManagement.Actor_SetLocalRotation);
      Functions.Add("Actor_GetLocalDirection".ToLowerInvariant(), ActorManagement.Actor_GetLocalDirection);
      Functions.Add("Actor_SetLocalDirection".ToLowerInvariant(), ActorManagement.Actor_SetLocalDirection);
      Functions.Add("Actor_GetPosition".ToLowerInvariant(), ActorManagement.Actor_GetPosition);
      Functions.Add("Actor_GetRotation".ToLowerInvariant(), ActorManagement.Actor_GetRotation);
      Functions.Add("Actor_SetRotation".ToLowerInvariant(), ActorManagement.Actor_SetRotation);
      Functions.Add("Actor_GetDirection".ToLowerInvariant(), ActorManagement.Actor_GetDirection);
      Functions.Add("Actor_SetDirection".ToLowerInvariant(), ActorManagement.Actor_SetDirection);
      Functions.Add("Actor_LookAtPoint".ToLowerInvariant(), ActorManagement.Actor_LookAtPoint);
      Functions.Add("Actor_GetProperty".ToLowerInvariant(), ActorManagement.Actor_GetProperty);
      Functions.Add("Actor_SetProperty".ToLowerInvariant(), ActorManagement.Actor_SetProperty);

      // Message Box
      Functions.Add("Message".ToLowerInvariant(), Messaging.MessageText);

      // Action Management
      Functions.Add("Actor_QueueFirst".ToLowerInvariant(), AIManagement.Actor_QueueFirst);
      Functions.Add("Actor_QueueNext".ToLowerInvariant(), AIManagement.Actor_QueueNext);
      Functions.Add("Actor_QueueLast".ToLowerInvariant(), AIManagement.Actor_QueueLast);
      Functions.Add("Actor_UnlockActor".ToLowerInvariant(), AIManagement.Actor_UnlockActor);
      Functions.Add("Actor_ClearQueue".ToLowerInvariant(), AIManagement.Actor_ClearQueue);
      Functions.Add("Actor_ForceClearQueue".ToLowerInvariant(), AIManagement.Actor_ForceClearQueue);

      // Game states
      Functions.Add("GetGameTime".ToLowerInvariant(), GameStateManagement.GetGameTime);
      Functions.Add("GetLastFrameTime".ToLowerInvariant(), GameStateManagement.GetLastFrameTime);
      Functions.Add("GetDifficulty".ToLowerInvariant(), GameStateManagement.GetDifficulty);
      Functions.Add("GetPlayerActorType".ToLowerInvariant(), GameStateManagement.GetPlayerActorType);
      Functions.Add("GetStageNumber".ToLowerInvariant(), GameStateManagement.GetStageNumber);
      Functions.Add("SetStageNumber".ToLowerInvariant(), GameStateManagement.SetStageNumber);
      Functions.Add("GetGameStateB".ToLowerInvariant(), GameStateManagement.GetGameStateB);
      Functions.Add("SetGameStateB".ToLowerInvariant(), GameStateManagement.SetGameStateB);
      Functions.Add("GetGameStateF".ToLowerInvariant(), GameStateManagement.GetGameStateF);
      Functions.Add("SetGameStateF".ToLowerInvariant(), GameStateManagement.SetGameStateF);
      Functions.Add("GetGameStateS".ToLowerInvariant(), GameStateManagement.GetGameStateS);
      Functions.Add("SetGameStateS".ToLowerInvariant(), GameStateManagement.SetGameStateS);
      Functions.Add("GetRegisterCount".ToLowerInvariant(), GameStateManagement.GetRegisterCount);
      Functions.Add("GetTimeSinceLostWing".ToLowerInvariant(), GameStateManagement.GetTimeSinceLostWing);
      Functions.Add("GetTimeSinceLostShip".ToLowerInvariant(), GameStateManagement.GetTimeSinceLostShip);
      Functions.Add("AddEvent".ToLowerInvariant(), GameStateManagement.AddEvent);

      // Player Management
      Functions.Add("Player_AssignActor".ToLowerInvariant(), PlayerManagement.Player_AssignActor);
      Functions.Add("Player_GetActor".ToLowerInvariant(), PlayerManagement.Player_GetActor);
      Functions.Add("Player_RequestSpawn".ToLowerInvariant(), PlayerManagement.Player_RequestSpawn);
      Functions.Add("Player_SetMovementEnabled".ToLowerInvariant(), PlayerManagement.Player_SetMovementEnabled);
      Functions.Add("Player_SetAI".ToLowerInvariant(), PlayerManagement.Player_SetAI);
      Functions.Add("Player_SetLives".ToLowerInvariant(), PlayerManagement.Player_SetLives);
      Functions.Add("Player_SetScorePerLife".ToLowerInvariant(), PlayerManagement.Player_SetScorePerLife);
      Functions.Add("Player_SetScoreForNextLife".ToLowerInvariant(), PlayerManagement.Player_SetScoreForNextLife);
      Functions.Add("Player_ResetScore".ToLowerInvariant(), PlayerManagement.Player_ResetScore);

      // Faction
      Functions.Add("AddFaction".ToLowerInvariant(), FactionManagement.AddFaction);
      Functions.Add("Faction_SetAsMainAllyFaction".ToLowerInvariant(), FactionManagement.Faction_SetAsMainAllyFaction);
      Functions.Add("Faction_SetAsMainEnemyFaction".ToLowerInvariant(), FactionManagement.Faction_SetAsMainEnemyFaction);
      Functions.Add("Faction_MakeAlly".ToLowerInvariant(), FactionManagement.Faction_MakeAlly);
      Functions.Add("Faction_MakeEnemy".ToLowerInvariant(), FactionManagement.Faction_MakeEnemy);
      Functions.Add("Faction_GetWingCount".ToLowerInvariant(), FactionManagement.Faction_GetWingCount);
      Functions.Add("Faction_GetShipCount".ToLowerInvariant(), FactionManagement.Faction_GetShipCount);
      Functions.Add("Faction_GetStructureCount".ToLowerInvariant(), FactionManagement.Faction_GetStructureCount);
      Functions.Add("Faction_GetWingLimit".ToLowerInvariant(), FactionManagement.Faction_GetWingLimit);
      Functions.Add("Faction_GetShipLimit".ToLowerInvariant(), FactionManagement.Faction_GetShipLimit);
      Functions.Add("Faction_GetStructureLimit".ToLowerInvariant(), FactionManagement.Faction_GetStructureLimit);
      Functions.Add("Faction_SetWingLimit".ToLowerInvariant(), FactionManagement.Faction_SetWingLimit);
      Functions.Add("Faction_SetShipLimit".ToLowerInvariant(), FactionManagement.Faction_SetShipLimit);
      Functions.Add("Faction_SetStructureLimit".ToLowerInvariant(), FactionManagement.Faction_SetStructureLimit);
      Functions.Add("Faction_GetWingSpawnLimit".ToLowerInvariant(), FactionManagement.Faction_GetWingSpawnLimit);
      Functions.Add("Faction_GetShipSpawnLimit".ToLowerInvariant(), FactionManagement.Faction_GetShipSpawnLimit);
      Functions.Add("Faction_GetStructureSpawnLimit".ToLowerInvariant(), FactionManagement.Faction_GetStructureSpawnLimit);
      Functions.Add("Faction_SetWingSpawnLimit".ToLowerInvariant(), FactionManagement.Faction_SetWingSpawnLimit);
      Functions.Add("Faction_SetShipSpawnLimit".ToLowerInvariant(), FactionManagement.Faction_SetShipSpawnLimit);
      Functions.Add("Faction_SetStructureSpawnLimit".ToLowerInvariant(), FactionManagement.Faction_SetStructureSpawnLimit);

      // Sounds and Music
      Functions.Add("SetMusic".ToLowerInvariant(), AudioManagement.SetMusic);
      Functions.Add("SetMusicLoop".ToLowerInvariant(), AudioManagement.SetMusicLoop);
      Functions.Add("SetMusicPause".ToLowerInvariant(), AudioManagement.SetMusicPause);
      Functions.Add("SetMusicResume".ToLowerInvariant(), AudioManagement.SetMusicResume);
      Functions.Add("SetMusicStop".ToLowerInvariant(), AudioManagement.SetMusicStop);

      // UI
      Functions.Add("SetUILine1Color".ToLowerInvariant(), UIManagement.SetUILine1Color);
      Functions.Add("SetUILine2Color".ToLowerInvariant(), UIManagement.SetUILine2Color);
      Functions.Add("SetUILine3Color".ToLowerInvariant(), UIManagement.SetUILine3Color);
      Functions.Add("SetUILine1Text".ToLowerInvariant(), UIManagement.SetUILine1Text);
      Functions.Add("SetUILine2Text".ToLowerInvariant(), UIManagement.SetUILine2Text);
      Functions.Add("SetUILine3Text".ToLowerInvariant(), UIManagement.SetUILine3Text);

      // Script flow
      Functions.Add("CallScript".ToLowerInvariant(), ScriptManagement.CallScript);

      // Misc
      Functions.Add("IsNull".ToLowerInvariant(), delegate (Context c, object[] ps) { return ps[0] == null; });
      Functions.Add("GetArrayElement".ToLowerInvariant(), ScriptManagement.GetArrayElement);


    }
  }
}
