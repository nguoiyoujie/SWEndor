namespace SWEndor.Actors.Components
{
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

    public void Initialize(ActorInfo actor)
    {
      actor.MoveComponent.ApplyZBalance = false;
      D_spin_r = actor.Engine.Random.Next(MinRate, MaxRate); // assumed D_spin_min_rate < D_spin_max_rate
      if (actor.Engine.Random.NextDouble() > 0.5f)
        D_spin_r = -D_spin_r;
    }

    public void Update(ActorInfo actor, float time)
    {
      float rotZ = D_spin_r * time;
      actor.Rotate(0, 0, rotZ);
      actor.MoveComponent.ResetTurn(); // force only forward
    }
  }
}
