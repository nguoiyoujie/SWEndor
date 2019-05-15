using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public struct MeshData
  {
    private float m_Scale;
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

    public TVMesh Mesh { get; set; }
    public TVMesh FarMesh { get; set; }

    public bool Initialized { get { return Mesh != null; } }

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
      //Mesh.SetShadowCast(true, false);

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
  }
}
