namespace Primrose.Primitives
{
  public struct BoundFloat
  {
    public float Min { get; }
    public float Max { get; }

    public BoundFloat(float value1, float value2)
    {
      if (value1 > value2)
      {
        Min = value2;
        Max = value1;
      }
      else
      {
        Min = value1;
        Max = value2;
      }
    }

    public bool Contains(float val)
    {
      return val <= Max && val >= Min;
    }
  }
}
