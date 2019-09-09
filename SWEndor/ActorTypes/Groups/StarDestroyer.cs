﻿namespace SWEndor.ActorTypes.Groups
{
  public class StarDestroyer : LargeShip
  {
    internal StarDestroyer(Factory owner, string name) : base(owner, name)
    {
      MoveLimitData.ZTilt = 2.5f;
      MoveLimitData.ZNormFrac = 0.011f;

      RenderData.RadarType = RadarType.TRIANGLE_GIANT;
    }
  }
}

