namespace SWEndor.Actors.Data
{
  public struct CombatData
  {
    public bool IsCombatObject;
    public bool HitWhileDyingLeadsToDeath;
    public float DamageModifier;
    public float CollisionDamageModifier;

    public CombatData Disabled { get { return new CombatData(false, false, 0, 0); } }
    public CombatData Immune { get { return new CombatData(true, false, 0, 0); } }
    public CombatData DefaultFighter { get { return new CombatData(true, true, 1, 1); } }
    public CombatData DefaultShip { get { return new CombatData(true, false, 1, 1); } }

    public CombatData(bool enabled, bool hitdeath, float DmgMod, float collisionDmgMod)
    {
      IsCombatObject = enabled;
      HitWhileDyingLeadsToDeath = hitdeath;
      DamageModifier = DmgMod;
      CollisionDamageModifier = collisionDmgMod;
    }

    public void CopyFrom(CombatData src)
    {
      IsCombatObject = src.IsCombatObject;
      HitWhileDyingLeadsToDeath = src.HitWhileDyingLeadsToDeath;
      DamageModifier = src.DamageModifier;
      CollisionDamageModifier = src.CollisionDamageModifier;
    }

    public void Reset()
    {
      IsCombatObject = false;
      DamageModifier = 1;
      CollisionDamageModifier = 1;
      HitWhileDyingLeadsToDeath = false;
    }
  }
}
