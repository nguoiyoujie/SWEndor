using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Factories;
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
  public class Context : AContext
  {
    /// <summary>The game engine</summary>
    public readonly Engine Engine;

    /// <summary>The list of supported game functions</summary>
    public readonly Registry<FunctionDelegate> Functions = new Registry<FunctionDelegate>();

    public readonly Registry<Pair<string, int>, IValFunc> ValFuncs = new Registry<Pair<string, int>, IValFunc>();


    internal Context(Engine engine)
    {
      Engine = engine;
      DefineFunc();
    }

    /// <summary>
    /// Runs a user defined function. An EvalException should be thrown if errors arise from the function.
    /// </summary>
    /// <param name="caller">The script object that called this function</param>
    /// <param name="_funcName">The function name</param>
    /// <param name="param">The list of parameters</param>
    /// <returns></returns>
    public override Val RunFunction(ITracker caller, string _funcName, Val[] param)
    {
      FunctionDelegate fd = Functions.Get(_funcName);
      if (fd == null)
      {
        IValFunc vfs = ValFuncs.Get(new Pair<string, int>(_funcName, param.Length));
        if (vfs == null)
          throw new EvalException(caller, "The function '{0}' does not exist!".F(_funcName));

        return vfs.Execute(caller, _funcName, this, param);
      }
      return fd.Invoke(this, param);
    }

    internal void Reset() { }

    private void AddFunc(string name, ValFunc fn) { ValFuncs.Add(new Pair<string, int>(name, 0), fn); }
    private void AddFunc<T1>(string name, ValFunc<T1> fn) { ValFuncs.Add(new Pair<string, int>(name, 1), fn); }
    private void AddFunc<T1, T2>(string name, ValFunc<T1, T2> fn) { ValFuncs.Add(new Pair<string, int>(name, 2), fn); }
    private void AddFunc<T1, T2, T3>(string name, ValFunc<T1, T2, T3> fn) { ValFuncs.Add(new Pair<string, int>(name, 3), fn); }
    private void AddFunc<T1, T2, T3, T4>(string name, ValFunc<T1, T2, T3, T4> fn) { ValFuncs.Add(new Pair<string, int>(name, 4), fn); }
    private void AddFunc<T1, T2, T3, T4, T5>(string name, ValFunc<T1, T2, T3, T4, T5> fn) { ValFuncs.Add(new Pair<string, int>(name, 5), fn); }
    private void AddFunc<T1, T2, T3, T4, T5, T6>(string name, ValFunc<T1, T2, T3, T4, T5, T6> fn) { ValFuncs.Add(new Pair<string, int>(name, 6), fn); }
    private void AddFunc<T1, T2, T3, T4, T5, T6, T7>(string name, ValFunc<T1, T2, T3, T4, T5, T6, T7> fn) { ValFuncs.Add(new Pair<string, int>(name, 7), fn); }
    private void AddFunc<T1, T2, T3, T4, T5, T6, T7, T8>(string name, ValFunc<T1, T2, T3, T4, T5, T6, T7, T8> fn) { ValFuncs.Add(new Pair<string, int>(name, 8), fn); }

    internal void DefineFunc()
    {
      ValFuncs.Clear();
      Functions.Clear();

      // Scene Management
      AddFunc("Scene.SetMaxBounds", new ValFunc<float3>(SceneFns.SetMaxBounds));
      AddFunc("Scene.SetMinBounds", new ValFunc<float3>(SceneFns.SetMinBounds));
      AddFunc("Scene.SetMaxAIBounds", new ValFunc<float3>(SceneFns.SetMaxAIBounds));
      AddFunc("Scene.SetMinAIBounds", new ValFunc<float3>(SceneFns.SetMinAIBounds));
      AddFunc("Scene.FadeOut", new ValFunc(SceneFns.FadeOut));

      // Scene Camera Management
      AddFunc("Camera.SetPlayerLook", new ValFunc(PlayerCameraFns.SetPlayerLook));
      AddFunc("Camera.SetSceneLook", new ValFunc(PlayerCameraFns.SetSceneLook));
      AddFunc("Camera.SetDeathLook", new ValFunc(PlayerCameraFns.SetDeathLook));
      AddFunc("Camera.SetSceneLook_LookAtActor", new ValFunc<int>(PlayerCameraFns.SetSceneLook_LookAtActor));
      AddFunc("Camera.SetSceneLook_LookAtActor", new ValFunc<int, float3>(PlayerCameraFns.SetSceneLook_LookAtActor));
      AddFunc("Camera.SetSceneLook_LookAtActor", new ValFunc<int, float3, float3>(PlayerCameraFns.SetSceneLook_LookAtActor));
      AddFunc("Camera.SetSceneLook_LookAtPoint", new ValFunc<float3>(PlayerCameraFns.SetSceneLook_LookAtPoint));
      AddFunc("Camera.SetSceneLook_LookFromActor", new ValFunc<int>(PlayerCameraFns.SetSceneLook_LookFromActor));
      AddFunc("Camera.SetSceneLook_LookFromActor", new ValFunc<int, float3>(PlayerCameraFns.SetSceneLook_LookFromActor));
      AddFunc("Camera.SetSceneLook_LookFromActor", new ValFunc<int, float3, float3>(PlayerCameraFns.SetSceneLook_LookFromActor));
      AddFunc("Camera.SetSceneLook_LookFromPoint", new ValFunc<float3>(PlayerCameraFns.SetSceneLook_LookFromPoint));

      // Actor Management
      Functions.Add("Actor.Squadron_Spawn", ActorFns.Squadron_Spawn);
      AddFunc("Actor.AddToSquad", new ValFunc<int, int>(ActorFns.AddToSquad));
      AddFunc("Actor.RemoveFromSquad", new ValFunc<int>(ActorFns.RemoveFromSquad));
      AddFunc("Actor.MakeSquadLeader", new ValFunc<int>(ActorFns.MakeSquadLeader));

      Functions.Add("Actor.Spawn", ActorFns.Spawn);
      AddFunc("Actor.GetActorType", new ValFunc<int>(ActorFns.GetActorType));
      AddFunc("Actor.IsFighter", new ValFunc<int>(ActorFns.IsFighter));
      AddFunc("Actor.IsLargeShip", new ValFunc<int>(ActorFns.IsLargeShip));
      AddFunc("Actor.IsAlive", new ValFunc<int>(ActorFns.IsAlive));
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
      AddFunc("Actor.GetProperty", new ValFunc<int, string>(ActorFns.GetProperty));
      Functions.Add("Actor.SetProperty", ActorFns.SetProperty);

      // Message Box
      AddFunc("Message", new ValFunc<string, float, float3>(MessagingFns.MessageText));
      AddFunc("Message", new ValFunc<string, float, float3, int>(MessagingFns.MessageText));

      // Action Management
      Functions.Add("AI.QueueFirst", AIFns.QueueFirst);
      Functions.Add("AI.QueueNext", AIFns.QueueNext);
      Functions.Add("AI.QueueLast", AIFns.QueueLast);
      AddFunc("AI.UnlockActor", new ValFunc<int>(AIFns.UnlockActor));
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

      AddFunc("Player.SetScorePerLife", new ValFunc<float>(ScoreFns.SetScorePerLife));
      AddFunc("Player.SetScoreForNextLife", new ValFunc<float>(ScoreFns.SetScoreForNextLife));
      AddFunc("Player.ResetScore", new ValFunc(ScoreFns.ResetScore));

      // Faction
      AddFunc("Faction.Add", new ValFunc<string, float3>(FactionFns.AddFaction));
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
      AddFunc("Audio.SetMood", new ValFunc<int>(SceneFns.SetMood));
      AddFunc("Audio.SetMusic", new ValFunc<string>(AudioFns.SetMusic));
      AddFunc("Audio.SetMusicDyn", new ValFunc<string>(AudioFns.SetMusicDyn));
      AddFunc("Audio.SetMusicLoop", new ValFunc<string>(AudioFns.SetMusicLoop));
      AddFunc("Audio.SetMusicPause", new ValFunc(AudioFns.SetMusicPause));
      AddFunc("Audio.SetMusicResume", new ValFunc(AudioFns.SetMusicResume));
      AddFunc("Audio.SetMusicStop", new ValFunc(AudioFns.SetMusicStop));

      // UI
      AddFunc("UI.SetLine1Color", new ValFunc<float3>(UIFns.SetUILine1Color));
      AddFunc("UI.SetLine2Color", new ValFunc<float3>(UIFns.SetUILine2Color));
      AddFunc("UI.SetLine3Color", new ValFunc<float3>(UIFns.SetUILine3Color));
      AddFunc("UI.SetLine1Text", new ValFunc<string>(UIFns.SetUILine1Text));
      AddFunc("UI.SetLine2Text", new ValFunc<string>(UIFns.SetUILine2Text));
      AddFunc("UI.SetLine3Text", new ValFunc<string>(UIFns.SetUILine3Text));

      // Script flow
      AddFunc("Script.Call", new ValFunc<string>(ScriptingFns.CallScript));
      AddFunc("Script.CallX", new ValFunc<string>(ScriptingFns.CallScriptX));

      // Misc
      AddFunc("IsNull", new ValFunc<Val>(MiscFns.IsNull));
      AddFunc("GetArrayElement", new ValFunc<Val, int>(MiscFns.GetArrayElement));
    }
  }
}
