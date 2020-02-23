using Primitives.FileFormat.INI;

namespace SWEndor.Models
{
  internal struct ExplodeData
  {
    private const string sNone = "";

    [INIValue(sNone, "Type")]
    public string Type;

    [INIValue(sNone, "Rate")]
    public float Rate;

    [INIValue(sNone, "Size")]
    public float Size;

    [INIValue(sNone, "Trigger")]
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