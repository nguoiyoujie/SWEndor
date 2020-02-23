using Primitives.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct LookData
  {
    private const string sNone = "";

    [INIValue(sNone, "LookFrom")]
    public float3 LookFrom;

    [INIValue(sNone, "LookAt")]
    public float3 LookAt;
  }
}
