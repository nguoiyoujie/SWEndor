using System.Collections.Generic;

namespace SWEndor
{
  public class ScoreInfo
  {
    public class ScoreComparer : IComparer<ScoreInfo>
    {
      public int Compare(ScoreInfo x, ScoreInfo y)
      {
        return x.Score.CompareTo(y.Score);
       }
    }

    public static ThreadSafeList<ScoreInfo> Scores = new ThreadSafeList<ScoreInfo>();

    public ScoreInfo(string name)
    {
      Name = name;
      Scores.Add(this);
    }

    public void Reset()
    {
      Score = 0;
      Hits = 0;
      HitsOnFighters = 0;
      Kills = 0;
      Deaths = 0;
      DamageTaken = 0;
      Dictionary<string, int> KillsByType = new Dictionary<string, int>();

      if (!Scores.Contains(this))
        Scores.Add(this);
    }

    public float Score = 0;
    public readonly string Name;

    public int Hits = 0;
    public int HitsOnFighters = 0;
    public int Kills = 0;
    public int Deaths = 0;
    public float DamageTaken = 0;
    public Dictionary<string, int> KillsByType = new Dictionary<string, int>();
  }
}
