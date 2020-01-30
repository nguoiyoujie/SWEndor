using Primrose.Expressions;
using SWEndor.Core;

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
    ///     INT actorID
    /// </param>
    /// <returns>TRUE if the assignment is successful, otherwise (i.e. there is no actor having this ID) returns FALSE. If the actorID is negative, always returns TRUE.</returns>
    public static Val AssignActor(IContext context, int actorID)
    {
      Engine e = ((Context)context).Engine;
      if (actorID < 0)
      {
        e.PlayerInfo.ActorID = -1;
        return Val.TRUE;
      }

      if (e.ActorFactory.Get(actorID) == null)
        return Val.FALSE;

      e.PlayerInfo.ActorID = actorID;
      return Val.TRUE;
    }

    /// <summary>
    /// Returns the player actor ID
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>INT representing the actor's ID if an actor is found, otherwise returns -1</returns>
    public static Val GetActor(IContext context)
    {
      return new Val(((Context)context).Engine.PlayerInfo.Actor?.ID ?? -1);
    }

    /// <summary>
    /// Toggles a request to spawn the player. If a spawner belonging to the player's faction exists, it will respawn the player. Otherwise, this has no effect.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>NULL</returns>
    public static Val RequestSpawn(IContext context)
    {
      ((Context)context).Engine.PlayerInfo.RequestSpawn = true;
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
    public static Val SetMovementEnabled(IContext context, bool enabled)
    {
      ((Context)context).Engine.PlayerInfo.SystemLockMovement = !enabled;
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
    public static Val SetAI(IContext context, bool enabled)
    {
      ((Context)context).Engine.PlayerInfo.PlayerAIEnabled = enabled;
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
    public static Val SetLives(IContext context, int lives)
    {
      ((Context)context).Engine.PlayerInfo.Lives = lives;
      return Val.NULL;
    }

    /// <summary>
    /// Decrements 1 life from the player
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>NULL</returns>
    public static Val DecreaseLives(IContext context)
    {
      ((Context)context).Engine.PlayerInfo.Lives--;
      return Val.NULL;
    }
  }
}
