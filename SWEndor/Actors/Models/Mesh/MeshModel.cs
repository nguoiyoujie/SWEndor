using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Player;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public struct MeshModel
    {
      private TVMesh Mesh;
      private TVMesh FarMesh;

      private ScopeCounterManager.ScopeCounter meshScope;
      private ScopeCounterManager.ScopeCounter disposeScope;

      public void Init(int id, ActorTypeInfo atype)
      {
        Mesh = atype.GenerateMesh();
        FarMesh = atype.GenerateFarMesh();

        if (meshScope == null)
          meshScope = new ScopeCounterManager.ScopeCounter();

        if (disposeScope == null)
          disposeScope = new ScopeCounterManager.ScopeCounter();

        ScopeCounterManager.Reset(disposeScope);

        using (ScopeCounterManager.Acquire(meshScope))
        {
          Mesh.SetTag(id.ToString());
          //Mesh.ShowBoundingBox(true);

          FarMesh.SetTag(id.ToString());

          Mesh.ComputeBoundings();
          FarMesh.ComputeBoundings();

          Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
          FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
        }
      }

      public void Dispose()
      {
        if (ScopeCounterManager.AcquireIfZero(disposeScope))
        {
          ScopeCounterManager.WaitForZero(meshScope, ScopeGlobals.GLOBAL_COLLISION, ScopeGlobals.GLOBAL_RENDER);

          Mesh?.Destroy();
          Mesh = null;

          FarMesh?.Destroy();
          FarMesh = null;
        }
      }

      public BoundingBox GetBoundingBox(bool uselocal)
      {
        TV_3DVECTOR minV = new TV_3DVECTOR();
        TV_3DVECTOR maxV = new TV_3DVECTOR();

        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            Mesh.GetBoundingBox(ref minV, ref maxV, uselocal);

        return new BoundingBox(minV, maxV);
      }

      public BoundingSphere GetBoundingSphere(bool uselocal)
      {
        TV_3DVECTOR p = new TV_3DVECTOR();
        float r = 0;

        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            Mesh.GetBoundingSphere(ref p, ref r, uselocal);

        return new BoundingSphere(p, r);
      }

      public int GetVertexCount()
      {
        int ret = 0;

        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            ret = Mesh.GetVertexCount();

        return ret;
      }

      public void SetTexture(int iTexture)
      {
        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            Mesh.SetTexture(iTexture);
      }

      public TV_3DVECTOR GetVertex(int vertexID)
      {
        float x = 0;
        float y = 0;
        float z = 0;
        float dummy = 0;
        int dumint = 0;

        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            Mesh.GetVertex(vertexID, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);

        return new TV_3DVECTOR(x, y, z);
      }

      public void Render(bool renderfar) { Render(renderfar ? FarMesh : Mesh); }

      private void Render(TVMesh mesh)
      {
        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            if (mesh.IsVisible())
              mesh.Render();
      }

      public void Update(ActorInfo actor)
      {
        TV_3DMATRIX mat = actor.GetMatrix();
        bool collide = actor.Mask.Has(ComponentMask.CAN_BECOLLIDED) && actor.Active && !actor.IsAggregateMode;
        bool render = actor.Mask.Has(ComponentMask.CAN_RENDER) && actor.Active && !actor.IsAggregateMode && (!actor.IsPlayer || actor.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION);
        bool far = actor.IsFarMode;

        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
          {
            Mesh.SetMatrix(mat);
            FarMesh.SetMatrix(mat);

            ScopeCounterManager.WaitForZero(ScopeGlobals.GLOBAL_COLLISION);
            using (ScopeCounterManager.Acquire(ScopeGlobals.PREREQ_COLLISION))
              Mesh.SetCollisionEnable(collide);

            ScopeCounterManager.WaitForZero(ScopeGlobals.GLOBAL_RENDER);
            using (ScopeCounterManager.Acquire(ScopeGlobals.PREREQ_RENDER))
            {
              Mesh.Enable(render && !far);
              FarMesh.Enable(render && far);
            }
          }
      }
    }

    public BoundingBox GetBoundingBox(bool uselocal) { return Meshes.GetBoundingBox(uselocal); }
    public BoundingSphere GetBoundingSphere(bool uselocal) { return Meshes.GetBoundingSphere(uselocal); }
    public void SetTexture(int iTexture) { Meshes.SetTexture(iTexture); }
    public TV_3DVECTOR GetVertex(int vertexID) { return Meshes.GetVertex(vertexID); }
    public int GetVertexCount() { return Meshes.GetVertexCount(); }
    public void Render(bool renderfar) { Meshes.Render(renderfar); }
  }
}
