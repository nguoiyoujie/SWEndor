using SWEndor.Scenarios.Scripting.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class PlayerManagement
  {
    public static Val AssignActor(Context context, Val[] ps)
    {
      int id = (int)ps[0];
      if (id < 0)
      {
        context.Engine.PlayerInfo.ActorID = -1;
        return Val.FALSE;
      }

      if (context.Engine.ActorFactory.Get(id) == null)
        return Val.FALSE;

      context.Engine.PlayerInfo.ActorID = id;
      return Val.TRUE;
    }

    public static Val GetActor(Context context, Val[] ps)
    {
      return new Val(context.Engine.PlayerInfo.ActorID);
    }

    public static Val RequestSpawn(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.RequestSpawn = true;
      return Val.TRUE;
    }

    public static Val SetMovementEnabled(Context context, Val[] ps)
    {
      bool enabled = (bool)ps[0];
      context.Engine.PlayerInfo.IsMovementControlsEnabled = enabled;
      return Val.TRUE;
    }

    public static Val SetAI(Context context, Val[] ps)
    {
      bool enabled = (bool)ps[0];
      context.Engine.PlayerInfo.PlayerAIEnabled = enabled;
      return Val.TRUE;
    }

    public static Val SetLives(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.Lives = (int)ps[0];
      return Val.TRUE;
    }

    public static Val DecreaseLives(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.Lives--;
      return Val.TRUE;
    }

    public static Val SetScorePerLife(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.ScorePerLife = (float)ps[0];
      return Val.TRUE;
    }

    public static Val SetScoreForNextLife(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.ScoreForNextLife = (float)ps[0];
      return Val.TRUE;
    }

    public static Val ResetScore(Context context, Val[] ps)
    {
      context.Engine.PlayerInfo.Score.Reset();
      return Val.TRUE;
    }
  }
}
