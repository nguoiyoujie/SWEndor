using MTV3D65;
using SWEndor.ActorTypes;
using System.Runtime.InteropServices;

namespace SWEndor.Actors.Data
{
  [StructLayout(LayoutKind.Explicit, Size = 12)]
  public struct MeshData
  {
    [FieldOffset(0)]
    private float m_Scale;

    [FieldOffset(4)]
    public TVMesh Mesh;

    [FieldOffset(8)]
    public TVMesh FarMesh;

    public float Scale
    {
      get { return m_Scale; }
      set
      {
        if (!m_Scale.Equals(value))
        {
          SetScale();
          m_Scale = value;
        }
      }
    }



    public void Init(ActorTypeInfo type, ActorCreationInfo acreate)
    {
      m_Scale = type.Scale * acreate.InitialScale;
    }

    public void Generate(int id, ActorTypeInfo atype)
    {
      Mesh = atype.GenerateMesh();
      FarMesh = atype.GenerateFarMesh();

      Mesh.SetTag(id.ToString());
      //Mesh.ShowBoundingBox(true);

      FarMesh.SetTag(id.ToString());

      SetScale();
      Mesh.ComputeBoundings();
      FarMesh.ComputeBoundings();

      Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);

      Mesh.Enable(true);
      FarMesh.Enable(true);
    }

    public void Reset()
    {
      Mesh?.Destroy();
      Mesh = null;

      FarMesh?.Destroy();
      FarMesh = null;

      Scale = 1;
    }

    private void SetScale()
    {
      Mesh?.SetScale(Scale, Scale, Scale);
      FarMesh?.SetScale(Scale, Scale, Scale);
    }

    public int GetVertexCount() { return Mesh?.GetVertexCount() ?? 0; }

    public static DoFunc<MeshData, TV_3DVECTOR, TV_3DVECTOR, TV_3DVECTOR> GetBasisVectors = (ref MeshData d, ref TV_3DVECTOR f, ref TV_3DVECTOR u, ref TV_3DVECTOR r) =>
      {
        d.Mesh?.GetBasisVectors(ref f, ref u, ref r);
      };


  public TV_3DVECTOR GetVertex(int vertexID)
    {
      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;
      Mesh?.GetVertex(vertexID, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
      return new TV_3DVECTOR(x, y, z);
    }
  }
}
