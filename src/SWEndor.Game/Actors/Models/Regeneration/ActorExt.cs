using Primrose.Primitives;

namespace SWEndor.Game.Actors
{
  public partial class ActorInfo
  {
    public void Regenerate(float time)
    {
      using (ScopeCounters.Acquire(Scope))
        Regen.Process(this, time);
    }

    public bool NoRegen { get { return Regen.NoRegen; } set { Regen.NoRegen = value; } }
    public float SelfRegenRate { get { return Regen.SelfRegenRate; } set { Regen.SelfRegenRate = value; } }
    public float SiblingRegenRate { get { return Regen.SiblingRegenRate; } set { Regen.SiblingRegenRate = value; } }
    public float ParentRegenRate { get { return Regen.ParentRegenRate; } set { Regen.ParentRegenRate = value; } }
    public float ChildRegenRate { get { return Regen.ChildRegenRate; } set { Regen.ChildRegenRate = value; } }
  }
}
