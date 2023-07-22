using Primrose.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.Models
{
  internal struct ParticleData
  {
    [INIValue]
    public string Type;

    [INIValue]
    public float3 PositionOffset;

    [INIValue]
    public float Size;

    [INIValue]
    public ExplodeTrigger Trigger;

    public ParticleData(string type, float3 offset, float size, ExplodeTrigger trigger)
    {
      Type = type;
      PositionOffset = offset;
      Size = size;
      Trigger = trigger;
    }
  }
}