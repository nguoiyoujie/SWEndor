using System;

namespace SWEndor.Game.UI
{
  public struct XYCoord
  {
    public float X;
    public float Y;

    public PolarCoord ToPolarCoord
    {
      get
      {
        return new PolarCoord
        {
          Angle = (float)Math.Atan2(X, Y) * Globals.Rad2Deg + 180,
          Dist = (float)Math.Sqrt(X * X + Y * Y)
        };
      }
    }
  }
}
