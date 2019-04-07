using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public struct RegenerationInfo
  {
    private readonly ActorInfo Actor;
    public bool AllowRegen;
    public float SelfRegenRate;
    public float ParentRegenRate;
    public float ChildRegenRate;
    public float RelativeRegenRate;

    public RegenerationInfo(ActorInfo actor)
    {
      Actor = actor;
      AllowRegen = true;

      SelfRegenRate = 0;
      ParentRegenRate = 0;
      ChildRegenRate = 0;
      RelativeRegenRate = 0;
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
