using MTV3D65;

namespace Primrose.Primitives.Geometry
{
  public struct Box
  {
    public BoundFloat X { get; }
    public BoundFloat Y { get; }
    public BoundFloat Z { get; }

    public Box(TV_3DVECTOR p0, TV_3DVECTOR p1)
    {
      X = new BoundFloat(p0.x, p1.x);
      Y = new BoundFloat(p0.y, p1.y);
      Z = new BoundFloat(p0.z, p1.z);
    }

    public Box(float x0, float x1, float y0, float y1, float z0, float z1)
    {
      X = new BoundFloat(x0, x1);
      Y = new BoundFloat(y0, y1);
      Z = new BoundFloat(z0, z1);
    }

    public bool ContainsPoint(TV_3DVECTOR pt)
    {
      return X.Contains(pt.x)
          && Y.Contains(pt.y)
          && Z.Contains(pt.z);
    }
  }
}
