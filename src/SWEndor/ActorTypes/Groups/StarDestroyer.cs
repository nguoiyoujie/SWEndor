using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class StarDestroyer : LargeShip
  {
    internal StarDestroyer(Factory owner, string id, string name) : base(owner, id, name)
    {
      MoveLimitData.ZTilt = 2.5f;
      MoveLimitData.ZNormFrac = 0.011f;

      RenderData.RadarType = RadarType.TRIANGLE_GIANT;
    }
  }
}

