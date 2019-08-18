namespace SWEndor.Actors.Data
{
  public struct TimedLifeData
  {
    public bool OnTimedLife;
    public float TimedLife;

    public TimedLifeData(bool enabled = false, float time = 100)
    {
      OnTimedLife = enabled;
      TimedLife = time;
    }

    public void CopyFrom(TimedLifeData src)
    {
      OnTimedLife = src.OnTimedLife;
      TimedLife = src.TimedLife;
    }

    public void Reset()
    {
      OnTimedLife = false;
      TimedLife = 100;
    }
  }
}
