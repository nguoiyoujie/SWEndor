using MTV3D65;

namespace SWEndor.Primitives
{
  public struct BoundingSphere
  {
    public readonly TV_3DVECTOR Position;
    public readonly float Radius;

    public BoundingSphere(float x, float y, float z, float r)
    {
      Position = new TV_3DVECTOR(x, y, z);
      Radius = r;
    }

    public BoundingSphere(TV_3DVECTOR p, float r)
    {
      Position = new TV_3DVECTOR(p.x, p.y, p.z);
      Radius = r;
    }

    public bool ContainsPoint(TV_3DVECTOR pt)
    {
      TV_3DVECTOR d = Position - pt;
      return d.x * d.x + d.y * d.y + d.z * d.z <= Radius * Radius;
    }
  }
}
