namespace SWEndor.ActorTypes.Groups
{
  public class Warship : LargeShip
  {
    internal Warship(Factory owner, string name): base(owner, name)
    {
      ZTilt = 7.5f;
      ZNormFrac = 0.005f;

      RadarType = RadarType.RECTANGLE_GIANT;
    }
  }
}

