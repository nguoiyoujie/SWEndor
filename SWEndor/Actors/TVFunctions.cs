using MTV3D65;
using SWEndor.Actors.Data;

namespace SWEndor.Actors
{
  public static class TVFunctions
  {
    //public static void GetBoundingBox(this ActorInfo a, ref TV_3DVECTOR min, ref TV_3DVECTOR max) { a.Engine.MeshDataSet.Mesh_get(a.ID)?.GetBoundingBox(ref min, ref max); }

    //public static void GetBoundingBox(this ActorInfo a, ref TV_3DVECTOR min, ref TV_3DVECTOR max, bool localmode) { a.Engine.MeshDataSet.Mesh_get(a.ID)?.GetBoundingBox(ref min, ref max, localmode); }

    //public static void GetBoundingSphere(this ActorInfo a, ref TV_3DVECTOR retCenter, ref float radius) { a.Engine.MeshDataSet.Mesh_get(a.ID)?.GetBoundingSphere(ref retCenter, ref radius); }

    //public static void GetBoundingSphere(this ActorInfo a, ref TV_3DVECTOR retCenter, ref float radius, bool localmode) { a.Engine.MeshDataSet.Mesh_get(a.ID)?.GetBoundingSphere(ref retCenter, ref radius, localmode); }

    //public static void SetTexture(this ActorInfo a, int iTexture) { a.Engine.MeshDataSet.Mesh_get(a.ID)?.SetTexture(iTexture); }

    //public static int GetVertexCount(this ActorInfo a) { return a.Engine.MeshDataSet.Mesh_getVertexCount(a.ID); }

    /*
    public static TV_3DVECTOR GetRandomVertex(this ActorInfo a)
    {
      TVMesh mesh = a.Engine.MeshDataSet.Mesh_get(a.ID);

      if (mesh == null)
        return new TV_3DVECTOR();

      int r = a.GetEngine().Random.Next(0, mesh.GetVertexCount());

      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;
      mesh.GetVertex(r, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
      return new TV_3DVECTOR(x, y, z);
    }
    */
  }
}
