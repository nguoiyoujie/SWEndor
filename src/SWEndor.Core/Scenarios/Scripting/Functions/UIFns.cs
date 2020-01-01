
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
    public static Val SetUILine1Color(Context context, float3 c)
    {
      COLOR color = new COLOR(c);
      context.Engine.Screen2D.Line1.Color = color;
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
    public static Val SetUILine2Color(Context context, float3 c)
    {
      COLOR color = new COLOR(c);
      context.Engine.Screen2D.Line2.Color = color;
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
    public static Val SetUILine3Color(Context context, float3 c)
    {
      COLOR color = new COLOR(c);
      context.Engine.Screen2D.Line3.Color = color;
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
    public static Val SetUILine1Text(Context context, string text)
    {
      context.Engine.Screen2D.Line1.Text = text;
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
    public static Val SetUILine2Text(Context context, string text)
    {
      context.Engine.Screen2D.Line2.Text = text;
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
    public static Val SetUILine3Text(Context context, string text)
    {
      context.Engine.Screen2D.Line3.Text = text;
      return Val.TRUE;
    }
  }
}
