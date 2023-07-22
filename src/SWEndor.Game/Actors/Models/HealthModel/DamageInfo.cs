using MTV3D65;
using SWEndor.Game.Models;

namespace SWEndor.Game.Actors
{
  public struct DamageInfo
  {
    public readonly float StunDuration;
    public readonly float SpecialShieldDamage;
    public readonly int SpecialSystemDamage;
    public readonly float SpecialSystemDamageChanceModifier;
    public readonly float Value;
    public readonly DamageType Type;
    public readonly TV_3DVECTOR Position;

    internal DamageInfo(float value, DamageType type, TV_3DVECTOR position = default(TV_3DVECTOR), DamageSpecialData dmgData = default)
    {
      Value = value;
      Type = type;
      Position = position;
      SpecialShieldDamage = dmgData.ShieldDamage;
      SpecialSystemDamage = dmgData.SystemDamageCount;
      SpecialSystemDamageChanceModifier = dmgData.SystemDamageChanceModifier;
      StunDuration = dmgData.StunDuration;
    }
  }
}