using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  /// <summary>
  /// Implementation of IDyingMoveComponent that spins the actor along its movement axis
  /// </summary>
  public class DyingSpin : IDyingMoveComponent
  {
    public int MinRate;
    public int MaxRate;
    private float D_spin_r;

    public DyingSpin(int minRate, int maxRate)
    {
      MinRate = minRate;
      MaxRate = maxRate;
    }

    public void Initialize(ActorInfo actor, ref MoveData data)
    {
      data.ApplyZBalance = false;
      D_spin_r = actor.Engine.Random.Next(MinRate, MaxRate); // assumed D_spin_min_rate < D_spin_max_rate
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
