using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct ScoreData
  {
    private const string sScore = "Score";

    [INIValue(sScore, "PerStrength")]
    public int PerStrength;

    [INIValue(sScore, "DestroyBonus")]
    public int DestroyBonus;

    public ScoreData(int perStrength, int destroyBonus)
    {
      PerStrength = perStrength;
      DestroyBonus = destroyBonus;
    }
  }
}

