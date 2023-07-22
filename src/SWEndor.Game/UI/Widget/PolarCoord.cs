namespace SWEndor.Game.UI
{
  public struct PolarCoord
  {
    public float Dist;
    public float Angle;

    public XYCoord ToXYCoord
    {
      get
      {
        return new XYCoord
        {
          X = -Dist * LookUp.Sin(Angle, LookUp.Measure.DEGREES),
          Y = Dist * LookUp.Cos(Angle, LookUp.Measure.DEGREES)
        };
      }
    }
  }
}
