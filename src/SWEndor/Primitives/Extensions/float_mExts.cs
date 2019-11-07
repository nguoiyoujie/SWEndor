using MTV3D65;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Primitives.Extensions
{
  public static class float_mExts
  {
    public static TV_2DVECTOR ToVec2(this float2 val)
    {
      return new TV_2DVECTOR(val.x, val.y);
    }

    public static TV_3DVECTOR ToVec3(this float3 val)
    {
      return new TV_3DVECTOR(val.x, val.y, val.z);
    }

    public static COLOR ToColor(this float3 val)
    {
      return new COLOR(val.x, val.y, val.z, 1);
    }

    public static TV_4DVECTOR ToVec4(this float4 val)
    {
      return new TV_4DVECTOR(val.x, val.y, val.z, val.w);
    }

    public static COLOR ToColor(this float4 val)
    {
      return new COLOR(val.x, val.y, val.z, val.w);
    }
  }
}
