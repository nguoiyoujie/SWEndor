using Primitives.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct LookData
  {
    private const string sNone = "";

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue(sNone, "LookFrom")]
    public float3 LookFrom;

    [INIValue(sNone, "LookAt")]
    public float3 LookAt;
#pragma warning restore 0649
  }
}
