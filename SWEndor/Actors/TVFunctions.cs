using MTV3D65;

namespace SWEndor.Actors
{
  public static class TVFunctions
  {
    public static void GetBoundingBox(this ActorInfo a, ref TV_3DVECTOR min, ref TV_3DVECTOR max) { a.Mesh?.GetBoundingBox(ref min, ref max); }

    public static void GetBoundingBox(this ActorInfo a, ref TV_3DVECTOR min, ref TV_3DVECTOR max, bool localmode) { a.Mesh?.GetBoundingBox(ref min, ref max, localmode); }

    public static void GetBoundingSphere(this ActorInfo a, ref TV_3DVECTOR retCenter, ref float radius) { a.Mesh?.GetBoundingSphere(ref retCenter, ref radius); }

    public static void GetBoundingSphere(this ActorInfo a, ref TV_3DVECTOR retCenter, ref float radius, bool localmode) { a.Mesh?.GetBoundingSphere(ref retCenter, ref radius, localmode); }

    public static void SetTexture(this ActorInfo a, int iTexture) { a.Mesh?.SetTexture(iTexture); }

    public static int GetVertexCount(this ActorInfo a) { return a.Mesh?.GetVertexCount() ?? 0; }

    public static TV_3DVECTOR GetRandomVertex(this ActorInfo a)
    {
      if (a.Mesh == null)
        return new TV_3DVECTOR();

      int r = Engine.Instance().Random.Next(0, a.Mesh.GetVertexCount());

      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;
      a.Mesh.GetVertex(r, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
      return new TV_3DVECTOR(x, y, z);
    }
  }
}
