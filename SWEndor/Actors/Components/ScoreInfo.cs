using SWEndor.Actors;
using System.Collections.Generic;

namespace SWEndor
{
  public struct ScoreInfo
  {
    public class ScoreComparer : IComparer<ScoreInfo>
    {
      public int Compare(ScoreInfo x, ScoreInfo y)
      {
        return x.Score.CompareTo(y.Score);
      }
    }

    public static readonly ScoreInfo Player = new ScoreInfo(null);

    private readonly ActorInfo Actor;
    public float Score;
    public int Hits;
    public int HitsOnFighters;
    public int Kills;
    public int Deaths;
    public float DamageTaken;
    public Dictionary<string, int> KillsByType;

    public ScoreInfo(ActorInfo actor)
    {
      Actor = actor;
      Score = 0;
      Hits = 0;
      HitsOnFighters = 0;
      Kills = 0;
      Deaths = 0;
      DamageTaken = 0;
      KillsByType = new Dictionary<string, int>();
    }

    public void Reset()
    {
      Score = 0;
      Hits = 0;
      HitsOnFighters = 0;
      Kills = 0;
      Deaths = 0;
      DamageTaken = 0;
      KillsByType = new Dictionary<string, int>();
    }
  }
}
