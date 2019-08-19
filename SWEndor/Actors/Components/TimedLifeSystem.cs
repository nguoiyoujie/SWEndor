namespace SWEndor.Actors.Components
{
  public static class TimedLifeSystem
  {
    internal static void Activate(Engine engine, ActorInfo actor, float time)
    {
      engine.TimedLifeDataSet.OnTimedLife_set(actor, true);
      engine.TimedLifeDataSet.TimedLife_set(actor, time);
    }

    internal static void ReduceTimerTo(Engine engine, ActorInfo actor, float time)
    {
      if (engine.TimedLifeDataSet.TimedLife_get(actor) > time)
        engine.TimedLifeDataSet.TimedLife_set(actor, time);
    }
  }
}
