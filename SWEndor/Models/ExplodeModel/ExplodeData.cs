namespace SWEndor.Models
{
  public struct ExplodeData
  {
    public string Type;
    public float Rate;
    public float Size;
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