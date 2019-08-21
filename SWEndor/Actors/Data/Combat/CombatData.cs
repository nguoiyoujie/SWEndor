using System.Collections.Generic;

namespace SWEndor.Actors.Data
{
  public struct CombatData
  {
    public bool IsCombatObject;
    public bool HitWhileDyingLeadsToDeath;
    //public float DamageModifier;
    //public float CollisionDamageModifier;

    //private Dictionary<DamageType, float> DamageModifiers; //move Modifiers to combat data or something

    public CombatData Disabled { get { return new CombatData(false, false); } }
    public CombatData DefaultFighter { get { return new CombatData(true, true); } }
    public CombatData DefaultShip { get { return new CombatData(true, false); } }

    public CombatData(bool enabled, bool hitdeath)//, float DmgMod, float collisionDmgMod)
    {
      IsCombatObject = enabled;
      HitWhileDyingLeadsToDeath = hitdeath;
      //DamageModifier = DmgMod;
      //CollisionDamageModifier = collisionDmgMod;

      //DamageModifiers = new Dictionary<DamageType, float>();
    }

    public void CopyFrom(CombatData src)
    {
      IsCombatObject = src.IsCombatObject;
      HitWhileDyingLeadsToDeath = src.HitWhileDyingLeadsToDeath;
      //DamageModifier = src.DamageModifier;
      //CollisionDamageModifier = src.CollisionDamageModifier;
    }

    public void Reset()
    {
      IsCombatObject = false;
      //DamageModifier = 1;
      //CollisionDamageModifier = 1;
      HitWhileDyingLeadsToDeath = false;
    }
  }
}
