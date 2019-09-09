namespace SWEndor.ActorTypes.Groups
{
  public class Warship : LargeShip
  {
    internal Warship(Factory owner, string name): base(owner, name)
    {
      MoveLimitData.ZTilt = 7.5f;
      MoveLimitData.ZNormFrac = 0.005f;

      RenderData.RadarType = RadarType.RECTANGLE_GIANT;
    }
  }
}

