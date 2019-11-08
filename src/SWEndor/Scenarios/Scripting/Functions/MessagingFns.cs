using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using Primrose.Primitives.ValueTypes;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class MessagingFns
  {
    /// <summary>
    /// Prints a message text on the center of the screen
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     STRING text, 
    ///     INT expiretime, 
    ///     FLOAT3 color,
    ///     optional INT priority
    /// </param>
    /// <returns>NULL</returns>
    public static Val MessageText(Context context, params Val[] ps)
    {
      string text = (string)ps[0];
      float expiretime = (int)ps[1];
      COLOR color = new COLOR((float3)ps[2]);

      if (ps.Length <= 3)
        context.Engine.Screen2D.MessageText(text, expiretime, color);
      else
      {
        int priority = (int)ps[3];
        context.Engine.Screen2D.MessageText(text, expiretime, color, priority);
      }
      return Val.NULL;
    }
  }
}
