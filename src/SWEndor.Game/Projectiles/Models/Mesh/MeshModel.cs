﻿using MTV3D65;
using Primrose.Primitives;
using System.Collections.Generic;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Shaders;
using SWEndor.Game.ProjectileTypes;
using Primrose.Primitives.Geometry;

namespace SWEndor.Game.Projectiles.Models
{
  internal struct MeshModel
  {
    private TVMesh Mesh;
    private ShaderInfo ShaderInfo;
    private TVShader Shader;

    private ScopeCounters.ScopeCounter meshScope;
    private ScopeCounters.ScopeCounter disposeScope;

    public void Init(ShaderInfo.Factory shaderFactory, MeshEntityTable table, int id, ref MeshData data)
    {
      if (meshScope == null)
        meshScope = new ScopeCounters.ScopeCounter();

      if (disposeScope == null)
        disposeScope = new ScopeCounters.ScopeCounter();

      GenerateMeshes(shaderFactory, table, id, ref data);

      ScopeCounters.Reset(disposeScope);
    }

    private void GenerateMeshes(ShaderInfo.Factory shaderFactory, MeshEntityTable table, int id, ref MeshData data)
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        Mesh = data.SourceMesh.Duplicate();
        table.Put(Mesh.GetIndex(), id);

        //Mesh.ShowBoundingBox(true);
        string shdr = data.Shader;
        if (shdr != null)
        {
          ShaderInfo = shaderFactory.Get(shdr);
          if (ShaderInfo != null)
          {
            Shader = ShaderInfo.GetOrCreate();
            Mesh.SetShader(Shader);
          }
        }

        Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      }
    }

    public void Dispose(MeshEntityTable table)
    {
      if (ScopeCounters.AcquireIfZero(disposeScope))
      {
        using (ScopeCounters.AcquireWhenZero(meshScope))
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          Mesh.SetShader(null);
          Mesh.Destroy();
          table.Remove(Mesh.GetIndex());
          Mesh = null;

          ShaderInfo?.ReturnShader(Shader);
          Shader = null;
          ShaderInfo = null;
        }
      }
    }

    public Box GetBoundingBox(bool uselocal)
    {
      TV_3DVECTOR minV = new TV_3DVECTOR();
      TV_3DVECTOR maxV = new TV_3DVECTOR();

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          Mesh.GetBoundingBox(ref minV, ref maxV, uselocal);

      return new Box(minV, maxV);
    }

    public Sphere GetBoundingSphere(bool uselocal)
    {
      TV_3DVECTOR p = new TV_3DVECTOR();
      float r = 0;

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          Mesh.GetBoundingSphere(ref p, ref r, uselocal);

      return new Sphere(p, r);
    }

    public int GetVertexCount()
    {
      int ret = 0;

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          ret = Mesh.GetVertexCount();

      return ret;
    }

    public void SetColor(int color)
    {
      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          Mesh.SetColor(color);
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

    public void SetTexMod(float u, float v, float su, float sv)
    {
      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
          Mesh.SetTextureModTranslationScale(u, v, su, sv);
    }
    */

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

    //public void Render() { Render(Mesh); }

    private void Render(TVMesh mesh)
    {
      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          if (mesh.IsVisible())
            mesh.Render();
    }

    bool prev_collide;
    bool prev_render;

    public void Update(ProjectileInfo actor)
    {
      TV_3DMATRIX mat = actor.GetWorldMatrix();
      bool collide = actor.Mask.Has(ComponentMask.CAN_BECOLLIDED) && actor.Active && !actor.IsAggregateMode;
      bool render = actor.Mask.Has(ComponentMask.CAN_RENDER) && actor.Active && !actor.IsAggregateMode;

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
        {
          Mesh.SetMatrix(mat);
          ShaderInfo?.SetShaderParam<ProjectileInfo, ProjectileTypeInfo, ProjectileCreationInfo>(actor, Shader);

          if (prev_render != render)
          {
            prev_render = render;
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              Mesh.Enable(render);
          }

          if (prev_collide != collide)
          {
            prev_collide = collide;
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              Mesh.SetCollisionEnable(collide);
          }
        }
    }

    public void EnableCollision(bool enable)
    {
      Mesh?.SetCollisionEnable(enable);
    }
  }
}

namespace SWEndor.Game.Projectiles
{
  public partial class ProjectileInfo
  {
    public Box GetBoundingBox(bool uselocal) { return Meshes.GetBoundingBox(uselocal); }
    public Sphere GetBoundingSphere(bool uselocal) { return Meshes.GetBoundingSphere(uselocal); }
    public void SetColor(int iColor) { Meshes.SetColor(iColor); }
    //public void SetTexture(int iTexture) { Meshes.SetTexture(iTexture); }
    //public void EnableTexMod(bool enable) { Meshes.EnableTexMod(enable); }
    //public void SetTexMod(float u, float v, float su, float sv) { Meshes.SetTexMod(u, v, su, sv); }
    public TV_3DVECTOR GetVertex(int vertexID) { return Meshes.GetVertex(vertexID); }
    public int GetVertexCount() { return Meshes.GetVertexCount(); }
    public void EnableCollision(bool enable) { Meshes.EnableCollision(enable); }

    //public void Render() { Meshes.Render(); }

    public TV_3DVECTOR MaxDimensions { get { return TypeInfo.MeshData.MaxDimensions; } }
    public TV_3DVECTOR MinDimensions { get { return TypeInfo.MeshData.MinDimensions; } }
  }
}