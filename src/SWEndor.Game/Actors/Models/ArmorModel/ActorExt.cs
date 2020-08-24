namespace SWEndor.Game.Actors
{
  public partial class ActorInfo
  {
    public void SetArmor(DamageType dmgtype, float value) { Armor.Set(dmgtype, value); }
    public void SetArmorAll(float value) { Armor.SetAll(value); }
    public void RestoreArmor() { Armor.Init(ref TypeInfo.ArmorData); }
    public float GetArmor(DamageType dmgtype) { return Armor.Get(dmgtype); }
  }
}
