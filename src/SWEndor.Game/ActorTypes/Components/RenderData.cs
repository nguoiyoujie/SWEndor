using Primrose.FileFormat.INI;
using SWEndor.Game.Models;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct RenderData
  {
    public bool EnableDistanceCull { get { return CullDistance > 0; } }

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public float CullDistance;

    [INIValue]
    public float RadarSize;

    [INIValue]
    public RadarType RadarType;

    [INIValue]
    public bool AlwaysShowInRadar;

    [INIValue]
    public float ZEaseInTime;

    [INIValue]
    public float ZEaseInDelay;

    [INIValue]
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

