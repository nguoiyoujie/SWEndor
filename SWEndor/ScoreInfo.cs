using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class ScoreInfo
  {
    public ScoreInfo()
    {
    }

    public float Score = 0;

    public int Hits = 0;
    public int HitsOnFighters = 0;
    public int Kills = 0;
    public int Deaths = 0;
    public float DamageTaken = 0;
    public Dictionary<string, int> KillsByType = new Dictionary<string, int>();
  }
}
