using MTV3D65;
using SWEndor.Actors;
using SWEndor.Player;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class PlayerManagement
  {
    public static object Player_AssignActor(Context context, object[] ps)
    {
      if (ps.Length == 0)
      {
        PlayerInfo.Instance().ActorID = GameScenarioManager.Instance().Scenario.ActiveActor?.ID ?? -1;
        return true;
      }

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

    public static object Player_GetActor(Context context, object[] ps)
    {
      ActorInfo a = GameScenarioManager.Instance().Scenario.ActiveActor;
      return a?.ID;
    }

    public static object Player_RequestSpawn(Context context, object[] ps)
    {
      PlayerInfo.Instance().RequestSpawn = true;
      return true;
    }

    public static object Player_SetMovementEnabled(Context context, object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      PlayerInfo.Instance().IsMovementControlsEnabled = enabled;
      return true;
    }

    public static object Player_SetAI(Context context, object[] ps)
    {
      bool enabled = Convert.ToBoolean(ps[0].ToString());
      PlayerInfo.Instance().PlayerAIEnabled = enabled;
      return true;
    }

    public static object Player_SetLives(Context context, object[] ps)
    {
      PlayerInfo.Instance().Lives = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object Player_SetScorePerLife(Context context, object[] ps)
    {
      PlayerInfo.Instance().ScorePerLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object Player_SetScoreForNextLife(Context context, object[] ps)
    {
      PlayerInfo.Instance().ScoreForNextLife = Convert.ToInt32(ps[0].ToString());
      return true;
    }

    public static object Player_ResetScore(Context context, object[] ps)
    {
      PlayerInfo.Instance().Score.Reset();
      return true;
    }
  }
}
