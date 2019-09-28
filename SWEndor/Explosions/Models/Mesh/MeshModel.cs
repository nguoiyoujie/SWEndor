using MTV3D65;
using SWEndor.Primitives;
using System.Collections.Generic;
using SWEndor.Core;
using SWEndor.ExplosionTypes;

namespace SWEndor.Explosions.Models
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

    private ScopeCounterManager.ScopeCounter meshScope;
    private ScopeCounterManager.ScopeCounter disposeScope;

    public void Init(int id, ExplosionTypeInfo atype)
    {
      if (meshScope == null)
        meshScope = new ScopeCounterManager.ScopeCounter();

      if (disposeScope == null)
        disposeScope = new ScopeCounterManager.ScopeCounter();

      GenerateMeshes(id, atype);

      ScopeCounterManager.Reset(disposeScope);
    }

    private void GenerateMeshes(int id, ExplosionTypeInfo atype)
    {
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        Mesh = atype.MeshData.SourceMesh.Duplicate();
        m_ids[Mesh.GetIndex()] = id;
        Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
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

    public void Render()
    {
      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
          if (Mesh.IsVisible())
            Mesh.Render();
    }

    bool prev_render;
    public void Update(ExplosionInfo actor)
    {
      TV_3DMATRIX mat = actor.GetWorldMatrix();
      bool render = actor.Active && !actor.IsAggregateMode;

      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
        {
          Mesh.SetMatrix(mat);

          if (prev_render != render)
          {
            prev_render = render;
            using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              Mesh.Enable(render);
          }
        }
    }
  }
}

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo
  {
    public BoundingBox GetBoundingBox(bool uselocal) { return Meshes.GetBoundingBox(uselocal); }
    public BoundingSphere GetBoundingSphere(bool uselocal) { return Meshes.GetBoundingSphere(uselocal); }
    public void SetTexture(int iTexture) { Meshes.SetTexture(iTexture); }
    public void EnableTexMod(bool enable) { Meshes.EnableTexMod(enable); }
    public void SetTexMod(float u, float v, float su, float sv) { Meshes.SetTexMod(u, v, su, sv); }
    public TV_3DVECTOR GetVertex(int vertexID) { return Meshes.GetVertex(vertexID); }
    public int GetVertexCount() { return Meshes.GetVertexCount(); }
    public void Render() { Meshes.Render(); }
  }
}
