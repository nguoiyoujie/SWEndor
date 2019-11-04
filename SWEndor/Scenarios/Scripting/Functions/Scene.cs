﻿using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using SWEndor.Scenarios.Scripting.Expressions.Primitives;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class Scene
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
    public static Val SetMood(Context context, params Val[] ps)
    {
#if DEBUG
      if (context.Engine.GameScenarioManager.Scenario == null)
        throw new InvalidOperationException("Attempted script function with null Scenario");
#endif

      context.Engine.GameScenarioManager.Scenario.Mood = (MoodStates)(int)ps[0];
      return Val.NULL;
    }

    /// <summary>
    /// Max the maximum scene boundaries for the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT3 bounds
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetMaxBounds(Context context, params Val[] ps)
    {
      float3 bounds = (float3)ps[0];
      context.Engine.GameScenarioManager.MaxBounds = bounds.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the minimum scene boundaries for the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT3 bounds
    /// </param>
    /// <returns>NULL</returns>    
    public static Val SetMinBounds(Context context, params Val[] ps)
    {
      float3 bounds = (float3)ps[0];
      context.Engine.GameScenarioManager.MinBounds = bounds.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the maximum scene boundaries for the AI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT3 bounds
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetMaxAIBounds(Context context, params Val[] ps)
    {
      float3 bounds = (float3)ps[0];
      context.Engine.GameScenarioManager.MaxAIBounds = bounds.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the minimum scene boundaries for the AI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT3 bounds
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetMinAIBounds(Context context, params Val[] ps)
    {
      float3 bounds = (float3)ps[0];
      context.Engine.GameScenarioManager.MinAIBounds = bounds.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Begin screen fade out.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     NONE
    /// </param>
    /// <returns>NULL</returns>
    public static Val FadeOut(Context context, params Val[] ps)
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
