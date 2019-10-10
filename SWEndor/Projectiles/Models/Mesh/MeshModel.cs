using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System.Collections.Generic;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Shaders;
using SWEndor.ProjectileTypes;

namespace SWEndor.Projectiles.Models
{
  public struct MeshModel
  {
    // TO-DO: Move out of 'static'
    private static Dictionary<int, int> m_ids = new Dictionary<int, int>();
    public static int GetID(int meshID)
    {
      int i;
      return m_ids.TryGetValue(meshID, out i) ? i : -1;
    }

    private TVMesh Mesh;
    private ShaderInfo ShaderInfo;
    private TVShader Shader;

    private ScopeCounterManager.ScopeCounter meshScope;
    private ScopeCounterManager.ScopeCounter disposeScope;

    public void Init(Engine engine, int id, ref MeshData data)
    {
      if (meshScope == null)
        meshScope = new ScopeCounterManager.ScopeCounter();

      if (disposeScope == null)
        disposeScope = new ScopeCounterManager.ScopeCounter();

      GenerateMeshes(engine, id, ref data);

      ScopeCounterManager.Reset(disposeScope);
    }

    private void GenerateMeshes(Engine engine, int id, ref MeshData data)
    {
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        Mesh = data.SourceMesh.Duplicate();

        m_ids[Mesh.GetIndex()] = id;

        //Mesh.ShowBoundingBox(true);
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
      if (ScopeCounterManager.AcquireIfZero(disposeScope))
      {
        using (ScopeCounterManager.AcquireWhenZero(meshScope))
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
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

    public void Render() { Render(Mesh); }

    private void Render(TVMesh mesh)
    {
      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
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

      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
        {
          Mesh.SetMatrix(mat);
          ShaderInfo?.SetShaderParam<ProjectileInfo, ProjectileTypeInfo, ProjectileCreationInfo>(actor, Shader);

          if (prev_render != render)
          {
            prev_render = render;
            using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              Mesh.Enable(render);
          }

          if (prev_collide != collide)
          {
            prev_collide = collide;
            using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              Mesh.SetCollisionEnable(collide);
          }
        }
    }

    public void UpdateRenderLine(Engine engine, ProjectileInfo actor)
    {
      if (Mesh == null)
        return;

      TV_3DVECTOR p = engine.PlayerCameraInfo.Camera.GetPosition();
      BoundingSphere sph = GetBoundingSphere(false);
      TV_3DVECTOR d2 = new TV_3DVECTOR();
      TVCamera c = engine.TrueVision.TargetRenderSurface.GetCamera();
      c.SetPosition(p.x, p.y, p.z);
      c.LookAtMesh(Mesh);
      c.SetPosition(sph.Position.x, sph.Position.y, sph.Position.z);
      d2 = c.GetFrontPosition(-sph.Radius * 2.5f);
      c.SetPosition(d2.x, d2.y, d2.z);

      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        engine.TrueVision.TargetRenderSurface.StartRender(false);
        Mesh?.Render();
        engine.TrueVision.TargetRenderSurface.EndRender();
      }
    }
  }
}

namespace SWEndor.Projectiles
{
  public partial class ProjectileInfo
  {
    public BoundingBox GetBoundingBox(bool uselocal) { return Meshes.GetBoundingBox(uselocal); }
    public BoundingSphere GetBoundingSphere(bool uselocal) { return Meshes.GetBoundingSphere(uselocal); }
    public void SetTexture(int iTexture) { Meshes.SetTexture(iTexture); }
    public void EnableTexMod(bool enable) { Meshes.EnableTexMod(enable); }
    public void SetTexMod(float u, float v, float su, float sv) { Meshes.SetTexMod(u, v, su, sv); }
    public TV_3DVECTOR GetVertex(int vertexID) { return Meshes.GetVertex(vertexID); }
    public int GetVertexCount() { return Meshes.GetVertexCount(); }
    public void Render() { Meshes.Render(); }
    public void UpdateRenderLine() { Meshes.UpdateRenderLine(Engine, this); }

    public TV_3DVECTOR MaxDimensions { get { return TypeInfo.MeshData.MaxDimensions; } }
    public TV_3DVECTOR MinDimensions { get { return TypeInfo.MeshData.MinDimensions; } }
  }
}
