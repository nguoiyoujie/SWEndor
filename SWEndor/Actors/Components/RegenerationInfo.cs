namespace SWEndor.Actors.Components
{
  public class RegenerationInfo
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

    public void Reset()
    {
      AllowRegen = true;
      SelfRegenRate = 0;
      ParentRegenRate = 0;
      ChildRegenRate = 0;
      RelativeRegenRate = 0;
    }

    public void Process(float time)
    {
      // Regen
      Regenerate(Actor, SelfRegenRate * time);

      if (ParentRegenRate != 0)
        foreach (int p in Actor.GetAllParents())
          Regenerate(p, ParentRegenRate * time);

      if (ChildRegenRate != 0)
        foreach (int c in Actor.GetAllChildren())
          Regenerate(c, ChildRegenRate * time);

      if (RelativeRegenRate != 0)
        foreach (int r in Actor.GetAllRelatives())
          Regenerate(r, RelativeRegenRate * time);
    }

    private void Regenerate(int i, float amount)
    {
      ActorInfo a = Actor.ActorFactory.Get(i);
      Regenerate(a, amount);
    }

    private void Regenerate(ActorInfo a, float amount)
    {
      if (a.RegenerationInfo.AllowRegen && !a.ActorState.IsDyingOrDead())
        a.CombatInfo.onNotify(CombatEventType.RECOVER, amount);
    }
  }
}
