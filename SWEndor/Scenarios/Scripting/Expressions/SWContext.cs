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
      Functions.Add("SetMood".ToLowerInvariant(), SceneManagement.SetMood);
      Functions.Add("SetMaxBounds".ToLowerInvariant(), SceneManagement.SetMaxBounds);
      Functions.Add("SetMinBounds".ToLowerInvariant(), SceneManagement.SetMinBounds);
      Functions.Add("SetMaxAIBounds".ToLowerInvariant(), SceneManagement.SetMaxAIBounds);
      Functions.Add("SetMinAIBounds".ToLowerInvariant(), SceneManagement.SetMinAIBounds);
      Functions.Add("FadeOut".ToLowerInvariant(), SceneManagement.FadeOut);

      // Scene Camera Management
      Functions.Add("Scene.SetSceneCameraAsActive".ToLowerInvariant(), SceneCameraManagement.SetSceneCameraAsActive);

      // Actor Management
      Functions.Add("Actor.Squadron_Spawn".ToLowerInvariant(), ActorManagement.Squadron_Spawn);
      Functions.Add("Actor.AddToSquad".ToLowerInvariant(), ActorManagement.AddToSquad);
      Functions.Add("Actor.RemoveFromSquad".ToLowerInvariant(), ActorManagement.RemoveFromSquad);
      Functions.Add("Actor.MakeSquadLeader".ToLowerInvariant(), ActorManagement.MakeSquadLeader);

      Functions.Add("Actor.Spawn".ToLowerInvariant(), ActorManagement.Spawn);
      //Functions.Add("Actor.SetActive".ToLowerInvariant(), ActorManagement.SetActive);
      Functions.Add("Actor.GetActorType".ToLowerInvariant(), ActorManagement.GetActorType);
      Functions.Add("Actor.IsFighter".ToLowerInvariant(), ActorManagement.IsFighter);
      Functions.Add("Actor.IsLargeShip".ToLowerInvariant(), ActorManagement.IsLargeShip);
      Functions.Add("Actor.IsAlive".ToLowerInvariant(), ActorManagement.IsAlive);
      Functions.Add("Actor.RegisterEvents".ToLowerInvariant(), ActorManagement.RegisterEvents);
      Functions.Add("Actor.GetLocalPosition".ToLowerInvariant(), ActorManagement.GetLocalPosition);
      //Functions.Add("Actor.GetGlobalPosition".ToLowerInvariant(), ActorManagement.GetGlobalPosition);
      Functions.Add("Actor.GetLocalRotation".ToLowerInvariant(), ActorManagement.GetLocalRotation);
      //Functions.Add("Actor.GetGlobalRotation = new TV_3DVECTOR".ToLowerInvariant(), ActorManagement.GetGlobalRotation = new TV_3DVECTOR);
      Functions.Add("Actor.GetLocalDirection".ToLowerInvariant(), ActorManagement.GetLocalDirection);
      //Functions.Add("Actor.GetGlobalDirection = new TV_3DVECTOR".ToLowerInvariant(), ActorManagement.GetGlobalDirection = new TV_3DVECTOR);
      Functions.Add("Actor.GetPosition".ToLowerInvariant(), ActorManagement.GetPosition);
      Functions.Add("Actor.GetRotation".ToLowerInvariant(), ActorManagement.GetRotation);
      //Functions.Add("Actor.SetRotation".ToLowerInvariant(), ActorManagement.SetRotation);
      Functions.Add("Actor.GetDirection".ToLowerInvariant(), ActorManagement.GetDirection);
      //Functions.Add("Actor.SetDirection".ToLowerInvariant(), ActorManagement.SetDirection);
      Functions.Add("Actor.LookAtPoint".ToLowerInvariant(), ActorManagement.LookAtPoint);
      Functions.Add("Actor.GetChildren".ToLowerInvariant(), ActorManagement.GetChildren);
      Functions.Add("Actor.GetProperty".ToLowerInvariant(), ActorManagement.GetProperty);
      Functions.Add("Actor.SetProperty".ToLowerInvariant(), ActorManagement.SetProperty);

      // Message Box
      Functions.Add("Message".ToLowerInvariant(), Messaging.MessageText);

      // Action Management
      Functions.Add("Actor.QueueFirst".ToLowerInvariant(), AIManagement.QueueFirst);
      Functions.Add("Actor.QueueNext".ToLowerInvariant(), AIManagement.QueueNext);
      Functions.Add("Actor.QueueLast".ToLowerInvariant(), AIManagement.QueueLast);
      Functions.Add("Actor.UnlockActor".ToLowerInvariant(), AIManagement.UnlockActor);
      Functions.Add("Actor.ClearQueue".ToLowerInvariant(), AIManagement.ClearQueue);
      Functions.Add("Actor.ForceClearQueue".ToLowerInvariant(), AIManagement.ForceClearQueue);

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
      Functions.Add("Player.AssignActor".ToLowerInvariant(), PlayerManagement.AssignActor);
      Functions.Add("Player.GetActor".ToLowerInvariant(), PlayerManagement.GetActor);
      Functions.Add("Player.RequestSpawn".ToLowerInvariant(), PlayerManagement.RequestSpawn);
      Functions.Add("Player.SetMovementEnabled".ToLowerInvariant(), PlayerManagement.SetMovementEnabled);
      Functions.Add("Player.SetAI".ToLowerInvariant(), PlayerManagement.SetAI);
      Functions.Add("Player.SetLives".ToLowerInvariant(), PlayerManagement.SetLives);
      Functions.Add("Player.DecreaseLives".ToLowerInvariant(), PlayerManagement.DecreaseLives);
      Functions.Add("Player.SetScorePerLife".ToLowerInvariant(), PlayerManagement.SetScorePerLife);
      Functions.Add("Player.SetScoreForNextLife".ToLowerInvariant(), PlayerManagement.SetScoreForNextLife);
      Functions.Add("Player.ResetScore".ToLowerInvariant(), PlayerManagement.ResetScore);

      // Faction
      Functions.Add("AddFaction".ToLowerInvariant(), FactionManagement.AddFaction);
      Functions.Add("Faction.SetAsMainAllyFaction".ToLowerInvariant(), FactionManagement.SetAsMainAllyFaction);
      Functions.Add("Faction.SetAsMainEnemyFaction".ToLowerInvariant(), FactionManagement.SetAsMainEnemyFaction);
      Functions.Add("Faction.MakeAlly".ToLowerInvariant(), FactionManagement.MakeAlly);
      Functions.Add("Faction.MakeEnemy".ToLowerInvariant(), FactionManagement.MakeEnemy);
      Functions.Add("Faction.GetWingCount".ToLowerInvariant(), FactionManagement.GetWingCount);
      Functions.Add("Faction.GetShipCount".ToLowerInvariant(), FactionManagement.GetShipCount);
      Functions.Add("Faction.GetStructureCount".ToLowerInvariant(), FactionManagement.GetStructureCount);
      Functions.Add("Faction.GetWingLimit".ToLowerInvariant(), FactionManagement.GetWingLimit);
      Functions.Add("Faction.GetShipLimit".ToLowerInvariant(), FactionManagement.GetShipLimit);
      Functions.Add("Faction.GetStructureLimit".ToLowerInvariant(), FactionManagement.GetStructureLimit);
      Functions.Add("Faction.SetWingLimit".ToLowerInvariant(), FactionManagement.SetWingLimit);
      Functions.Add("Faction.SetShipLimit".ToLowerInvariant(), FactionManagement.SetShipLimit);
      Functions.Add("Faction.SetStructureLimit".ToLowerInvariant(), FactionManagement.SetStructureLimit);
      Functions.Add("Faction.GetWingSpawnLimit".ToLowerInvariant(), FactionManagement.GetWingSpawnLimit);
      Functions.Add("Faction.GetShipSpawnLimit".ToLowerInvariant(), FactionManagement.GetShipSpawnLimit);
      Functions.Add("Faction.GetStructureSpawnLimit".ToLowerInvariant(), FactionManagement.GetStructureSpawnLimit);
      Functions.Add("Faction.SetWingSpawnLimit".ToLowerInvariant(), FactionManagement.SetWingSpawnLimit);
      Functions.Add("Faction.SetShipSpawnLimit".ToLowerInvariant(), FactionManagement.SetShipSpawnLimit);
      Functions.Add("Faction.SetStructureSpawnLimit".ToLowerInvariant(), FactionManagement.SetStructureSpawnLimit);

      // Sounds and Music
      Functions.Add("SetMusic".ToLowerInvariant(), AudioManagement.SetMusic);
      Functions.Add("SetMusicDyn".ToLowerInvariant(), AudioManagement.SetMusicDyn);
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
      Functions.Add("IsNull".ToLowerInvariant(), IsNull);
      Functions.Add("GetArrayElement".ToLowerInvariant(), ScriptManagement.GetArrayElement);
    }

    public Val IsNull(Context c, Val[] ps)
    {
      return new Val(ps[0].IsNull);
    }
  }
}
