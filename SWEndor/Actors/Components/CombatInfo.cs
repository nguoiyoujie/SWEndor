namespace SWEndor.Actors.Components
{
  public class CombatInfo
  {
    private ActorInfo Actor;
    public bool IsCombatObject = false;
    public bool OnTimedLife = false;
    public float TimedLife = 100;
    public float Strength = 1;
    public float MaxStrength = 1;
    public float DamageModifier = 1;

    public CombatInfo(ActorInfo actor)
    {
      Actor = actor;
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
