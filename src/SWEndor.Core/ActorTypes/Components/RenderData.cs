using Primitives.FileFormat.INI;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Components
{
  internal struct RenderData
  {
    private const string sRender = "Render";

    public bool EnableDistanceCull { get { return CullDistance > 0; } }

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue(sRender, "CullDistance")]
    public float CullDistance;

    [INIValue(sRender, "RadarSize")]
    public float RadarSize;

    [INIValue(sRender, "RadarType")]
    public RadarType RadarType;

    [INIValue(sRender, "AlwaysShowInRadar")]
    public bool AlwaysShowInRadar;

    [INIValue(sRender, "ZEaseInTime")]
    public float ZEaseInTime;

    [INIValue(sRender, "ZEaseInDelay")]
    public float ZEaseInDelay;

    [INIValue(sRender, "RemapLaserColor")]
    public bool RemapLaserColor;
#pragma warning restore 0649 

    public readonly static RenderData Default =
        new RenderData
        {
          CullDistance = 20000,
          ZEaseInTime = -1,
        };
  }
}

