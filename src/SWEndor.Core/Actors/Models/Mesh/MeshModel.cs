using MTV3D65;
using SWEndor.ActorTypes;
using Primrose.Primitives;
using System.Collections.Generic;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Shaders;
using SWEndor.UI;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Geometry;

namespace SWEndor.Actors.Models
{
  internal struct MeshModel
  {
    // TO-DO: Move out of 'static'
    private static Dictionary<int, int> m_ids = new Dictionary<int, int>();
    public static int GetID(int meshID)
    {
      int i;
      return m_ids.TryGetValue(meshID, out i) ? i : -1;
    }

    private TVMesh Mesh;
    private TVMesh FarMesh;
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
        FarMesh = data.SourceFarMesh == null ? data.SourceMesh.Duplicate() : data.SourceFarMesh.Duplicate();

        m_ids[Mesh.GetIndex()] = id;
        m_ids[FarMesh.GetIndex()] = id;

        //Mesh.ShowBoundingBox(true);
        string shdr = data.Shader;
        if (shdr != null)
        {
          ShaderInfo = engine.ShaderFactory.Get(shdr);
          if (ShaderInfo != null)
          {
            Shader = ShaderInfo.GetOrCreate();
            Mesh.SetShader(Shader);
            FarMesh.SetShader(Shader);
          }
        }

        Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
        FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
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

          FarMesh.SetShader(null);
          FarMesh?.Destroy();
          m_ids.Remove(FarMesh.GetIndex());
          FarMesh = null;

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

    public void SetTexture(int iTexture)
    {
      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          Mesh.SetTexture(iTexture);
    }

    /*
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

    public void Render(bool renderfar) { Render(renderfar ? FarMesh : Mesh); }

    private void Render(TVMesh mesh)
    {
      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
          mesh.Render();
    }

    bool prev_collide;
    bool prev_collidefar;
    bool prev_render;
    bool prev_renderfar;

    public void Update(ActorInfo actor)
    {
      TV_3DMATRIX mat = actor.GetWorldMatrix();
      bool collide = actor.Mask.Has(ComponentMask.CAN_BECOLLIDED) && actor.Active && !actor.IsAggregateMode;
      bool render = actor.Mask.Has(ComponentMask.CAN_RENDER) && actor.Active && !actor.IsAggregateMode; //&& (!actor.IsPlayer); //|| actor.PlayerCameraInfo.CameraMode != CameraMode.FREEROTATION);
      bool far = actor.IsFarMode;
      bool collidefar = collide && far;
      bool renderfar = render && far;
      collide &= !far;
      render &= !far;

      using (ScopeCounters.Acquire(meshScope))
        if (ScopeCounters.IsZero(disposeScope))
        {
          Mesh.SetMatrix(mat);
          FarMesh.SetMatrix(mat);
          ShaderInfo?.SetShaderParam<ActorInfo, ActorTypeInfo, ActorCreationInfo>(actor, Shader);

          if (prev_render != render)
          {
            prev_render = render;
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              Mesh.Enable(render);
          }

          if (prev_renderfar != renderfar)
          {
            prev_renderfar = renderfar;
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              FarMesh.Enable(renderfar);
          }

          if (prev_collide != collide)
          {
            prev_collide = collide;
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              Mesh.SetCollisionEnable(collide);
          }

          if (prev_collidefar != collidefar)
          {
            prev_collidefar = collidefar;
            using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              FarMesh.SetCollisionEnable(collidefar);
          }
        }
    }

    public void EnableCollision(bool enable)
    {
      Mesh?.SetCollisionEnable(enable);
      FarMesh?.SetCollisionEnable(enable);
    }

    public bool Collision(ActorInfo actor, TV_3DVECTOR start, TV_3DVECTOR end)
    {
      TVMesh m = actor.IsFarMode ? FarMesh : Mesh;
      return m.Collision(start, end);
    }

    public bool AdvancedCollision(ActorInfo actor, TV_3DVECTOR start, TV_3DVECTOR end, ref TV_COLLISIONRESULT result)
    {
      TVMesh m = actor.IsFarMode ? FarMesh : Mesh;
      return m.AdvancedCollision(start, end, ref result);
    }
  }
}

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public Box GetBoundingBox(bool uselocal) { return Meshes.GetBoundingBox(uselocal); }
    public Sphere GetBoundingSphere(bool uselocal) { return Meshes.GetBoundingSphere(uselocal); }
    public void SetTexture(int iTexture) { Meshes.SetTexture(iTexture); }
    //public void EnableTexMod(bool enable) { Meshes.EnableTexMod(enable); }
    //public void SetTexMod(float u, float v, float su, float sv) { Meshes.SetTexMod(u, v, su, sv); }
    public TV_3DVECTOR GetVertex(int vertexID) { return Meshes.GetVertex(vertexID); }
    public int GetVertexCount() { return Meshes.GetVertexCount(); }
    public void Render(bool renderfar) { Meshes.Render(renderfar); }
    public void EnableCollision(bool enable) { Meshes.EnableCollision(enable); }
    public bool AdvancedCollision(TV_3DVECTOR start, TV_3DVECTOR end, ref TV_COLLISIONRESULT result) { return Meshes.AdvancedCollision(this, start, end, ref result); }

    public TV_3DVECTOR MaxDimensions { get { return TypeInfo.MeshData.MaxDimensions; } }
    public TV_3DVECTOR MinDimensions { get { return TypeInfo.MeshData.MinDimensions; } }
  }
}
