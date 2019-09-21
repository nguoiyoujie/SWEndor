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

      public void EnableTexMod(bool enable)
      {
        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            Mesh.SetTextureModEnable(enable);
      }

      public void SetTexMod(float u, float v, float su, float sv)
      {
        using (ScopeCounterManager.Acquire(meshScope))
          if (ScopeCounterManager.IsZero(disposeScope))
            Mesh.SetTextureModTranslationScale(u, v, su, sv);
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
        //bool render = !occluded && actor.Mask.Has(ComponentMask.CAN_RENDER) && actor.Active && !actor.IsAggregateMode && (!actor.IsPlayer || actor.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION);
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
          }
      }

      //bool occluded;
      public void UpdateRender(ActorInfo actor)
      {
        // revise
        /*
        bool render = actor.Mask.Has(ComponentMask.CAN_RENDER) && actor.Active && !actor.IsAggregateMode && (!actor.IsPlayer || actor.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION);
        
        if (render)
        {
          TVCamera c = actor.PlayerCameraInfo.Camera;
          int q = c.OccQuery_Begin();
          TV_3DVECTOR bMin = new TV_3DVECTOR();
          TV_3DVECTOR bMax = new TV_3DVECTOR();
          actor.Meshes.FarMesh.GetBoundingBox(ref bMin, ref bMax, false);
          c.OccQuery_DrawBox(bMin, bMax);

          //BoundingBox b = GetBoundingBox(false);
          //c.OccQuery_DrawBox(new TV_3DVECTOR(b.X.Min, b.Y.Min, b.Z.Min), new TV_3DVECTOR(b.X.Max, b.Y.Max, b.Z.Max));
          c.OccQuery_End();
          occluded = c.OccQuery_GetData(q, true) == 0;
        }
        */
      }

      public void UpdateRenderLine(ActorInfo actor)
      {
        if (Mesh == null)
          return;

        TV_3DVECTOR p = actor.PlayerCameraInfo.Camera.GetPosition();
        BoundingSphere sph = actor.GetBoundingSphere(false);
        TV_3DVECTOR d2 = new TV_3DVECTOR();
        TVCamera c = actor.TrueVision.TargetRenderSurface.GetCamera();
        c.SetPosition(p.x, p.y, p.z);
        c.LookAtMesh(Mesh);
        c.SetPosition(sph.Position.x, sph.Position.y, sph.Position.z);
        d2 = c.GetFrontPosition((actor.TypeInfo.MeshData.MinDimensions.z - actor.TypeInfo.MeshData.MaxDimensions.z) * 2); // - sph.Radius * 3);
        c.SetPosition(d2.x, d2.y, d2.z);

        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          actor.TrueVision.TargetRenderSurface.StartRender(false);
          //actor.TrueVision.TVScene.SetRenderMode(CONST_TV_RENDERMODE.TV_LINE);
          //FarMesh?.Render();
          actor.TrueVision.TVScene.RenderAllMeshes(true);

          actor.TrueVision.TargetRenderSurface.EndRender();
          //actor.TrueVision.TVScene.SetRenderMode(CONST_TV_RENDERMODE.TV_SOLID);
        }
      }
    }

    public BoundingBox GetBoundingBox(bool uselocal) { return Meshes.GetBoundingBox(uselocal); }
    public BoundingSphere GetBoundingSphere(bool uselocal) { return Meshes.GetBoundingSphere(uselocal); }
    public void SetTexture(int iTexture) { Meshes.SetTexture(iTexture); }
    public void EnableTexMod(bool enable) { Meshes.EnableTexMod(enable); }
    public void SetTexMod(float u, float v, float su, float sv) { Meshes.SetTexMod(u, v, su, sv); }
    public TV_3DVECTOR GetVertex(int vertexID) { return Meshes.GetVertex(vertexID); }
    public int GetVertexCount() { return Meshes.GetVertexCount(); }
    public void Render(bool renderfar) { Meshes.Render(renderfar); }
    public void UpdateRenderLine() { Meshes.UpdateRenderLine(this); }
  }
}
