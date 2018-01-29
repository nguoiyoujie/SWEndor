using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public static class Utilities
  {
    public static TV_3DVECTOR GetRotation(TV_3DVECTOR direction)
    {
      float x = Engine.Instance().TVMathLibrary.Direction2Ang(-direction.y, Engine.Instance().TVMathLibrary.TVVec2Length(new TV_2DVECTOR(direction.z, direction.x)));
      float y = Engine.Instance().TVMathLibrary.Direction2Ang(direction.x, direction.z);

      return new TV_3DVECTOR(x, y, 0);
    }

    public static TV_3DVECTOR GetDirection(TV_3DVECTOR rotation)
    {
      float x = (float)(Math.Cos(rotation.x / 180 * Globals.PI) * Math.Sin(rotation.y / 180 * Globals.PI));
      float y = -(float)Math.Sin(rotation.x / 180 * Globals.PI);
      float z = (float)(Math.Cos(rotation.x / 180 * Globals.PI) * Math.Cos(rotation.y / 180 * Globals.PI));

      return new TV_3DVECTOR(x, y, z);
    }

    public static string ToString(TV_3DVECTOR vector)
    {
      return string.Format("(VEC:{0},{1},{2})", vector.x, vector.y, vector.z);
    }

  }
}
