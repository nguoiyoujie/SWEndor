
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
    public static Val SetUILine1Color(IContext context, float3 c)
    {
      COLOR color = new COLOR(c);
      ((Context)context).Engine.Screen2D.Line1.Color = color;
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
    public static Val SetUILine2Color(IContext context, float3 c)
    {
      COLOR color = new COLOR(c);
      ((Context)context).Engine.Screen2D.Line2.Color = color;
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
    public static Val SetUILine3Color(IContext context, float3 c)
    {
      COLOR color = new COLOR(c);
      ((Context)context).Engine.Screen2D.Line3.Color = color;
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
    public static Val SetUILine1Text(IContext context, string text)
    {
      ((Context)context).Engine.Screen2D.Line1.Text = text;
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
    public static Val SetUILine2Text(IContext context, string text)
    {
      ((Context)context).Engine.Screen2D.Line2.Text = text;
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
    public static Val SetUILine3Text(IContext context, string text)
    {
      ((Context)context).Engine.Screen2D.Line3.Text = text;
      return Val.TRUE;
    }
  }
}
