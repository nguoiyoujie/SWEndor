using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
using Primrose.Primitives.ValueTypes;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Scenarios.Scripting.Functions;
using System.Collections.Generic;

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

    public Context(Engine engine)
    {
      Engine = engine;
      DefineFunc();
    }

    public override void DefineFunc()
    {
      ValFuncs.Clear();
      Functions.Clear();
      ValFuncRef.Clear();

      // Scene Management
      AddFunc("Scene.SetMaxBounds", new ValFunc<float3>(SceneFns.SetMaxBounds));
      AddFunc("Scene.SetMinBounds", new ValFunc<float3>(SceneFns.SetMinBounds));
      AddFunc("Scene.SetMaxAIBounds", new ValFunc<float3>(SceneFns.SetMaxAIBounds));
      AddFunc("Scene.SetMinAIBounds", new ValFunc<float3>(SceneFns.SetMinAIBounds));
      AddFunc("Scene.FadeOut", new ValFunc(SceneFns.FadeOut));

      // Camera Management
      AddFunc("Camera.SetPlayerLook", new ValFunc(PlayerCameraFns.SetPlayerLook));
      AddFunc("Camera.SetSceneLook", new ValFunc(PlayerCameraFns.SetSceneLook));
      AddFunc("Camera.SetDeathLook", new ValFunc(PlayerCameraFns.SetDeathLook));
      AddFunc("Camera.EnableFreeLook", new ValFunc<bool>(PlayerCameraFns.EnableFreeLook));
      AddFunc("Camera.SetSceneLook_LookAtActor", new ValFunc<int>(PlayerCameraFns.SetSceneLook_LookAtActor));
      AddFunc("Camera.SetSceneLook_LookAtActor", new ValFunc<int, float3>(PlayerCameraFns.SetSceneLook_LookAtActor));
      AddFunc("Camera.SetSceneLook_LookAtActor", new ValFunc<int, float3, float3>(PlayerCameraFns.SetSceneLook_LookAtActor));
      AddFunc("Camera.SetSceneLook_LookAtPoint", new ValFunc<float3>(PlayerCameraFns.SetSceneLook_LookAtPoint));
      AddFunc("Camera.SetSceneLook_LookFromActor", new ValFunc<int>(PlayerCameraFns.SetSceneLook_LookFromActor));
      AddFunc("Camera.SetSceneLook_LookFromActor", new ValFunc<int, float3>(PlayerCameraFns.SetSceneLook_LookFromActor));
      AddFunc("Camera.SetSceneLook_LookFromActor", new ValFunc<int, float3, float3>(PlayerCameraFns.SetSceneLook_LookFromActor));
      AddFunc("Camera.SetSceneLook_LookFromPoint", new ValFunc<float3>(PlayerCameraFns.SetSceneLook_LookFromPoint));

      // Spawn Management
      Functions.Add("Squad.Spawn", SpawnFns.Squadron_Spawn);
      Functions.Add("Actor.Spawn", SpawnFns.Spawn);
      AddFunc("Actor.QueueAtSpawner", new ValFunc<int, int>(SpawnFns.QueueAtSpawner));

      // Squad Management
      AddFunc("Squad.JoinSquad", new ValFunc<int, int>(SquadFns.JoinSquad));
      AddFunc("Squad.RemoveFromSquad", new ValFunc<int>(SquadFns.RemoveFromSquad));
      AddFunc("Squad.MakeSquadLeader", new ValFunc<int>(SquadFns.MakeSquadLeader));

      // Actor Management
      AddFunc("Actor.GetActorType", new ValFunc<int>(ActorFns.GetActorType));
      AddFunc("Actor.IsFighter", new ValFunc<int>(ActorFns.IsFighter));
      AddFunc("Actor.IsLargeShip", new ValFunc<int>(ActorFns.IsLargeShip));
      AddFunc("Actor.IsAlive", new ValFunc<int>(ActorFns.IsAlive));
      AddFunc("Actor.GetFaction", new ValFunc<int>(ActorFns.GetFaction));
      AddFunc("Actor.SetFaction", new ValFunc<int, string>(ActorFns.SetFaction));
      AddFunc("Actor.AddToRegister", new ValFunc<int, string>(ActorFns.AddToRegister));
      AddFunc("Actor.RemoveFromRegister", new ValFunc<int, string>(ActorFns.RemoveFromRegister));
      AddFunc("Actor.GetLocalPosition", new ValFunc<int>(ActorFns.GetLocalPosition));
      AddFunc("Actor.GetLocalRotation", new ValFunc<int>(ActorFns.GetLocalRotation));
      AddFunc("Actor.GetLocalDirection", new ValFunc<int>(ActorFns.GetLocalDirection));
      AddFunc("Actor.GetGlobalPosition", new ValFunc<int>(ActorFns.GetGlobalPosition));
      AddFunc("Actor.GetGlobalRotation", new ValFunc<int>(ActorFns.GetGlobalRotation));
      AddFunc("Actor.GetGlobalDirection", new ValFunc<int>(ActorFns.GetGlobalDirection));
      AddFunc("Actor.SetLocalPosition", new ValFunc<int, float3>(ActorFns.SetLocalPosition));
      AddFunc("Actor.SetLocalRotation", new ValFunc<int, float3>(ActorFns.SetLocalRotation));
      AddFunc("Actor.SetLocalDirection", new ValFunc<int, float3>(ActorFns.SetLocalDirection));

      AddFunc("Actor.LookAtPoint", new ValFunc<int, float3>(ActorFns.LookAtPoint));
      AddFunc("Actor.GetChildren", new ValFunc<int>(ActorFns.GetChildren));
      AddFunc("Actor.GetChildrenByType", new ValFunc<int, string>(ActorFns.GetChildrenByType));
      AddFunc("Actor.GetProperty", new ValFunc<int, string>(ActorFns.GetProperty));
      Functions.Add("Actor.SetProperty", ActorFns.SetProperty);
      AddFunc("Actor.GetArmor", new ValFunc<int, string>(ActorFns.GetArmor));
      AddFunc("Actor.SetArmor", new ValFunc<int, string, float>(ActorFns.SetArmor));
      AddFunc("Actor.SetArmorAll", new ValFunc<int, float>(ActorFns.SetArmorAll));
      AddFunc("Actor.RestoreArmor", new ValFunc<int>(ActorFns.RestoreArmor));

      AddFunc("Actor.GetHP", new ValFunc<int>(ActorFns.GetHP));
      AddFunc("Actor.GetMaxHP", new ValFunc<int>(ActorFns.GetMaxHP));
      AddFunc("Actor.SetHP", new ValFunc<int, float>(ActorFns.SetHP));
      AddFunc("Actor.SetMaxHP", new ValFunc<int, float>(ActorFns.SetMaxHP));

      AddFunc("Actor.GetHull", new ValFunc<int>(ActorFns.GetHull));
      AddFunc("Actor.GetMaxHull", new ValFunc<int>(ActorFns.GetMaxHull));
      AddFunc("Actor.SetHull", new ValFunc<int, float>(ActorFns.SetHull));
      AddFunc("Actor.SetMaxHull", new ValFunc<int, float>(ActorFns.SetMaxHull));

      AddFunc("Actor.GetShd", new ValFunc<int>(ActorFns.GetShd));
      AddFunc("Actor.GetMaxShd", new ValFunc<int>(ActorFns.GetMaxShd));
      AddFunc("Actor.SetShd", new ValFunc<int, float>(ActorFns.SetShd));
      AddFunc("Actor.SetMaxShd", new ValFunc<int, float>(ActorFns.SetMaxShd));


      // Message Box
      AddFunc("Message", new ValFunc<string, float, float3>(MessagingFns.MessageText));
      AddFunc("Message", new ValFunc<string, float, float3, int>(MessagingFns.MessageText));

      // Action Management
      Functions.Add("AI.QueueFirst", AIFns.QueueFirst);
      Functions.Add("AI.QueueNext", AIFns.QueueNext);
      Functions.Add("AI.QueueLast", AIFns.QueueLast);
      AddFunc("AI.UnlockOne", new ValFunc<int>(AIFns.UnlockOne));
      AddFunc("AI.ClearQueue", new ValFunc<int>(AIFns.ClearQueue));
      AddFunc("AI.ForceClearQueue", new ValFunc<int>(AIFns.ForceClearQueue));

      // Game states
      AddFunc("GetGameTime", new ValFunc(GameFns.GetGameTime));
      AddFunc("GetLastFrameTime", new ValFunc(GameFns.GetLastFrameTime));
      AddFunc("GetDifficulty", new ValFunc(GameFns.GetDifficulty));
      AddFunc("GetPlayerActorType", new ValFunc(GameFns.GetPlayerActorType));
      AddFunc("GetPlayerName", new ValFunc(GameFns.GetPlayerName));
      AddFunc("GetStageNumber", new ValFunc(GameFns.GetStageNumber));
      AddFunc("SetStageNumber", new ValFunc<int>(GameFns.SetStageNumber));
      AddFunc("GetGameStateB", new ValFunc<string>(GameFns.GetGameStateB));
      AddFunc("GetGameStateB", new ValFunc<string, bool>(GameFns.GetGameStateB));
      AddFunc("SetGameStateB", new ValFunc<string, bool>(GameFns.SetGameStateB));
      AddFunc("GetGameStateF", new ValFunc<string>(GameFns.GetGameStateF));
      AddFunc("GetGameStateF", new ValFunc<string, float>(GameFns.GetGameStateF));
      AddFunc("SetGameStateF", new ValFunc<string, float>(GameFns.SetGameStateF));
      AddFunc("GetGameStateS", new ValFunc<string>(GameFns.GetGameStateS));
      AddFunc("GetGameStateS", new ValFunc<string, string>(GameFns.GetGameStateS));
      AddFunc("SetGameStateS", new ValFunc<string, string>(GameFns.SetGameStateS));
      AddFunc("GetRegisterCount", new ValFunc<string>(GameFns.GetRegisterCount));
      AddFunc("GetTimeSinceLostWing", new ValFunc(GameFns.GetTimeSinceLostWing));
      AddFunc("GetTimeSinceLostShip", new ValFunc(GameFns.GetTimeSinceLostShip));
      AddFunc("GetTimeSinceLostStructure", new ValFunc(GameFns.GetTimeSinceLostStructure));
      AddFunc("AddEvent", new ValFunc<float, string>(GameFns.AddEvent));

      // Player Management
      AddFunc("Player.AssignActor", new ValFunc<int>(PlayerFns.AssignActor));
      AddFunc("Player.GetActor", new ValFunc(PlayerFns.GetActor));
      AddFunc("Player.RequestSpawn", new ValFunc(PlayerFns.RequestSpawn));
      AddFunc("Player.SetMovementEnabled", new ValFunc<bool>(PlayerFns.SetMovementEnabled));
      AddFunc("Player.SetAI", new ValFunc<bool>(PlayerFns.SetAI));
      AddFunc("Player.SetLives", new ValFunc<int>(PlayerFns.SetLives));
      AddFunc("Player.DecreaseLives", new ValFunc(PlayerFns.DecreaseLives));

      // Score Management
      AddFunc("Score.SetScorePerLife", new ValFunc<float>(ScoreFns.SetScorePerLife));
      AddFunc("Score.SetScoreForNextLife", new ValFunc<float>(ScoreFns.SetScoreForNextLife));
      AddFunc("Score.Reset", new ValFunc(ScoreFns.ResetScore));

      // Faction
      AddFunc("Faction.Add", new ValFunc<string, float3>(FactionFns.AddFaction));
      AddFunc("Faction.Add", new ValFunc<string, float3, float3>(FactionFns.AddFaction));
      AddFunc("Faction.GetColor", new ValFunc<string>(FactionFns.GetColor));
      AddFunc("Faction.SetColor", new ValFunc<string, float3>(FactionFns.SetColor));
      AddFunc("Faction.GetLaserColor", new ValFunc<string>(FactionFns.GetLaserColor));
      AddFunc("Faction.SetLaserColor", new ValFunc<string, float3>(FactionFns.SetLaserColor));
      AddFunc("Faction.MakeAlly", new ValFunc<string, string>(FactionFns.MakeAlly));
      AddFunc("Faction.MakeEnemy", new ValFunc<string, string>(FactionFns.MakeEnemy));
      AddFunc("Faction.GetWingCount", new ValFunc<string>(FactionFns.GetWingCount));
      AddFunc("Faction.GetShipCount", new ValFunc<string>(FactionFns.GetShipCount));
      AddFunc("Faction.GetStructureCount", new ValFunc<string>(FactionFns.GetStructureCount));
      AddFunc("Faction.GetWingLimit", new ValFunc<string>(FactionFns.GetWingLimit));
      AddFunc("Faction.GetShipLimit", new ValFunc<string>(FactionFns.GetShipLimit));
      AddFunc("Faction.GetStructureLimit", new ValFunc<string>(FactionFns.GetStructureLimit));
      AddFunc("Faction.SetWingLimit", new ValFunc<string, int>(FactionFns.SetWingLimit));
      AddFunc("Faction.SetShipLimit", new ValFunc<string, int>(FactionFns.SetShipLimit));
      AddFunc("Faction.SetStructureLimit", new ValFunc<string, int>(FactionFns.SetStructureLimit));
      AddFunc("Faction.GetWingSpawnLimit", new ValFunc<string>(FactionFns.GetWingSpawnLimit));
      AddFunc("Faction.GetShipSpawnLimit", new ValFunc<string>(FactionFns.GetShipSpawnLimit));
      AddFunc("Faction.GetStructureSpawnLimit", new ValFunc<string>(FactionFns.GetStructureSpawnLimit));
      AddFunc("Faction.SetWingSpawnLimit", new ValFunc<string, int>(FactionFns.SetWingSpawnLimit));
      AddFunc("Faction.SetShipSpawnLimit", new ValFunc<string, int>(FactionFns.SetShipSpawnLimit));
      AddFunc("Faction.SetStructureSpawnLimit", new ValFunc<string, int>(FactionFns.SetStructureSpawnLimit));

      // Sounds and Music
      AddFunc("Audio.GetMood", new ValFunc(AudioFns.GetMood));
      AddFunc("Audio.SetMood", new ValFunc<int>(AudioFns.SetMood));
      AddFunc("Audio.SetMusic", new ValFunc<string>(AudioFns.SetMusic));
      AddFunc("Audio.SetMusicDyn", new ValFunc<string>(AudioFns.SetMusicDyn));
      AddFunc("Audio.SetMusicLoop", new ValFunc<string>(AudioFns.SetMusicLoop));
      AddFunc("Audio.PauseMusic", new ValFunc(AudioFns.PauseMusic));
      AddFunc("Audio.ResumeMusic", new ValFunc(AudioFns.ResumeMusic));
      AddFunc("Audio.StopMusic", new ValFunc(AudioFns.StopMusic));
      AddFunc("Audio.SetSound", new ValFunc<string>(AudioFns.SetSound));
      AddFunc("Audio.SetSound", new ValFunc<string, float>(AudioFns.SetSound));
      AddFunc("Audio.SetSound", new ValFunc<string, float, bool>(AudioFns.SetSound));
      AddFunc("Audio.SetSoundSingle", new ValFunc<string>(AudioFns.SetSoundSingle));
      AddFunc("Audio.SetSoundSingle", new ValFunc<string, bool>(AudioFns.SetSoundSingle));
      AddFunc("Audio.SetSoundSingle", new ValFunc<string, bool, float>(AudioFns.SetSoundSingle));
      AddFunc("Audio.SetSoundSingle", new ValFunc<string, bool, float, bool>(AudioFns.SetSoundSingle));
      AddFunc("Audio.StopSound", new ValFunc<string>(AudioFns.StopSound));
      AddFunc("Audio.StopAllSound", new ValFunc(AudioFns.StopAllSounds));

      // UI
      AddFunc("UI.SetLine1Color", new ValFunc<float3>(UIFns.SetUILine1Color));
      AddFunc("UI.SetLine2Color", new ValFunc<float3>(UIFns.SetUILine2Color));
      AddFunc("UI.SetLine3Color", new ValFunc<float3>(UIFns.SetUILine3Color));
      AddFunc("UI.SetLine1Text", new ValFunc<string>(UIFns.SetUILine1Text));
      AddFunc("UI.SetLine2Text", new ValFunc<string>(UIFns.SetUILine2Text));
      AddFunc("UI.SetLine3Text", new ValFunc<string>(UIFns.SetUILine3Text));

      // Script flow
      AddFunc("Script.TryCall", new ValFunc<string>(ScriptingFns.TryCall));
      AddFunc("Script.Call", new ValFunc<string>(ScriptingFns.Call));

      // Math
      AddFunc("Math.GetDistance", new ValFunc<float3, float3>(MathFns.GetDistance));
      AddFunc("Math.GetActorDistance", new ValFunc<int, int>(MathFns.GetActorDistance));
      AddFunc("Math.GetActorDistance", new ValFunc<int, int, float>(MathFns.GetActorDistance));
      AddFunc("Math.Int", new ValFunc<float>(MathFns.Int));
      AddFunc("Math.Max", new ValFunc<float, float>(MathFns.Max));
      AddFunc("Math.Min", new ValFunc<float, float>(MathFns.Min));
      AddFunc("Math.FormatAsTime", new ValFunc<float>(MathFns.FormatAsTime));

      // Misc
      AddFunc("IsNull", new ValFunc<Val>(MiscFns.IsNull));
      AddFunc("Random", new ValFunc(MiscFns.Random));
      AddFunc("Random", new ValFunc<int>(MiscFns.Random));
      AddFunc("Random", new ValFunc<int, int>(MiscFns.Random));
      AddFunc("GetArrayElement", new ValFunc<Val, int>(MiscFns.GetArrayElement));
    }
  }
}
