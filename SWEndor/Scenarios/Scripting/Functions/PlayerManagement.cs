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
        PlayerInfo.Instance().ActorID = -1;
        return false;
      }

      if (!ActorInfo.Factory.Exists(id))
        return false;

      PlayerInfo.Instance().ActorID = id;
      return true;
    }

    public static object GetActor(Context context, object[] ps)
    {
      return PlayerInfo.Instance().ActorID;
    }

    public static object RequestSpawn(Context context, object[] ps)
    {
      PlayerInfo.Instance().RequestSpawn = true;
      return true;
    }

    public static object SetMovementEnabled(Context context, object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      PlayerInfo.Instance().IsMovementControlsEnabled = enabled;
      return true;
    }

    public static object SetAI(Context context, object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      PlayerInfo.Instance().PlayerAIEnabled = enabled;
      return true;
    }

    public static object SetLives(Context context, object[] ps)
    {
      PlayerInfo.Instance().Lives = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object DecreaseLives(Context context, object[] ps)
    {
      PlayerInfo.Instance().Lives--;
      return true;
    }

    public static object SetScorePerLife(Context context, object[] ps)
    {
      PlayerInfo.Instance().ScorePerLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object SetScoreForNextLife(Context context, object[] ps)
    {
      PlayerInfo.Instance().ScoreForNextLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object ResetScore(Context context, object[] ps)
    {
      PlayerInfo.Instance().Score.Reset();
      return true;
    }
  }
}
