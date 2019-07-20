using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public class DyingSpin : IDyingMoveComponent
  {
    private int min;
    private int max;
    private float D_spin_r;

    public DyingSpin(int minRate, int maxRate)
    {
      if (minRate > maxRate)
      {
        min = maxRate;
        max = minRate;
      }
      else
      {
        min = minRate;
        max = maxRate;
      }
    }

    public void Initialize(ActorInfo actor, ref MoveData data)
    {
      data.ApplyZBalance = false;
      D_spin_r = actor.Engine.Random.Next(min, max); // assumed D_spin_min_rate < D_spin_max_rate
      if (actor.Engine.Random.NextDouble() > 0.5f)
        D_spin_r = -D_spin_r;
    }

    public void Update(ActorInfo actor, ref MoveData data, float time)
    {
      float rotZ = D_spin_r * time;
      actor.Rotate(0, 0, rotZ);
      actor.MoveData.ResetTurn(); // force only forward
    }
  }
}
