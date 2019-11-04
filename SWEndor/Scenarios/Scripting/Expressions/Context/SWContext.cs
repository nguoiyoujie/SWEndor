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
      Functions.Add("SetMood".ToLowerInvariant(), Scene.SetMood);
      Functions.Add("SetMaxBounds".ToLowerInvariant(), Scene.SetMaxBounds);
      Functions.Add("SetMinBounds".ToLowerInvariant(), Scene.SetMinBounds);
      Functions.Add("SetMaxAIBounds".ToLowerInvariant(), Scene.SetMaxAIBounds);
      Functions.Add("SetMinAIBounds".ToLowerInvariant(), Scene.SetMinAIBounds);
      Functions.Add("FadeOut".ToLowerInvariant(), Scene.FadeOut);

      // Scene Camera Management
      //Functions.Add("Scene.SetSceneCameraAsActive".ToLowerInvariant(), SceneCameraManagement.SetSceneCameraAsActive);

      // Actor Management
      Functions.Add("Actor.Squadron_Spawn".ToLowerInvariant(), Actor.Squadron_Spawn);
      Functions.Add("Actor.AddToSquad".ToLowerInvariant(), Actor.AddToSquad);
      Functions.Add("Actor.RemoveFromSquad".ToLowerInvariant(), Actor.RemoveFromSquad);
      Functions.Add("Actor.MakeSquadLeader".ToLowerInvariant(), Actor.MakeSquadLeader);

      Functions.Add("Actor.Spawn".ToLowerInvariant(), Actor.Spawn);
      //Functions.Add("Actor.SetActive".ToLowerInvariant(), ActorManagement.SetActive);
      Functions.Add("Actor.GetActorType".ToLowerInvariant(), Actor.GetActorType);
      Functions.Add("Actor.IsFighter".ToLowerInvariant(), Actor.IsFighter);
      Functions.Add("Actor.IsLargeShip".ToLowerInvariant(), Actor.IsLargeShip);
      Functions.Add("Actor.IsAlive".ToLowerInvariant(), Actor.IsAlive);
      Functions.Add("Actor.RegisterEvents".ToLowerInvariant(), Actor.RegisterEvents);
      Functions.Add("Actor.GetLocalPosition".ToLowerInvariant(), Actor.GetLocalPosition);
      //Functions.Add("Actor.GetGlobalPosition".ToLowerInvariant(), ActorManagement.GetGlobalPosition);
      Functions.Add("Actor.GetLocalRotation".ToLowerInvariant(), Actor.GetLocalRotation);
      //Functions.Add("Actor.GetGlobalRotation = new TV_3DVECTOR".ToLowerInvariant(), ActorManagement.GetGlobalRotation = new TV_3DVECTOR);
      Functions.Add("Actor.GetLocalDirection".ToLowerInvariant(), Actor.GetLocalDirection);
      //Functions.Add("Actor.GetGlobalDirection = new TV_3DVECTOR".ToLowerInvariant(), ActorManagement.GetGlobalDirection = new TV_3DVECTOR);
      Functions.Add("Actor.GetPosition".ToLowerInvariant(), Actor.GetPosition);
      Functions.Add("Actor.GetRotation".ToLowerInvariant(), Actor.GetRotation);
      //Functions.Add("Actor.SetRotation".ToLowerInvariant(), ActorManagement.SetRotation);
      Functions.Add("Actor.GetDirection".ToLowerInvariant(), Actor.GetDirection);
      //Functions.Add("Actor.SetDirection".ToLowerInvariant(), ActorManagement.SetDirection);
      Functions.Add("Actor.LookAtPoint".ToLowerInvariant(), Actor.LookAtPoint);
      Functions.Add("Actor.GetChildren".ToLowerInvariant(), Actor.GetChildren);
      Functions.Add("Actor.GetProperty".ToLowerInvariant(), Actor.GetProperty);
      Functions.Add("Actor.SetProperty".ToLowerInvariant(), Actor.SetProperty);

      // Message Box
      Functions.Add("Message".ToLowerInvariant(), Messaging.MessageText);

      // Action Management
      Functions.Add("Actor.QueueFirst".ToLowerInvariant(), Scripting.Functions.AI.QueueFirst);
      Functions.Add("Actor.QueueNext".ToLowerInvariant(), Scripting.Functions.AI.QueueNext);
      Functions.Add("Actor.QueueLast".ToLowerInvariant(), Scripting.Functions.AI.QueueLast);
      Functions.Add("Actor.UnlockActor".ToLowerInvariant(), Scripting.Functions.AI.UnlockActor);
      Functions.Add("Actor.ClearQueue".ToLowerInvariant(), Scripting.Functions.AI.ClearQueue);
      Functions.Add("Actor.ForceClearQueue".ToLowerInvariant(), Scripting.Functions.AI.ForceClearQueue);

      // Game states
      Functions.Add("GetGameTime".ToLowerInvariant(), Game.GetGameTime);
      Functions.Add("GetLastFrameTime".ToLowerInvariant(), Game.GetLastFrameTime);
      Functions.Add("GetDifficulty".ToLowerInvariant(), Game.GetDifficulty);
      Functions.Add("GetPlayerActorType".ToLowerInvariant(), Game.GetPlayerActorType);
      Functions.Add("GetStageNumber".ToLowerInvariant(), Game.GetStageNumber);
      Functions.Add("SetStageNumber".ToLowerInvariant(), Game.SetStageNumber);
      Functions.Add("GetGameStateB".ToLowerInvariant(), Game.GetGameStateB);
      Functions.Add("SetGameStateB".ToLowerInvariant(), Game.SetGameStateB);
      Functions.Add("GetGameStateF".ToLowerInvariant(), Game.GetGameStateF);
      Functions.Add("SetGameStateF".ToLowerInvariant(), Game.SetGameStateF);
      Functions.Add("GetGameStateS".ToLowerInvariant(), Game.GetGameStateS);
      Functions.Add("SetGameStateS".ToLowerInvariant(), Game.SetGameStateS);
      Functions.Add("GetRegisterCount".ToLowerInvariant(), Game.GetRegisterCount);
      Functions.Add("GetTimeSinceLostWing".ToLowerInvariant(), Game.GetTimeSinceLostWing);
      Functions.Add("GetTimeSinceLostShip".ToLowerInvariant(), Game.GetTimeSinceLostShip);
      Functions.Add("AddEvent".ToLowerInvariant(), Game.AddEvent);

      // Player Management
      Functions.Add("Player.AssignActor".ToLowerInvariant(), Scripting.Functions.Player.AssignActor);
      Functions.Add("Player.GetActor".ToLowerInvariant(), Scripting.Functions.Player.GetActor);
      Functions.Add("Player.RequestSpawn".ToLowerInvariant(), Scripting.Functions.Player.RequestSpawn);
      Functions.Add("Player.SetMovementEnabled".ToLowerInvariant(), Scripting.Functions.Player.SetMovementEnabled);
      Functions.Add("Player.SetAI".ToLowerInvariant(), Scripting.Functions.Player.SetAI);
      Functions.Add("Player.SetLives".ToLowerInvariant(), Scripting.Functions.Player.SetLives);
      Functions.Add("Player.DecreaseLives".ToLowerInvariant(), Scripting.Functions.Player.DecreaseLives);
      Functions.Add("Player.SetScorePerLife".ToLowerInvariant(), Scripting.Functions.Player.SetScorePerLife);
      Functions.Add("Player.SetScoreForNextLife".ToLowerInvariant(), Scripting.Functions.Player.SetScoreForNextLife);
      Functions.Add("Player.ResetScore".ToLowerInvariant(), Scripting.Functions.Player.ResetScore);

      // Faction
      Functions.Add("AddFaction".ToLowerInvariant(), Faction.AddFaction);
      //Functions.Add("Faction.SetAsMainAllyFaction".ToLowerInvariant(), FactionManagement.SetAsMainAllyFaction);
      //Functions.Add("Faction.SetAsMainEnemyFaction".ToLowerInvariant(), FactionManagement.SetAsMainEnemyFaction);
      Functions.Add("Faction.MakeAlly".ToLowerInvariant(), Faction.MakeAlly);
      Functions.Add("Faction.MakeEnemy".ToLowerInvariant(), Faction.MakeEnemy);
      Functions.Add("Faction.GetWingCount".ToLowerInvariant(), Faction.GetWingCount);
      Functions.Add("Faction.GetShipCount".ToLowerInvariant(), Faction.GetShipCount);
      Functions.Add("Faction.GetStructureCount".ToLowerInvariant(), Faction.GetStructureCount);
      Functions.Add("Faction.GetWingLimit".ToLowerInvariant(), Faction.GetWingLimit);
      Functions.Add("Faction.GetShipLimit".ToLowerInvariant(), Faction.GetShipLimit);
      Functions.Add("Faction.GetStructureLimit".ToLowerInvariant(), Faction.GetStructureLimit);
      Functions.Add("Faction.SetWingLimit".ToLowerInvariant(), Faction.SetWingLimit);
      Functions.Add("Faction.SetShipLimit".ToLowerInvariant(), Faction.SetShipLimit);
      Functions.Add("Faction.SetStructureLimit".ToLowerInvariant(), Faction.SetStructureLimit);
      Functions.Add("Faction.GetWingSpawnLimit".ToLowerInvariant(), Faction.GetWingSpawnLimit);
      Functions.Add("Faction.GetShipSpawnLimit".ToLowerInvariant(), Faction.GetShipSpawnLimit);
      Functions.Add("Faction.GetStructureSpawnLimit".ToLowerInvariant(), Faction.GetStructureSpawnLimit);
      Functions.Add("Faction.SetWingSpawnLimit".ToLowerInvariant(), Faction.SetWingSpawnLimit);
      Functions.Add("Faction.SetShipSpawnLimit".ToLowerInvariant(), Faction.SetShipSpawnLimit);
      Functions.Add("Faction.SetStructureSpawnLimit".ToLowerInvariant(), Faction.SetStructureSpawnLimit);

      // Sounds and Music
      Functions.Add("SetMusic".ToLowerInvariant(), Audio.SetMusic);
      Functions.Add("SetMusicDyn".ToLowerInvariant(), Audio.SetMusicDyn);
      Functions.Add("SetMusicLoop".ToLowerInvariant(), Audio.SetMusicLoop);
      Functions.Add("SetMusicPause".ToLowerInvariant(), Audio.SetMusicPause);
      Functions.Add("SetMusicResume".ToLowerInvariant(), Audio.SetMusicResume);
      Functions.Add("SetMusicStop".ToLowerInvariant(), Audio.SetMusicStop);

      // UI
      Functions.Add("SetUILine1Color".ToLowerInvariant(), Scripting.Functions.UI.SetUILine1Color);
      Functions.Add("SetUILine2Color".ToLowerInvariant(), Scripting.Functions.UI.SetUILine2Color);
      Functions.Add("SetUILine3Color".ToLowerInvariant(), Scripting.Functions.UI.SetUILine3Color);
      Functions.Add("SetUILine1Text".ToLowerInvariant(), Scripting.Functions.UI.SetUILine1Text);
      Functions.Add("SetUILine2Text".ToLowerInvariant(), Scripting.Functions.UI.SetUILine2Text);
      Functions.Add("SetUILine3Text".ToLowerInvariant(), Scripting.Functions.UI.SetUILine3Text);

      // Script flow
      Functions.Add("CallScript".ToLowerInvariant(), Scripting.Functions.Scripting.CallScript);

      // Misc
      Functions.Add("IsNull".ToLowerInvariant(), IsNull);
      Functions.Add("GetArrayElement".ToLowerInvariant(), Scripting.Functions.Scripting.GetArrayElement);
    }

    public Val IsNull(Context c, Val[] ps)
    {
      return new Val(ps[0].IsNull);
    }
  }
}
