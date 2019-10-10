using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class Warship : LargeShip
  {
    internal Warship(Factory owner, string id, string name) : base(owner, id, name)
    {
      MoveLimitData.ZTilt = 7.5f;
      MoveLimitData.ZNormFrac = 0.005f;

      RenderData.RadarType = RadarType.RECTANGLE_GIANT;
    }
  }
}

