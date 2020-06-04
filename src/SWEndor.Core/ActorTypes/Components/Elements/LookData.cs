using Primitives.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct LookData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public float3 LookFrom;

    [INIValue]
    public float3 LookAt;
#pragma warning restore 0649
  }
}
