using Primrose.Expressions;
using Primrose.Primitives.ValueTypes;
using SWEndor.Core;
using SWEndor.Scenarios.Scripting.Functions;

namespace SWEndor.Scenarios.Scripting
{
  /// <summary>
  /// Represents a function for the script language to interact with the game context
  /// </summary>
  /// <param name="context">The game context</param>
  /// <param name="param">The list of parameters entering this function</param>
  /// <returns>A Val value for further processing</returns>
  public delegate Val FunctionDelegate(Context context, Val[] param);

  /// <summary>
  /// Provides the game context for the script
  /// </summary>
  public class Context : ContextBase
  {
    /// <summary>The game engine</summary>
    public readonly Engine Engine;

    public Context() { }

    public Context(Engine engine) : base()
    {
      Engine = engine;
    }

    protected override void DefineFunctions()
    {
      // Scene Management
      AddFunc<float3>("Scene.SetMaxBounds", SceneFns.SetMaxBounds);
      AddFunc<float3>("Scene.SetMinBounds", SceneFns.SetMinBounds);
      AddFunc<float3>("Scene.SetMaxAIBounds", SceneFns.SetMaxAIBounds);
      AddFunc<float3>("Scene.SetMinAIBounds", SceneFns.SetMinAIBounds);
      AddFunc("Scene.FadeOut", SceneFns.FadeOut);

      // Camera Management
      AddFunc("Camera.SetPlayerLook", PlayerCameraFns.SetPlayerLook);
      AddFunc("Camera.SetSceneLook", PlayerCameraFns.SetSceneLook);
      AddFunc("Camera.SetDeathLook", PlayerCameraFns.SetDeathLook);
      AddFunc<bool>("Camera.EnableFreeLook", PlayerCameraFns.EnableFreeLook);
      AddFunc<int>("Camera.SetSceneLook_LookAtActor", PlayerCameraFns.SetSceneLook_LookAtActor);
      AddFunc<int, float3>("Camera.SetSceneLook_LookAtActor", PlayerCameraFns.SetSceneLook_LookAtActor);
      AddFunc<int, float3, float3>("Camera.SetSceneLook_LookAtActor", PlayerCameraFns.SetSceneLook_LookAtActor);
      AddFunc<float3>("Camera.SetSceneLook_LookAtPoint", PlayerCameraFns.SetSceneLook_LookAtPoint);
      AddFunc<int>("Camera.SetSceneLook_LookFromActor", PlayerCameraFns.SetSceneLook_LookFromActor);
      AddFunc<int, float3>("Camera.SetSceneLook_LookFromActor", PlayerCameraFns.SetSceneLook_LookFromActor);
      AddFunc<int, float3, float3>("Camera.SetSceneLook_LookFromActor", PlayerCameraFns.SetSceneLook_LookFromActor);
      AddFunc<float3>("Camera.SetSceneLook_LookFromPoint", PlayerCameraFns.SetSceneLook_LookFromPoint);

      // Spawn Management
      AddFunc<string, string, string, int, float, bool, float3, float3, string, float, float, string>("Squad.Spawn", SpawnFns.Squadron_Spawn);
      AddFunc<string, string, string, int, float, bool, float3, float3, string, float, float, string, string[]>("Squad.Spawn", SpawnFns.Squadron_Spawn);
      AddFunc<string, string, string, string, float, float3, float3>("Actor.Spawn", SpawnFns.Spawn);
      AddFunc<string, string, string, string, float, float3, float3, string[]>("Actor.Spawn", SpawnFns.Spawn);
      AddFunc<int, int>("Actor.QueueAtSpawner", SpawnFns.QueueAtSpawner);

      // Squad Management
      AddFunc<int, int>("Squad.JoinSquad", SquadFns.JoinSquad);
      AddFunc<int>("Squad.RemoveFromSquad", SquadFns.RemoveFromSquad);
      AddFunc<int>("Squad.MakeSquadLeader", SquadFns.MakeSquadLeader);

      // Actor Management
      AddFunc<int>("Actor.GetActorType", ActorFns.GetActorType);
      AddFunc<int>("Actor.IsFighter", ActorFns.IsFighter);
      AddFunc<int>("Actor.IsLargeShip", ActorFns.IsLargeShip);
      AddFunc<int>("Actor.IsAlive", ActorFns.IsAlive);
      AddFunc<int>("Actor.GetFaction", ActorFns.GetFaction);
      AddFunc<int, string>("Actor.SetFaction", ActorFns.SetFaction);
      AddFunc<int, string>("Actor.AddToRegister", ActorFns.AddToRegister);
      AddFunc<int, string>("Actor.RemoveFromRegister", ActorFns.RemoveFromRegister);
      AddFunc<int>("Actor.GetLocalPosition", ActorFns.GetLocalPosition);
      AddFunc<int>("Actor.GetLocalRotation", ActorFns.GetLocalRotation);
      AddFunc<int>("Actor.GetLocalDirection", ActorFns.GetLocalDirection);
      AddFunc<int>("Actor.GetGlobalPosition", ActorFns.GetGlobalPosition);
      AddFunc<int>("Actor.GetGlobalRotation", ActorFns.GetGlobalRotation);
      AddFunc<int>("Actor.GetGlobalDirection", ActorFns.GetGlobalDirection);
      AddFunc<int, float3>("Actor.SetLocalPosition", ActorFns.SetLocalPosition);
      AddFunc<int, float3>("Actor.SetLocalRotation", ActorFns.SetLocalRotation);
      AddFunc<int, float3>("Actor.SetLocalDirection", ActorFns.SetLocalDirection);

      AddFunc<int, float3>("Actor.LookAtPoint", ActorFns.LookAtPoint);
      AddFunc<int>("Actor.GetChildren", ActorFns.GetChildren);
      AddFunc<int, string>("Actor.GetChildrenByType", ActorFns.GetChildrenByType);
      AddFunc<int, string>("Actor.GetProperty", ActorFns.GetProperty);
      AddFunc<int, string, Val>("Actor.SetProperty", ActorFns.SetProperty);
      AddFunc<int, string>("Actor.GetArmor", ActorFns.GetArmor);
      AddFunc<int, string, float>("Actor.SetArmor", ActorFns.SetArmor);
      AddFunc<int, float>("Actor.SetArmorAll", ActorFns.SetArmorAll);
      AddFunc<int>("Actor.RestoreArmor", ActorFns.RestoreArmor);

      AddFunc<int>("Actor.GetHP", ActorFns.GetHP);
      AddFunc<int>("Actor.GetMaxHP", ActorFns.GetMaxHP);
      AddFunc<int, float>("Actor.SetHP", ActorFns.SetHP);
      AddFunc<int, float>("Actor.SetMaxHP", ActorFns.SetMaxHP);

      AddFunc<int>("Actor.GetHull", ActorFns.GetHull);
      AddFunc<int>("Actor.GetMaxHull", ActorFns.GetMaxHull);
      AddFunc<int, float>("Actor.SetHull", ActorFns.SetHull);
      AddFunc<int, float>("Actor.SetMaxHull", ActorFns.SetMaxHull);

      AddFunc<int>("Actor.GetShd", ActorFns.GetShd);
      AddFunc<int>("Actor.GetMaxShd", ActorFns.GetMaxShd);
      AddFunc<int, float>("Actor.SetShd", ActorFns.SetShd);
      AddFunc<int, float>("Actor.SetMaxShd", ActorFns.SetMaxShd);


      // Message Box
      AddFunc<string, float, float3>("Message", MessagingFns.MessageText);
      AddFunc<string, float, float3, int>("Message", MessagingFns.MessageText);

      // Action Management
      AddDynamicFunc("AI.QueueFirst", AIFns.QueueFirst);
      AddDynamicFunc("AI.QueueNext", AIFns.QueueNext);
      AddDynamicFunc("AI.QueueLast", AIFns.QueueLast);
      AddFunc<int>("AI.UnlockOne", AIFns.UnlockOne);
      AddFunc<int>("AI.ClearQueue", AIFns.ClearQueue);
      AddFunc<int>("AI.ForceClearQueue", AIFns.ForceClearQueue);

      // Game states
      AddFunc("GetGameTime", GameFns.GetGameTime);
      AddFunc("GetLastFrameTime", GameFns.GetLastFrameTime);
      AddFunc("GetDifficulty", GameFns.GetDifficulty);
      AddFunc("GetPlayerActorType", GameFns.GetPlayerActorType);
      AddFunc("GetPlayerName", GameFns.GetPlayerName);
      AddFunc("GetStageNumber", GameFns.GetStageNumber);
      AddFunc<int>("SetStageNumber", GameFns.SetStageNumber);
      AddFunc<string>("GetGameStateB", GameFns.GetGameStateB);
      AddFunc<string, bool>("GetGameStateB", GameFns.GetGameStateB);
      AddFunc<string, bool>("SetGameStateB", GameFns.SetGameStateB);
      AddFunc<string>("GetGameStateF", GameFns.GetGameStateF);
      AddFunc<string, float>("GetGameStateF", GameFns.GetGameStateF);
      AddFunc<string, float>("SetGameStateF", GameFns.SetGameStateF);
      AddFunc<string>("GetGameStateS", GameFns.GetGameStateS);
      AddFunc<string, string>("GetGameStateS", GameFns.GetGameStateS);
      AddFunc<string, string>("SetGameStateS", GameFns.SetGameStateS);
      AddFunc<string>("GetRegisterCount", GameFns.GetRegisterCount);
      AddFunc("GetTimeSinceLostWing", GameFns.GetTimeSinceLostWing);
      AddFunc("GetTimeSinceLostShip", GameFns.GetTimeSinceLostShip);
      AddFunc("GetTimeSinceLostStructure", GameFns.GetTimeSinceLostStructure);
      AddFunc<float, string>("AddEvent", GameFns.AddEvent);

      // Player Management
      AddFunc<int>("Player.AssignActor", PlayerFns.AssignActor);
      AddFunc("Player.GetActor", PlayerFns.GetActor);
      AddFunc("Player.RequestSpawn", PlayerFns.RequestSpawn);
      AddFunc<bool>("Player.SetMovementEnabled", PlayerFns.SetMovementEnabled);
      AddFunc<bool>("Player.SetAI", PlayerFns.SetAI);
      AddFunc<int>("Player.SetLives", PlayerFns.SetLives);
      AddFunc("Player.DecreaseLives", PlayerFns.DecreaseLives);

      // Score Management
      AddFunc<float>("Score.SetScorePerLife", ScoreFns.SetScorePerLife);
      AddFunc<float>("Score.SetScoreForNextLife", ScoreFns.SetScoreForNextLife);
      AddFunc("Score.Reset", ScoreFns.ResetScore);

      // Faction
      AddFunc<string, float3>("Faction.Add", FactionFns.AddFaction);
      AddFunc<string, float3, float3>("Faction.Add", FactionFns.AddFaction);
      AddFunc<string>("Faction.GetColor", FactionFns.GetColor);
      AddFunc<string, float3>("Faction.SetColor", FactionFns.SetColor);
      AddFunc<string>("Faction.GetLaserColor", FactionFns.GetLaserColor);
      AddFunc<string, float3>("Faction.SetLaserColor", FactionFns.SetLaserColor);
      AddFunc<string, string>("Faction.MakeAlly", FactionFns.MakeAlly);
      AddFunc<string, string>("Faction.MakeEnemy", FactionFns.MakeEnemy);
      AddFunc<string>("Faction.GetWingCount", FactionFns.GetWingCount);
      AddFunc<string>("Faction.GetShipCount", FactionFns.GetShipCount);
      AddFunc<string>("Faction.GetStructureCount", FactionFns.GetStructureCount);
      AddFunc<string>("Faction.GetWingLimit", FactionFns.GetWingLimit);
      AddFunc<string>("Faction.GetShipLimit", FactionFns.GetShipLimit);
      AddFunc<string>("Faction.GetStructureLimit", FactionFns.GetStructureLimit);
      AddFunc<string, int>("Faction.SetWingLimit", FactionFns.SetWingLimit);
      AddFunc<string, int>("Faction.SetShipLimit", FactionFns.SetShipLimit);
      AddFunc<string, int>("Faction.SetStructureLimit", FactionFns.SetStructureLimit);
      AddFunc<string>("Faction.GetWingSpawnLimit", FactionFns.GetWingSpawnLimit);
      AddFunc<string>("Faction.GetShipSpawnLimit", FactionFns.GetShipSpawnLimit);
      AddFunc<string>("Faction.GetStructureSpawnLimit", FactionFns.GetStructureSpawnLimit);
      AddFunc<string, int>("Faction.SetWingSpawnLimit", FactionFns.SetWingSpawnLimit);
      AddFunc<string, int>("Faction.SetShipSpawnLimit", FactionFns.SetShipSpawnLimit);
      AddFunc<string, int>("Faction.SetStructureSpawnLimit", FactionFns.SetStructureSpawnLimit);

      // Sounds and Music
      AddFunc("Audio.GetMood", AudioFns.GetMood);
      AddFunc<int>("Audio.SetMood", AudioFns.SetMood);
      AddFunc<string>("Audio.SetMusic", AudioFns.SetMusic);
      AddFunc<string>("Audio.SetMusicDyn", AudioFns.SetMusicDyn);
      AddFunc<string>("Audio.SetMusicLoop", AudioFns.SetMusicLoop);
      AddFunc("Audio.PauseMusic", AudioFns.PauseMusic);
      AddFunc("Audio.ResumeMusic", AudioFns.ResumeMusic);
      AddFunc("Audio.StopMusic", AudioFns.StopMusic);
      AddFunc<string>("Audio.SetSound", AudioFns.SetSound);
      AddFunc<string, float>("Audio.SetSound", AudioFns.SetSound);
      AddFunc<string, float, bool>("Audio.SetSound", AudioFns.SetSound);
      AddFunc<string>("Audio.SetSoundSingle", AudioFns.SetSoundSingle);
      AddFunc<string, bool>("Audio.SetSoundSingle", AudioFns.SetSoundSingle);
      AddFunc<string, bool, float>("Audio.SetSoundSingle", AudioFns.SetSoundSingle);
      AddFunc<string, bool, float, bool>("Audio.SetSoundSingle", AudioFns.SetSoundSingle);
      AddFunc<string>("Audio.StopSound", AudioFns.StopSound);
      AddFunc("Audio.StopAllSound", AudioFns.StopAllSounds);

      // UI
      AddFunc<float3>("UI.SetLine1Color", UIFns.SetUILine1Color);
      AddFunc<float3>("UI.SetLine2Color", UIFns.SetUILine2Color);
      AddFunc<float3>("UI.SetLine3Color", UIFns.SetUILine3Color);
      AddFunc<string>("UI.SetLine1Text", UIFns.SetUILine1Text);
      AddFunc<string>("UI.SetLine2Text", UIFns.SetUILine2Text);
      AddFunc<string>("UI.SetLine3Text", UIFns.SetUILine3Text);

      // Script flow
      AddFunc<string>("Script.TryCall", ScriptingFns.TryCall);
      AddFunc<string>("Script.Call", ScriptingFns.Call);

      // Math
      AddFunc<float3, float3>("Math.GetDistance", MathFns.GetDistance);
      AddFunc<int, int>("Math.GetActorDistance", MathFns.GetActorDistance);
      AddFunc<int, int, float>("Math.GetActorDistance", MathFns.GetActorDistance);
      AddFunc<float>("Math.Int", MathFns.Int);
      AddFunc<float, float>("Math.Max", MathFns.Max);
      AddFunc<float, float>("Math.Min", MathFns.Min);
      AddFunc<float>("Math.FormatAsTime", MathFns.FormatAsTime);

      // Misc
      AddFunc<Val>("IsNull", MiscFns.IsNull);
      AddFunc("Random", MiscFns.Random);
      AddFunc<int>("Random", MiscFns.Random);
      AddFunc<int, int>("Random", MiscFns.Random);
      AddFunc<Val, int>("GetArrayElement", MiscFns.GetArrayElement);
    }
  }
}
