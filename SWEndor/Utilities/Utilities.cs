using System;
using MTV3D65;
using SWEndor.Core;

namespace SWEndor
{
  public static class Utilities
  {
    public static TV_3DVECTOR GetRelativePositionFUR(Engine engine, TV_3DVECTOR position, TV_3DVECTOR rotation, float front, float up, float right, bool uselocal = false)
    {
      TV_3DVECTOR ret = new TV_3DVECTOR();

      engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, front), rotation.y, rotation.x, rotation.z);
      ret += position;
      return ret;
    }

    public static TV_3DVECTOR GetRelativePositionXYZ(Engine engine, TV_3DVECTOR position, TV_3DVECTOR rotation, float x, float y, float z, bool uselocal = false)
    {
      TV_3DVECTOR ret = new TV_3DVECTOR();
      engine.TrueVision.TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(x, y, z), rotation.y, rotation.x, rotation.z);
      ret += position;
      return ret;
    }
  }
}
