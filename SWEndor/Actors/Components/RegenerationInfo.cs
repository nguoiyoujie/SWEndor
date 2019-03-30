using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public class RegenerationInfo
  {
    private ActorInfo Actor;
    public bool AllowRegen = true;
    public float SelfRegenRate = 0;
    public float ParentRegenRate = 0;
    public float ChildRegenRate = 0;
    public float RelativeRegenRate = 0;

    public RegenerationInfo(ActorInfo actor)
    {
      Actor = actor;
    }
    public void Process()
    {
      // Regen
      Regenerate(Actor, SelfRegenRate);

      if (ParentRegenRate != 0)
        foreach (ActorInfo p in Actor.GetAllParents())
          Regenerate(p, ParentRegenRate);

      if (ChildRegenRate != 0)
        foreach (ActorInfo c in Actor.GetAllChildren())
          Regenerate(c, ChildRegenRate);

      if (RelativeRegenRate != 0)
        foreach (ActorInfo r in Actor.GetAllRelatives())
          Regenerate(r, RelativeRegenRate);
    }

    private void Regenerate(ActorInfo a, float amount)
    {
      if (a.RegenerationInfo.AllowRegen
        && a.ActorState != ActorState.DYING 
        && a.ActorState != ActorState.DEAD)
      {
        a.CombatInfo.Strength += amount;
        if (a.CombatInfo.Strength > a.TypeInfo.MaxStrength)
          a.CombatInfo.Strength = a.TypeInfo.MaxStrength;
      }
    }
  }
}
