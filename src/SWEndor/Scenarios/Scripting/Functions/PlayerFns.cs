using Primrose.Expressions;
using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class PlayerFns
  {
    /// <summary>
    /// Assigns an actor of this ID the player actor ID. Unassigns the player actor if actor_id is negative.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     INT actor_id
    /// </param>
    /// <returns>TRUE if the assignment is successful, otherwise (i.e. there is no actor having this ID) returns FALSE. If the actor_id is negative, always returns TRUE.</returns>
    public static Val AssignActor(Context context, Val[] ps)
    {
      int id = (int)ps[0];
      if (id < 0)
      {
        context.Engine.PlayerInfo.ActorID = -1;
        return Val.TRUE;
      }

      if (context.Engine.ActorFactory.Get(id) == null)
        return Val.FALSE;

      context.Engine.PlayerInfo.ActorID = id;
      return Val.TRUE;
    }

    /// <summary>
    /// Returns the player actor ID
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     NONE
    /// </param>
    /// <returns>INT representing the actor's ID if an actor is found, otherwise returns -1</returns>
    public static Val GetActor(Context context, Val[] ps)
    {
      return new Val(context.Engine.PlayerInfo.Actor?.ID ?? -1);
    }

    /// <summary>
    /// Toggles a request to spawn the player. If a spawner belonging to the player's faction exists, it will respawn the player. Otherwise, this has no effect.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     NONE
    /// </param>
    /// <returns>NULL</returns>
    public static Val RequestSpawn(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.RequestSpawn = true;
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables the player movement controls 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     BOOL enabled
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetMovementEnabled(Context context, Val[] ps)
    {
      bool enabled = (bool)ps[0];
      context.Engine.PlayerInfo.IsMovementControlsEnabled = enabled;
      return Val.NULL;
    }

    /// <summary>
    /// Enables/disables the player AI
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     BOOL enabled
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetAI(Context context, Val[] ps)
    {
      bool enabled = (bool)ps[0];
      context.Engine.PlayerInfo.PlayerAIEnabled = enabled;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the number of lives for the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     INT lives
    /// </param>
    /// <returns>NULL</returns>
    public static Val SetLives(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.Lives = (int)ps[0];
      return Val.NULL;
    }

    /// <summary>
    /// Decrements 1 life from the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     NONE
    /// </param>
    /// <returns>NULL</returns>
    public static Val DecreaseLives(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.Lives--;
      return Val.NULL;
    }
  }
}
