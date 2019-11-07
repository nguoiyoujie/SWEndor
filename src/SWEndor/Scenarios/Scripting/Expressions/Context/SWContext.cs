using SWEndor.Core;
using SWEndor.Scenarios.Scripting.Functions;

namespace SWEndor.Scenarios.Scripting.Expressions
{
  public class SWContext : Context
  {
    internal SWContext(Engine engine) : base(engine)
    {
      AddFunctions();
    }

    public void AddFunctions()
    {
      // Scene Management
      Functions.Add("Scene.SetMaxBounds", SceneFns.SetMaxBounds);
      Functions.Add("Scene.SetMinBounds", SceneFns.SetMinBounds);
      Functions.Add("Scene.SetMaxAIBounds", SceneFns.SetMaxAIBounds);
      Functions.Add("Scene.SetMinAIBounds", SceneFns.SetMinAIBounds);
      Functions.Add("Scene.FadeOut", SceneFns.FadeOut);

      // Scene Camera Management
      //Functions.Add("Scene.SetSceneCameraAsActive", SceneCameraManagement.SetSceneCameraAsActive);

      // Actor Management
      Functions.Add("Actor.Squadron_Spawn", ActorFns.Squadron_Spawn);
      Functions.Add("Actor.AddToSquad", ActorFns.AddToSquad);
      Functions.Add("Actor.RemoveFromSquad", ActorFns.RemoveFromSquad);
      Functions.Add("Actor.MakeSquadLeader", ActorFns.MakeSquadLeader);

      Functions.Add("Actor.Spawn", ActorFns.Spawn);
      //Functions.Add("Actor.SetActive", ActorManagement.SetActive);
      Functions.Add("Actor.GetActorType", ActorFns.GetActorType);
      Functions.Add("Actor.IsFighter", ActorFns.IsFighter);
      Functions.Add("Actor.IsLargeShip", ActorFns.IsLargeShip);
      Functions.Add("Actor.IsAlive", ActorFns.IsAlive);
      Functions.Add("Actor.RegisterEvents", ActorFns.RegisterEvents);
      Functions.Add("Actor.GetLocalPosition", ActorFns.GetLocalPosition);
      //Functions.Add("Actor.GetGlobalPosition", ActorManagement.GetGlobalPosition);
      Functions.Add("Actor.GetLocalRotation", ActorFns.GetLocalRotation);
      //Functions.Add("Actor.GetGlobalRotation = new TV_3DVECTOR", ActorManagement.GetGlobalRotation = new TV_3DVECTOR);
      Functions.Add("Actor.GetLocalDirection", ActorFns.GetLocalDirection);
      //Functions.Add("Actor.GetGlobalDirection = new TV_3DVECTOR", ActorManagement.GetGlobalDirection = new TV_3DVECTOR);
      Functions.Add("Actor.GetPosition", ActorFns.GetPosition);
      Functions.Add("Actor.GetRotation", ActorFns.GetRotation);
      //Functions.Add("Actor.SetRotation", ActorManagement.SetRotation);
      Functions.Add("Actor.GetDirection", ActorFns.GetDirection);
      //Functions.Add("Actor.SetDirection", ActorManagement.SetDirection);
      Functions.Add("Actor.LookAtPoint", ActorFns.LookAtPoint);
      Functions.Add("Actor.GetChildren", ActorFns.GetChildren);
      Functions.Add("Actor.GetProperty", ActorFns.GetProperty);
      Functions.Add("Actor.SetProperty", ActorFns.SetProperty);

      // Message Box
      Functions.Add("Message", MessagingFns.MessageText);

      // Action Management
      Functions.Add("AI.QueueFirst", AIFns.QueueFirst);
      Functions.Add("AI.QueueNext", AIFns.QueueNext);
      Functions.Add("AI.QueueLast", AIFns.QueueLast);
      Functions.Add("AI.UnlockActor", AIFns.UnlockActor);
      Functions.Add("AI.ClearQueue", AIFns.ClearQueue);
      Functions.Add("AI.ForceClearQueue", AIFns.ForceClearQueue);

      // Game states
      Functions.Add("GetGameTime", GameFns.GetGameTime);
      Functions.Add("GetLastFrameTime", GameFns.GetLastFrameTime);
      Functions.Add("GetDifficulty", GameFns.GetDifficulty);
      Functions.Add("GetPlayerActorType", GameFns.GetPlayerActorType);
      Functions.Add("GetStageNumber", GameFns.GetStageNumber);
      Functions.Add("SetStageNumber", GameFns.SetStageNumber);
      Functions.Add("GetGameStateB", GameFns.GetGameStateB);
      Functions.Add("SetGameStateB", GameFns.SetGameStateB);
      Functions.Add("GetGameStateF", GameFns.GetGameStateF);
      Functions.Add("SetGameStateF", GameFns.SetGameStateF);
      Functions.Add("GetGameStateS", GameFns.GetGameStateS);
      Functions.Add("SetGameStateS", GameFns.SetGameStateS);
      Functions.Add("GetRegisterCount", GameFns.GetRegisterCount);
      Functions.Add("GetTimeSinceLostWing", GameFns.GetTimeSinceLostWing);
      Functions.Add("GetTimeSinceLostShip", GameFns.GetTimeSinceLostShip);
      Functions.Add("AddEvent", GameFns.AddEvent);

      // Player Management
      Functions.Add("Player.AssignActor", PlayerFns.AssignActor);
      Functions.Add("Player.GetActor", PlayerFns.GetActor);
      Functions.Add("Player.RequestSpawn", PlayerFns.RequestSpawn);
      Functions.Add("Player.SetMovementEnabled", PlayerFns.SetMovementEnabled);
      Functions.Add("Player.SetAI", PlayerFns.SetAI);
      Functions.Add("Player.SetLives", PlayerFns.SetLives);
      Functions.Add("Player.DecreaseLives", PlayerFns.DecreaseLives);

      Functions.Add("Player.SetScorePerLife", ScoreFns.SetScorePerLife);
      Functions.Add("Player.SetScoreForNextLife", ScoreFns.SetScoreForNextLife);
      Functions.Add("Player.ResetScore", ScoreFns.ResetScore);

      // Faction
      Functions.Add("Faction.Add", FactionFns.AddFaction);
      //Functions.Add("Faction.SetAsMainAllyFaction", FactionManagement.SetAsMainAllyFaction);
      //Functions.Add("Faction.SetAsMainEnemyFaction", FactionManagement.SetAsMainEnemyFaction);
      Functions.Add("Faction.MakeAlly", FactionFns.MakeAlly);
      Functions.Add("Faction.MakeEnemy", FactionFns.MakeEnemy);
      Functions.Add("Faction.GetWingCount", FactionFns.GetWingCount);
      Functions.Add("Faction.GetShipCount", FactionFns.GetShipCount);
      Functions.Add("Faction.GetStructureCount", FactionFns.GetStructureCount);
      Functions.Add("Faction.GetWingLimit", FactionFns.GetWingLimit);
      Functions.Add("Faction.GetShipLimit", FactionFns.GetShipLimit);
      Functions.Add("Faction.GetStructureLimit", FactionFns.GetStructureLimit);
      Functions.Add("Faction.SetWingLimit", FactionFns.SetWingLimit);
      Functions.Add("Faction.SetShipLimit", FactionFns.SetShipLimit);
      Functions.Add("Faction.SetStructureLimit", FactionFns.SetStructureLimit);
      Functions.Add("Faction.GetWingSpawnLimit", FactionFns.GetWingSpawnLimit);
      Functions.Add("Faction.GetShipSpawnLimit", FactionFns.GetShipSpawnLimit);
      Functions.Add("Faction.GetStructureSpawnLimit", FactionFns.GetStructureSpawnLimit);
      Functions.Add("Faction.SetWingSpawnLimit", FactionFns.SetWingSpawnLimit);
      Functions.Add("Faction.SetShipSpawnLimit", FactionFns.SetShipSpawnLimit);
      Functions.Add("Faction.SetStructureSpawnLimit", FactionFns.SetStructureSpawnLimit);

      // Sounds and Music
      Functions.Add("Audio.SetMood", SceneFns.SetMood);
      Functions.Add("Audio.SetMusic", AudioFns.SetMusic);
      Functions.Add("Audio.SetMusicDyn", AudioFns.SetMusicDyn);
      Functions.Add("Audio.SetMusicLoop", AudioFns.SetMusicLoop);
      Functions.Add("Audio.SetMusicPause", AudioFns.SetMusicPause);
      Functions.Add("Audio.SetMusicResume", AudioFns.SetMusicResume);
      Functions.Add("Audio.SetMusicStop", AudioFns.SetMusicStop);

      // UI
      Functions.Add("UI.SetLine1Color", UIFns.SetUILine1Color);
      Functions.Add("UI.SetLine2Color", UIFns.SetUILine2Color);
      Functions.Add("UI.SetLine3Color", UIFns.SetUILine3Color);
      Functions.Add("UI.SetLine1Text", UIFns.SetUILine1Text);
      Functions.Add("UI.SetLine2Text", UIFns.SetUILine2Text);
      Functions.Add("UI.SetLine3Text", UIFns.SetUILine3Text);

      // Script flow
      Functions.Add("Script.Call", ScriptingFns.CallScript);
      Functions.Add("Script.CallX", ScriptingFns.CallScriptX);

      // Misc
      Functions.Add("IsNull", MiscFns.IsNull);
      Functions.Add("GetArrayElement", MiscFns.GetArrayElement);
    }
  }
}
