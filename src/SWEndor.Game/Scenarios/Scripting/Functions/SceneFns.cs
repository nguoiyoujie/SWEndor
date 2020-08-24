using SWEndor.Game.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;
using Primrose.Expressions;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class SceneFns
  {
    /// <summary>
    /// Max the maximum scene boundaries for the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>
    public static Val SetMaxBounds(IContext context, float3 value)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.State.MaxBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the minimum scene boundaries for the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>    
    public static Val SetMinBounds(IContext context, float3 value)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.State.MinBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the maximum scene boundaries for the AI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>
    public static Val SetMaxAIBounds(IContext context, float3 value)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.State.MaxAIBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Max the minimum scene boundaries for the AI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="value">The bound value</param>
    /// <returns>NULL</returns>
    public static Val SetMinAIBounds(IContext context, float3 value)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.State.MinAIBounds = value.ToVec3();
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables lost fighter notifications
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="enable">The enable flag</param>
    /// <returns>NULL</returns>
    public static Val EnableInformLostWing(IContext context, bool enable)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.State.InformLostWing = enable;
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables lost ship notifications
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="enable">The enable flag</param>
    /// <returns>NULL</returns>
    public static Val EnableInformLostShip(IContext context, bool enable)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.State.InformLostShip = enable;
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables lost structure notifications
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="enable">The enable flag</param>
    /// <returns>NULL</returns>
    public static Val EnableInformLostStructure(IContext context, bool enable)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.State.InformLostStructure = enable;
      return Val.NULL;
    }

    /// <summary>
    /// Begin screen fade out.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>NULL</returns>
    public static Val FadeOut(IContext context)
    {
      ((Context)context).Engine.GameScenarioManager.Scenario.FadeOut();
      return Val.NULL;
    }
  }
}
