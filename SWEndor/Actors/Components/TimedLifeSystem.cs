namespace SWEndor.Actors.Components
{
  public static class TimedLifeSystem
  {
    internal static void Activate(Engine engine, int id, float time)
    {
      engine.TimedLifeDataSet.OnTimedLife_set(id, true);
      engine.TimedLifeDataSet.TimedLife_set(id, time);
    }

    internal static void ReduceTimerTo(Engine engine, int id, float time)
    {
      if (engine.TimedLifeDataSet.TimedLife_get(id) > time)
        engine.TimedLifeDataSet.TimedLife_set(id, time);
    }
  }
}
