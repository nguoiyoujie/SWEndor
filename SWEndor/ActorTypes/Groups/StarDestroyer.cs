namespace SWEndor.ActorTypes.Groups
{
  public class StarDestroyer : LargeShip
  {
    internal StarDestroyer(Factory owner, string name) : base(owner, name)
    {
      ZTilt = 2.5f;
      ZNormFrac = 0.011f;

      RadarType = RadarType.TRIANGLE_GIANT;
    }
  }
}

