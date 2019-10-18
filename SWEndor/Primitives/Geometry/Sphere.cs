using MTV3D65;

namespace SWEndor.Primitives.Geometry
{
  public struct Sphere
  {
    public float X { get; }
    public float Y { get; }
    public float Z { get; }
    public float R { get; }
    public TV_3DVECTOR Position { get { return new TV_3DVECTOR(X, Y, Z); } }

    public Sphere(float x, float y, float z, float r)
    {
      X = x;
      Y = y;
      Z = z;
      R = r;
    }

    public Sphere(TV_3DVECTOR p, float r)
    {
      X = p.x;
      Y = p.y;
      Z = p.z;
      R = r;
    }

    public bool Contains(float x, float y, float z)
    {
      float dx = X - x;
      float dy = Y - y;
      float dz = Z - z;
      return dx * dx + dy * dy + dz * dz <= R * R;
    }

    public bool Contains(TV_3DVECTOR p)
    {
      return Contains(p.x, p.y, p.z);
    }
  }
}
