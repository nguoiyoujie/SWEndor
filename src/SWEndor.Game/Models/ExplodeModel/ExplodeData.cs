using Primrose.FileFormat.INI;

namespace SWEndor.Game.Models
{
  internal struct ExplodeData
  {
    [INIValue]
    public string Type;

    [INIValue]
    public float Rate;

    [INIValue]
    public float Size;

    [INIValue]
    public ExplodeTrigger Trigger;

    public ExplodeData(string type, float rate, float size, ExplodeTrigger trigger)
    {
      Type = type;
      Rate = rate;
      Size = size;
      Trigger = trigger;
    }
  }
}