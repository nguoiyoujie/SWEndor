using Primrose.Expressions;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class ScoreFns
  {
    /// <summary>
    /// Sets the score increment required for the player to receive 1 life
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT score
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetScorePerLife(IContext context, int score)
    {
      ((Context)context).Engine.PlayerInfo.ScorePerLife = score;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the next score required for the player to receive 1 life
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     FLOAT score
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetScoreForNextLife(IContext context, int score)
    {
      ((Context)context).Engine.PlayerInfo.ScoreForNextLife = score;
      return Val.NULL;
    }

    /// <summary>
    /// Resets the player score. This also resets the player statistics such as kill/hit/damage/death count
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     NONE
    /// </param>
    /// <returns>NULL</returns>
    public static Val ResetScore(IContext context)
    {
      ((Context)context).Engine.PlayerInfo.Score.Reset();
      return Val.NULL;
    }
  }
}
