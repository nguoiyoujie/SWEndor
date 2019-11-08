using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using Primrose.Primitives.ValueTypes;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class UIFns
  {
    /// <summary>
    /// Sets the color of line 1 of the scenario UI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT3 color
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetUILine1Color(Context context, Val[] ps)
    {
      float3 c = (float3)ps[0];
      COLOR color = new COLOR(c);
      context.Engine.GameScenarioManager.Line1Color = color;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the color of line 2 of the scenario UI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT3 color
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetUILine2Color(Context context, Val[] ps)
    {
      float3 c = (float3)ps[0];
      COLOR color = new COLOR(c);
      context.Engine.GameScenarioManager.Line2Color = color;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the color of line 3 of the scenario UI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT3 color
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetUILine3Color(Context context, Val[] ps)
    {
      float3 c = (float3)ps[0];
      COLOR color = new COLOR(c);
      context.Engine.GameScenarioManager.Line3Color = color;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the text of line 1 of the scenario UI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING text
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetUILine1Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line1Text = (string)ps[0];
      return Val.TRUE;
    }

    /// <summary>
    /// Sets the text of line 2 of the scenario UI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING text
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetUILine2Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line2Text = (string)ps[0];
      return Val.TRUE;
    }

    /// <summary>
    /// Sets the text of line 3 of the scenario UI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING text
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetUILine3Text(Context context, Val[] ps)
    {
      context.Engine.GameScenarioManager.Line3Text = (string)ps[0];
      return Val.TRUE;
    }
  }
}
