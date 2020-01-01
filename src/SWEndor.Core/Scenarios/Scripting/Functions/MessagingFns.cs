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
    /// <param name="text"></param>
    /// <param name="expireTime"></param>
    /// <param name="vec_color"></param>
    /// <returns>NULL</returns>
    public static Val MessageText(Context context, string text, float expireTime, float3 vec_color)
    {
      COLOR color = new COLOR(vec_color);
      context.Engine.Screen2D.MessageText(text, expireTime, color);
      return Val.NULL;
    }

    /// <summary>
    /// Prints a message text on the center of the screen
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="text"></param>
    /// <param name="expireTime"></param>
    /// <param name="vec_color"></param>
    /// <returns>NULL</returns>
    public static Val MessageText(Context context, string text, float expireTime, float3 vec_color, int priority)
    {
      COLOR color = new COLOR(vec_color);
      context.Engine.Screen2D.MessageText(text, expireTime, color, priority);
      return Val.NULL;
    }
  }
}
