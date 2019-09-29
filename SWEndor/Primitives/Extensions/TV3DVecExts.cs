using MTV3D65;
using System;

namespace SWEndor.Primitives.Extensions
{
  public static class TV3DVecExts
  {
    public static string Str(this TV_3DVECTOR vector)
    {
      return "(VEC:{0},{1},{2})".F(vector.x, vector.y, vector.z);
    }

    public static TV_3DVECTOR ConvertDirToRot(this TV_3DVECTOR direction)
    {
      float x = Globals.Engine.TrueVision.TVMathLibrary.Direction2Ang(-direction.y, Globals.Engine.TrueVision.TVMathLibrary.TVVec2Length(new TV_2DVECTOR(direction.z, direction.x)));
      float y = Globals.Engine.TrueVision.TVMathLibrary.Direction2Ang(direction.x, direction.z);

      return new TV_3DVECTOR(x, y, 0);
    }

    public static TV_3DVECTOR ConvertRotToDir(this TV_3DVECTOR rotation)
    {
      float x = (float)(Math.Cos(rotation.x / 180 * Globals.PI) * Math.Sin(rotation.y / 180 * Globals.PI));
      float y = -(float)Math.Sin(rotation.x / 180 * Globals.PI);
      float z = (float)(Math.Cos(rotation.x / 180 * Globals.PI) * Math.Cos(rotation.y / 180 * Globals.PI));

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
