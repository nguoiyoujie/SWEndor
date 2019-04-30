namespace SWEndor.Actors.Components
{
  public enum CombatEventType
  {
    TIMEDECAY,
    SET_STRENGTH,
    DAMAGE,
    COLLISIONDAMAGE,
    RECOVER,
    DYING
  }

  public class CombatInfo
  {
    private readonly ActorInfo Actor;
    public bool IsCombatObject;
    public bool OnTimedLife;
    public float TimedLife;
    public float Strength { get; private set; }
    public float MaxStrength;
    public float CollisionDamageModifier;
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
      CollisionDamageModifier = 1;
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
      CollisionDamageModifier = 1;
      HitWhileDyingLeadsToDeath = false;
    }

    public void onNotify(CombatEventType type, float parameter)
    {
      switch (type)
      {
        case CombatEventType.TIMEDECAY:
          TimedLife -= Actor.Engine.Game.TimeSinceRender;
          if (TimedLife < 0f)
            Dying();
          break;

        case CombatEventType.SET_STRENGTH:
          Strength = parameter;
          if (IsCombatObject && Strength <= 0)
            Dying();
          break;

        case CombatEventType.DAMAGE:
          Strength -= parameter * DamageModifier;
          if (IsCombatObject && Strength <= 0)
            Dying();
          break;

        case CombatEventType.COLLISIONDAMAGE:
          Strength -= parameter * CollisionDamageModifier;
          if (IsCombatObject && Strength <= 0)
            Dying();
          break;

        case CombatEventType.RECOVER:
          Strength += parameter;
          if (Strength >= MaxStrength)
            Strength = MaxStrength;
          break;

        case CombatEventType.DYING:
          Dying();
          break;
      }
    } 

    private void Dying()
    {
      if (!Actor.ActorState.IsDyingOrDead())
        Actor.ActorState = ActorState.DYING;
      else if (Actor.ActorState.IsDying())
        Actor.ActorState = ActorState.DEAD;
    }

    public void Process()
    {
      // Expired
      if (OnTimedLife)
        onNotify(CombatEventType.TIMEDECAY, Actor.Engine.Game.TimeSinceRender);
    }
  }
}
