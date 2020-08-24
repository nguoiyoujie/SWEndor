using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct RegenData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public bool NoRegen;

    [INIValue]
    public float SelfRegenRate;

    [INIValue]
    public float ParentRegenRate;

    [INIValue]
    public float ChildRegenRate;

    [INIValue]
    public float SiblingRegenRate;
#pragma warning restore 0649
  }
}
