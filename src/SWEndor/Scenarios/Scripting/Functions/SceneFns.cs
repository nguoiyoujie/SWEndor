using SWEndor.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using System;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SceneFns
  {
    /// <summary>
    /// Triggers the audio Mood
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     INT mood
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetMood(Context context, int state)
    {
#if DEBUG
      if (context.Engine.GameScenarioManager.Scenario == null)
        throw new InvalidOperationException("Attempted script function with null Scenario");
#endif

      context.Engine.SoundManager.SetMood(state);
      return Val.NULL;
    }

    /// <summary>
    /// Max the maximum scene boundaries for the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>
    public static Val SetMaxBounds(Context context, float3 value)
    {
      context.Engine.GameScenarioManager.Scenario.State.MaxBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the minimum scene boundaries for the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>    
    public static Val SetMinBounds(Context context, float3 value)
    {
      context.Engine.GameScenarioManager.Scenario.State.MinBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the maximum scene boundaries for the AI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>
    public static Val SetMaxAIBounds(Context context, float3 value)
    {
      context.Engine.GameScenarioManager.Scenario.State.MaxAIBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the minimum scene boundaries for the AI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>
    public static Val SetMinAIBounds(Context context, float3 value)
    {
      context.Engine.GameScenarioManager.Scenario.State.MinAIBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables lost fighter notifications
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="enable">The enable flag</param>
    /// <returns>NULL</returns>
    public static Val EnableInformLostWing(Context context, bool enable)
    {
#if DEBUG
      if (context.Engine.GameScenarioManager.Scenario == null)
        throw new InvalidOperationException("Attempted script function with null Scenario");
#endif

      context.Engine.GameScenarioManager.Scenario.State.InformLostWing = enable;
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables lost ship notifications
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="enable">The enable flag</param>
    /// <returns>NULL</returns>
    public static Val EnableInformLostShip(Context context, bool enable)
    {
#if DEBUG
      if (context.Engine.GameScenarioManager.Scenario == null)
        throw new InvalidOperationException("Attempted script function with null Scenario");
#endif

      context.Engine.GameScenarioManager.Scenario.State.InformLostShip = enable;
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables lost structure notifications
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="enable">The enable flag</param>
    /// <returns>NULL</returns>
    public static Val EnableInformLostStructure(Context context, bool enable)
    {
#if DEBUG
      if (context.Engine.GameScenarioManager.Scenario == null)
        throw new InvalidOperationException("Attempted script function with null Scenario");
#endif

      context.Engine.GameScenarioManager.Scenario.State.InformLostStructure = enable;
      return Val.NULL;
    }

    /// <summary>
    /// Begin screen fade out.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>NULL</returns>
    public static Val FadeOut(Context context)
    {
#if DEBUG
      if (context.Engine.GameScenarioManager.Scenario == null)
        throw new InvalidOperationException("Attempted script function with null Scenario");
#endif

      context.Engine.GameScenarioManager.Scenario.FadeOut();
      return Val.NULL;
    }
  }
}
