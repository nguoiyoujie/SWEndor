namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public void SetArmor(DamageType dmgtype, float value) { Armor.Set(dmgtype, value); }
    public float GetArmor(DamageType dmgtype) { return Armor.Get(dmgtype); }
  }
}
