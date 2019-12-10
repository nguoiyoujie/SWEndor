using MTV3D65;

namespace SWEndor.Actors
{
  public struct DamageInfo
  {
    public readonly float Value;
    public readonly DamageType Type;
    public readonly TV_3DVECTOR Position;

    public DamageInfo(float value, DamageType type, TV_3DVECTOR position = default(TV_3DVECTOR))
    {
      Value = value;
      Type = type;
      Position = position;
    }
  }
}