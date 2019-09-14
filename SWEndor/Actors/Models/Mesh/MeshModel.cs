using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Player;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public struct MeshModel
    {
      private static Dictionary<int, int> m_ids = new Dictionary<int, int>();
      public static int GetID(int meshID)
      {
        int i;
        return m_ids.TryGetValue(meshID, out i) ? i : -1;
      }

      private TVMesh Mesh;
      private TVMesh FarMesh;

      private ScopeCounterManager.ScopeCounter meshScope;
      private ScopeCounterManager.ScopeCounter disposeScope;

      public void Init(int id, ActorTypeInfo atype)
      {
        if (meshScope == null)
          meshScope = new ScopeCounterManager.ScopeCounter();

        if (disposeScope == null)
          disposeScope = new ScopeCounterManager.ScopeCounter();

        GenerateMeshes(id, atype);

        ScopeCounterManager.Reset(disposeScope);
      }

      private void GenerateMeshes(int id, ActorTypeInfo atype)
      {
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          Mesh = atype.MeshData.SourceMesh.Duplicate();
          FarMesh = atype.MeshData.SourceFarMesh == null ? atype.MeshData.SourceMesh.Duplicate() : atype.MeshData.SourceFarMesh.Duplicate();

          m_ids[Mesh.GetIndex()] = id;
          m_ids[FarMesh.GetIndex()] = id;

          //Mesh.ShowBoundingBox(true);

          Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
          FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
        }        
      }

      public void Dispose()
      {
        if (ScopeCounterManager.AcquireIfZero(disposeScope))
        {
          using (ScopeCounterManager.AcquireWhenZero(meshScope))
          using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE)) //ScopeGlobals.GLOBAL_COLLISION, ScopeGlobals.GLOBAL_RENDER);
          {
            Mesh?.Destroy();
            m_ids.Remove(Mesh.GetIndex());
            Mesh = null;

            FarMesh?.Destroy();
            m_ids.Remove(FarMesh.GetIndex());
            FarMesh = null;
          }
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

      bool prev_collide;
      bool prev_collidefar;
      bool prev_render;
      bool prev_renderfar;

      public void Update(ActorInfo actor)
      {
        TV_3DMATRIX mat = actor.GetMatrix();
        bool collide = actor.Mask.Has(ComponentMask.CAN_BECOLLIDED) && actor.Active && !actor.IsAggregateMode;
        bool render = actor.Mask.Has(ComponentMask.CAN_RENDER) && actor.Active && !actor.IsAggregateMode && (!actor.IsPlayer || actor.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION);
        bool far = actor.IsFarMode;
        bool collidefar = collide && far;
        bool renderfar = render && far;
        collide &= !far;
        render &= !far;

        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
          {
            Mesh.SetMatrix(mat);
            FarMesh.SetMatrix(mat);

            if (prev_collide != collide)
            {
              prev_collide = collide;
              using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
                Mesh.SetCollisionEnable(collide);
            }

            if (prev_collidefar != collidefar)
            {
              prev_collidefar = collidefar;
              using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
                FarMesh.SetCollisionEnable(collidefar);
            }

            if (prev_render != render)
            {
              prev_render = render;
              using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
                Mesh.Enable(render);
            }

            if (prev_renderfar != renderfar)
            {
              prev_renderfar = renderfar;
              using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
                FarMesh.Enable(renderfar);
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
