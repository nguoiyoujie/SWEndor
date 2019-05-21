namespace SWEndor.Primitives
{
  public struct BoundingValue
  {
    public readonly float Min;
    public readonly float Max;

    public BoundingValue(float value1, float value2)
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

    public bool ContainsValue(float val)
    {
      return val <= Max && val >= Min;
    }
  }
}
