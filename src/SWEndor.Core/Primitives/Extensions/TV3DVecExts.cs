using MTV3D65;
using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.Primitives.Extensions
{
  public static class TV3DVecExts
  {
    public static string Str(this TV_3DVECTOR vector)
    {
      return "(VEC:{0},{1},{2})".F(vector.x, vector.y, vector.z);
    }

    public static TV_3DVECTOR ConvertDirToRot(this TV_3DVECTOR direction, TVMathLibrary math)
    {
      float x = math.Direction2Ang(-direction.y, math.TVVec2Length(new TV_2DVECTOR(direction.z, direction.x)));
      float y = math.Direction2Ang(direction.x, direction.z);

      // Less performant alternative
      //float x = (float)Math.Atan2(-direction.y, Math.Sqrt(direction.z * direction.z + direction.x * direction.x)) * Globals.Rad2Deg;
      //float y = (float)Math.Atan2(direction.x, direction.z) * Globals.Rad2Deg;

      return new TV_3DVECTOR(x, y, 0);
    }

    public static TV_3DVECTOR ConvertRotToDir(this TV_3DVECTOR rotation)
    {
      float x = (float)(Math.Cos(rotation.x * Globals.Deg2Rad) * Math.Sin(rotation.y * Globals.Deg2Rad));
      float y = -(float)Math.Sin(rotation.x * Globals.Deg2Rad);
      float z = (float)(Math.Cos(rotation.x * Globals.Deg2Rad) * Math.Cos(rotation.y * Globals.Deg2Rad));

      return new TV_3DVECTOR(x, y, z);
    }

    public static void Clamp(ref TV_3DVECTOR point, TV_3DVECTOR minBound, TV_3DVECTOR maxBound)
    {
      TV_3DVECTOR ret = point - minBound;
      TV_3DVECTOR vol = maxBound - minBound;
      if (ret.x < 0)
        ret.x = 0;
      else if (ret.x > vol.x)
        ret.x = vol.x;

      if (ret.y < 0)
        ret.y = 0;
      else if (ret.y > vol.y)
        ret.y = vol.y;

      if (ret.z < 0)
        ret.z = 0;
      else if (ret.z > vol.z)
        ret.z = vol.z;

      ret += minBound;
    }

    public static TV_3DVECTOR ModulusBox(TV_3DVECTOR point, TV_3DVECTOR minBound, TV_3DVECTOR maxBound)
    {
      TV_3DVECTOR ret = point - minBound;
      TV_3DVECTOR vol = maxBound - minBound;
      ret.x %= vol.x;
      if (ret.x < 0)
        ret.x += vol.x;

      ret.y %= vol.y;
      if (ret.y < 0)
        ret.y += vol.y;

      ret.z %= vol.z;
      if (ret.z < 0)
        ret.z += vol.z;

      ret += minBound;

      return ret;
    }
  }
}
