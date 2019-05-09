namespace SWEndor.ActorTypes
{
  /// <summary>
  /// The 'type' of an Actor for drawing on the radar
  /// </summary>
  public enum RadarType
  {
    /// <summary>
    /// Draw nothing
    /// </summary>
    NULL = 0,

    /// <summary>
    /// Draw a trail line from previous position to current position
    /// </summary>
    TRAILLINE,

    /// <summary>
    /// Draw an unfilled square
    /// </summary>
    HOLLOW_SQUARE,

    /// <summary>
    /// Draw an filled square
    /// </summary>
    FILLED_SQUARE,

    /// <summary>
    /// Draw an hollow circle, using 4 points
    /// </summary>
    HOLLOW_CIRCLE_S,

    /// <summary>
    /// Draw an hollow circle, using 12 points
    /// </summary>
    HOLLOW_CIRCLE_M,

    /// <summary>
    /// Draw an hollow circle, using 36 points
    /// </summary>
    HOLLOW_CIRCLE_L,

    /// <summary>
    /// Draw an filled circle, using 4 points
    /// </summary>
    FILLED_CIRCLE_S,

    /// <summary>
    /// Draw an filled circle, using 12 points
    /// </summary>
    FILLED_CIRCLE_M,

    /// <summary>
    /// Draw an filled circle, using 36 points
    /// </summary>
    FILLED_CIRCLE_L,

    /// <summary>
    /// Draw a large rectangle that obeys xz-rotation (capital ships)
    /// </summary>
    RECTANGLE_GIANT,

    /// <summary>
    /// Draw a large triangle that obeys xz-rotation (capital ships - star destroyers)
    /// </summary>
    TRIANGLE_GIANT
  }
}
