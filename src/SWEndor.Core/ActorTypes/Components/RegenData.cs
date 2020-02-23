using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct RegenData
  {
    private const string sRegen = "Regen";

    [INIValue(sRegen, "NoRegen")]
    public bool NoRegen;

    [INIValue(sRegen, "SelfRegenRate")]
    public float SelfRegenRate;

    [INIValue(sRegen, "ParentRegenRate")]
    public float ParentRegenRate;

    [INIValue(sRegen, "ChildRegenRate")]
    public float ChildRegenRate;

    [INIValue(sRegen, "SiblingRegenRate")]
    public float SiblingRegenRate;
  }
}
