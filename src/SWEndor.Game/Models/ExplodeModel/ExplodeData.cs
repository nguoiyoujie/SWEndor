using Primrose.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.Models
{
  internal struct ExplodeData
  {
    [INIValue]
    public string Type;

    [INIValue]
    public float3 PositionOffset;

    [INIValue]
    public float Rate;

    [INIValue]
    public float Size;

    [INIValue]
    public ExplodeTrigger Trigger;

    public ExplodeData(string type, float3 offset, float rate, float size, ExplodeTrigger trigger)
    {
      Type = type;
      PositionOffset = offset;
      Rate = rate;
      Size = size;
      Trigger = trigger;
    }
  }
}