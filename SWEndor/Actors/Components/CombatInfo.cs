namespace SWEndor.Actors.Components
{
  public struct CombatInfo
  {
    private readonly ActorInfo Actor;
    public bool IsCombatObject;
    public bool OnTimedLife;
    public float TimedLife;
    public float Strength;
    public float MaxStrength;
    public float DamageModifier;
    public bool HitWhileDyingLeadsToDeath;

    public CombatInfo(ActorInfo actor)
    {
      Actor = actor;

      IsCombatObject = false;
      OnTimedLife = false;
      TimedLife = 100;
      Strength = 1;
      MaxStrength = 1;
      DamageModifier = 1;
      HitWhileDyingLeadsToDeath = false;
    }

    public void Reset()
    {
      IsCombatObject = false;
      OnTimedLife = false;
      TimedLife = 100;
      Strength = 1;
      MaxStrength = 1;
      DamageModifier = 1;
      HitWhileDyingLeadsToDeath = false;
    }

    private void Dying()
    {
      if (Actor.ActorState != ActorState.DYING && Actor.ActorState != ActorState.DEAD)
        Actor.ActorState = ActorState.DYING;
      else if (Actor.ActorState == ActorState.DYING)
        Actor.ActorState = ActorState.DEAD;
    }

    public void Process()
    {
      // Expired
      if (OnTimedLife)
      {
        if (TimedLife < 0f)
          Dying();
        TimedLife -= Game.Instance().TimeSinceRender;
      }

      // Strength
      if (IsCombatObject && Strength <= 0f)
        Dying();
    }
  }
}
