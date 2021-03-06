﻿using MTV3D65;
using Primrose.Primitives;
using System.Collections.Generic;
using SWEndor.ExplosionTypes;
using SWEndor.Core;
using SWEndor.Shaders;
using SWEndor.ActorTypes.Components;

namespace SWEndor.Explosions.Models
{
  internal struct MeshModel
  {
    private static Dictionary<int, int> m_ids = new Dictionary<int, int>();
    public static int GetID(int meshID)
    {
      int i;
      return m_ids.TryGetValue(meshID, out i) ? i : -1;
    }

    private TVMesh Mesh;
    private ShaderInfo ShaderInfo;
    private TVShader Shader;

    private ScopeCounters.ScopeCounter meshScope;
    private ScopeCounters.ScopeCounter disposeScope;

    public void Init(Engine engine, int id, ref MeshData data)
    {
      if (meshScope == null)
        meshScope = new ScopeCounters.ScopeCounter();

      if (disposeScope == null)
        disposeScope = new ScopeCounters.ScopeCounter();

      GenerateMeshes(engine, id, ref data);

      ScopeCounters.Reset(disposeScope);
    }

    private void GenerateMeshes(Engine engine, int id, ref MeshData data)
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        Mesh = data.SourceMesh.Duplicate();
        m_ids[Mesh.GetIndex()] = id;

        string shdr = data.Shader;
        if (shdr != null)
        {
          ShaderInfo = engine.ShaderFactory.Get(shdr);
          if (ShaderInfo != null)
          {
            Shader = ShaderInfo.GetOrCreate();
            Mesh.SetShader(Shader);
          }
        }

        Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      }
    }

    public void Dispose()
    {
      if (ScopeCounters.AcquireIfZero(disposeScope))
      {
        using (ScopeCounters.AcquireWhenZero(meshScope))
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          Mesh.SetShader(null);
          Mesh?.Destroy();
          m_ids.Remove(Mesh.GetIndex());
          Mesh = null;

          ShaderInfo?.ReturnShader(Shader);
          Shader = null;
          ShaderInfo = null;
        }
      }
    }

    /*
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
    */

    public int GetVertexCount()
    {
      int ret = 0;

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          ret = Mesh.GetVertexCount();

      return ret;
    }

    /*
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
    */

    public void SetTexMod(float u, float v, float su, float sv)
    {
      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          Mesh.SetTextureModTranslationScale(u, v, su, sv);
    }

    public TV_3DVECTOR GetVertex(int vertexID)
    {
      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          Mesh.GetVertex(vertexID, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);

      return new TV_3DVECTOR(x, y, z);
    }

    /*
    public void Render()
    {
      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
          if (Mesh.IsVisible())
            Mesh.Render();
    }
    */

    bool prev_render;
    public void Update(ExplosionInfo actor)
    {
      TV_3DMATRIX mat = actor.GetWorldMatrix();
      bool render = actor.Active && !actor.IsAggregateMode;

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
        {
          Mesh.SetMatrix(mat);
          ShaderInfo?.SetShaderParam<ExplosionInfo, ExplosionTypeInfo, ExplosionCreationInfo>(actor, Shader);

          if (prev_render != render)
          {
            prev_render = render;
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
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
    //public BoundingBox GetBoundingBox(bool uselocal) { return Meshes.GetBoundingBox(uselocal); }
    //public BoundingSphere GetBoundingSphere(bool uselocal) { return Meshes.GetBoundingSphere(uselocal); }
    //public void SetTexture(int iTexture) { Meshes.SetTexture(iTexture); }
    //public void EnableTexMod(bool enable) { Meshes.EnableTexMod(enable); }
    public void SetTexMod(float u, float v, float su, float sv) { Meshes.SetTexMod(u, v, su, sv); }
    public TV_3DVECTOR GetVertex(int vertexID) { return Meshes.GetVertex(vertexID); }
    public int GetVertexCount() { return Meshes.GetVertexCount(); }
    //public void Render() { Meshes.Render(); }

    public TV_3DVECTOR MaxDimensions { get { return TypeInfo.MeshData.MaxDimensions; } }
    public TV_3DVECTOR MinDimensions { get { return TypeInfo.MeshData.MinDimensions; } }
  }
}
