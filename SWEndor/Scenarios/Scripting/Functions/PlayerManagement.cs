using SWEndor.Actors;
using SWEndor.Player;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class PlayerManagement
  {
    public static object AssignActor(Context context, object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      if (id < 0)
      {
        Globals.Engine.PlayerInfo.ActorID = -1;
        return false;
      }

      if (!ActorInfo.Factory.Exists(id))
        return false;

      Globals.Engine.PlayerInfo.ActorID = id;
      return true;
    }

    public static object GetActor(Context context, object[] ps)
    {
      return Globals.Engine.PlayerInfo.ActorID;
    }

    public static object RequestSpawn(Context context, object[] ps)
    {
      Globals.Engine.PlayerInfo.RequestSpawn = true;
      return true;
    }

    public static object SetMovementEnabled(Context context, object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      Globals.Engine.PlayerInfo.IsMovementControlsEnabled = enabled;
      return true;
    }

    public static object SetAI(Context context, object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      Globals.Engine.PlayerInfo.PlayerAIEnabled = enabled;
      return true;
    }

    public static object SetLives(Context context, object[] ps)
    {
      Globals.Engine.PlayerInfo.Lives = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object DecreaseLives(Context context, object[] ps)
    {
      Globals.Engine.PlayerInfo.Lives--;
      return true;
    }

    public static object SetScorePerLife(Context context, object[] ps)
    {
      Globals.Engine.PlayerInfo.ScorePerLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object SetScoreForNextLife(Context context, object[] ps)
    {
      Globals.Engine.PlayerInfo.ScoreForNextLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object ResetScore(Context context, object[] ps)
    {
      Globals.Engine.PlayerInfo.Score.Reset();
      return true;
    }
  }
}
