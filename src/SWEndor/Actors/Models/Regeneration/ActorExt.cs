using Primrose.Primitives;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public void Regenerate(float time)
    {
      using (ScopeCounters.Acquire(Scope))
        Regen.Process(this, time);
    }

    public bool NoRegen { get { return Regen.NoRegen; } }
  }
}
