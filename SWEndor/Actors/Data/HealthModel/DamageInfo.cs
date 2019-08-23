using MTV3D65;

namespace SWEndor.Actors
{
  public class DamageInfo
  {
    public readonly ActorInfo Source;
    public readonly float Value;
    public readonly DamageType Type;
    public readonly TV_3DVECTOR Position;

    public DamageInfo(ActorInfo source, float value, DamageType type = DamageType.NORMAL, TV_3DVECTOR position = default(TV_3DVECTOR))
    {
      Source = source;
      Value = value;
      Type = type;
      Position = position;
    }
  }
}