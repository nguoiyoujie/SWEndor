using MTV3D65;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Primitives.Extensions
{
  public static class float_mExts
  {
    public static float2 ToFloat2(this TV_2DVECTOR val)
    {
      return new float2(val.x, val.y);
    }

    public static float3 ToFloat3(this TV_3DVECTOR val)
    {
      return new float3(val.x, val.y, val.z);
    }

    public static float4 ToFloat4(this TV_4DVECTOR val)
    {
      return new float4(val.x, val.y, val.z, val.w);
    }

    public static float3 ToFloat3(this COLOR val)
    {
      return new float3(val.fR, val.fG, val.fB);
    }

    public static float4 ToFloat4(this COLOR val)
    {
      return new float4(val.fR, val.fG, val.fB, val.fA);
    }

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
