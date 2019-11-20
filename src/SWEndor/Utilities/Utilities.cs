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

    public static TV_3DVECTOR RotateXY(Engine engine, TV_3DVECTOR org, float x, float y)
    {
      // Hack: Using an existing object to perform rotation calculations, then restore it.
      TVCamera cam = engine.PlayerCameraInfo.Camera;
      TV_3DVECTOR o = cam.GetRotation();
      cam.SetRotation(org.x, org.y, org.z);
      cam.RotateX(x);
      cam.RotateY(y);
      TV_3DVECTOR ret = cam.GetRotation();
      cam.SetRotation(o.x, o.y, o.z);
      return ret;
    }

    public static TV_3DVECTOR RotateXYZ(Engine engine, TV_3DVECTOR org, float x, float y, float z)
    {
      // Hack: Using an existing object to perform rotation calculations, then restore it.
      TVCamera cam = engine.PlayerCameraInfo.Camera;
      TV_3DVECTOR o = cam.GetRotation();
      cam.SetRotation(org.x, org.y, org.z);
      cam.RotateX(x);
      cam.RotateY(y);
      cam.RotateZ(z);
      TV_3DVECTOR ret = cam.GetRotation();
      cam.SetRotation(o.x, o.y, o.z);
      return ret;
    }
  }
}
