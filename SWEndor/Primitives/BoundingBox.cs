using MTV3D65;

namespace SWEndor.Primitives
{
  public struct BoundingBox
  {
    public readonly BoundingValue X;
    public readonly BoundingValue Y;
    public readonly BoundingValue Z;

    public BoundingBox(TV_3DVECTOR p0, TV_3DVECTOR p1)
    {
      X = new BoundingValue(p0.x, p1.x);
      Y = new BoundingValue(p0.y, p1.y);
      Z = new BoundingValue(p0.z, p1.z);
    }

    public BoundingBox(float x0, float x1, float y0, float y1, float z0, float z1)
    {
      X = new BoundingValue(x0, x1);
      Y = new BoundingValue(y0, y1);
      Z = new BoundingValue(z0, z1);
    }

    public bool ContainsPoint(TV_3DVECTOR pt)
    {
      return X.ContainsValue(pt.x)
          && Y.ContainsValue(pt.y)
          && Z.ContainsValue(pt.z);
    }
  }
}
