using SWEndor.Game.Actors.Data;

namespace SWEndor.Game.Actors.Data
{
  internal struct SpecialData
  {
    // General
    public float Hyperspace;

    public void Init()
    {
      Hyperspace = 0;
    }

    public void Reset()
    {
      Hyperspace = 0;

    }
  }
}

namespace SWEndor.Game.Actors
{
  public partial class ActorInfo
  {
    public float HyperspaceFactor { get { return SpecialData.Hyperspace; }  set { SpecialData.Hyperspace = value; } }
  }
}