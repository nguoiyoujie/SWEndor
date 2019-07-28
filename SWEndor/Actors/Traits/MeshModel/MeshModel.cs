using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;

namespace SWEndor.Actors.Traits
{
  public class MeshModel : IMeshModel
  {
    private TVMesh Mesh { get; set; }
    private TVMesh FarMesh { get; set; }

    public void Init(int id, ActorTypeInfo atype)
    {
      Mesh = atype.GenerateMesh();
      FarMesh = atype.GenerateFarMesh();

      Mesh.SetTag(id.ToString());
      //Mesh.ShowBoundingBox(true);

      FarMesh.SetTag(id.ToString());

      Mesh.ComputeBoundings();
      FarMesh.ComputeBoundings();

      Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);

      Mesh.Enable(true);
      FarMesh.Enable(true);
    }

    public void Dispose()
    {
      Mesh?.Destroy();
      Mesh = null;

      FarMesh?.Destroy();
      FarMesh = null;
    }

    public BoundingBox GetBoundingBox(bool uselocal)
    {
      TV_3DVECTOR minV = new TV_3DVECTOR();
      TV_3DVECTOR maxV = new TV_3DVECTOR();
      Mesh?.GetBoundingBox(ref minV, ref maxV, uselocal);

      return new BoundingBox(minV, maxV);
    }

    public BoundingSphere GetBoundingSphere(bool uselocal)
    {
      TV_3DVECTOR p = new TV_3DVECTOR();
      float r = 0;
      Mesh?.GetBoundingSphere(ref p, ref r, uselocal);

      return new BoundingSphere(p, r);
    }

    public int GetVertexCount() { return Mesh?.GetVertexCount() ?? 0; }

    public void SetTexture(int iTexture) { Mesh?.SetTexture(iTexture); }

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

    public void Render(bool renderfar) { Render(renderfar ? FarMesh : Mesh); }

    private void Render(TVMesh mesh) { if (mesh?.IsVisible() ?? false) mesh.Render(); }

    public void Update(ActorInfo actor)
    {
      if (Mesh == null && FarMesh == null)
        return;

      TV_3DMATRIX mat = actor.GetMatrix();

      Mesh?.SetMatrix(mat);
      FarMesh?.SetMatrix(mat);

      Mesh?.SetCollisionEnable(actor.StateModel.ComponentMask.Has(ComponentMask.CAN_BECOLLIDED) && !actor.IsAggregateMode);
    }
  }
}
