using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Player;
using System.Collections.Generic;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Shaders;
using SWEndor.UI;
using SWEndor.Primitives.Extensions;
using SWEndor.Primitives.Geometry;

namespace SWEndor.Actors.Models
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
    private TVMesh FarMesh;
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
      if (ScopeCounterManager.AcquireIfZero(disposeScope))
      {
        using (ScopeCounterManager.AcquireWhenZero(meshScope))
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
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

      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
          Mesh.GetBoundingBox(ref minV, ref maxV, uselocal);

      return new Box(minV, maxV);
    }

    public Sphere GetBoundingSphere(bool uselocal)
    {
      TV_3DVECTOR p = new TV_3DVECTOR();
      float r = 0;

      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
          Mesh.GetBoundingSphere(ref p, ref r, uselocal);

      return new Sphere(p, r);
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

      using (ScopeCounterManager.Acquire(meshScope))
        if (ScopeCounterManager.IsZero(disposeScope))
          Mesh.GetVertex(vertexID, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);

      return new TV_3DVECTOR(x, y, z);
    }

    //public void Render(bool renderfar) { Render(renderfar ? FarMesh : Mesh); }

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
      TV_3DMATRIX mat = actor.GetWorldMatrix();
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
          ShaderInfo?.SetShaderParam<ActorInfo, ActorTypeInfo, ActorCreationInfo>(actor, Shader);

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

    public void UpdateRenderLine(Engine engine, ActorInfo actor)
    {
      if (Mesh == null)
        return;

      TV_3DVECTOR p = engine.PlayerCameraInfo.Camera.GetPosition();
      TV_3DVECTOR r = engine.PlayerCameraInfo.Camera.GetRotation();
      Sphere sph = GetBoundingSphere(false);
      TVCamera c = engine.Surfaces.RS_PreTarget.GetCamera();
      c.SetRotation(r.x, r.y, r.z);
      c.SetPosition(sph.X, sph.Y, sph.Z);
      TV_3DVECTOR d2 = c.GetFrontPosition(-sph.R * 2.5f);
      c.SetPosition(d2.x, d2.y, d2.z);

      engine.Surfaces.RS_PreTarget.StartRender(false);
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        FarMesh?.Render();
      engine.Surfaces.RS_PreTarget.EndRender();

      // post process:
      engine.Surfaces.RS_Target.StartRender(false);
      int tex = engine.Surfaces.RS_PreTarget.GetTexture();
      int icolor = actor.Faction.Color.Value;
      int w = engine.Surfaces.Target_width;
      int h = engine.Surfaces.Target_height;
      engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      engine.TrueVision.TVScreen2DImmediate.Draw_Texture(tex
                                , 0
                                , 0
                                , w
                                , h
                                , icolor);

      engine.TrueVision.TVScreen2DImmediate.Draw_Box(2, 2, w - 2, h - 2, icolor);
      engine.TrueVision.TVScreen2DImmediate.Action_End2D();

      ActorInfo tp = actor.ParentForCoords ?? actor;
      int fntID = engine.FontFactory.Get(Font.T12).ID;
      engine.TrueVision.TVScreen2DText.Action_BeginText();
      // Name
      engine.TrueVision.TVScreen2DText.TextureFont_DrawText(tp.Name
                                        , 10
                                        , 10
                                        , icolor
                                        , fntID);

      // Shields
      engine.TrueVision.TVScreen2DText.TextureFont_DrawText("SHD"
                                              , 15
                                              , h - 45
                                              , icolor
                                              , fntID);

      engine.TrueVision.TVScreen2DText.TextureFont_DrawText((tp.MaxShd == 0) ? "----" : "{0:0}%".F(tp.Shd_Perc)
                                              , 15 + 40
                                              , h - 45
                                              , ((tp.MaxShd == 0) ? new COLOR(1, 1, 1, 0.4f) : tp.Shd_Color).Value
                                              , fntID);

      // Hull
      engine.TrueVision.TVScreen2DText.TextureFont_DrawText("HULL"
                                              , 15
                                              , h - 25
                                              , icolor
                                              , fntID);

      engine.TrueVision.TVScreen2DText.TextureFont_DrawText((tp.MaxHull == 0) ? "100%" : "{0:0}%".F(tp.Hull_Perc)
                                              , 15 + 40
                                              , h - 25
                                              , ((tp.MaxHull == 0) ? new COLOR(0, 1, 0, 1) : tp.Hull_Color).Value
                                              , fntID);

      // Systems
      int i = 0;
      int maxpart = tp.TypeInfo.SystemData.Parts.Length;
      fntID = engine.FontFactory.Get(Font.T08).ID;
      foreach (SystemPart part in tp.TypeInfo.SystemData.Parts)
      {
        SystemState s = tp.GetStatus(part);
        ColorLocalKeys k = s == SystemState.ACTIVE ? ColorLocalKeys.GAME_SYSTEMSTATE_ACTIVE :
                           s == SystemState.DISABLED ? ColorLocalKeys.GAME_SYSTEMSTATE_DISABLED :
                           s == SystemState.DESTROYED ? ColorLocalKeys.GAME_SYSTEMSTATE_DESTROYED :
                                                        ColorLocalKeys.GAME_SYSTEMSTATE_NULL;
        int scolor = ColorLocalization.Get(k).Value;

        engine.TrueVision.TVScreen2DText.TextureFont_DrawText(part.GetShorthand()
                                                      , w - 5 - 25 * (1 + i % 4)
                                                      , h - 5 - 12 * (1 + maxpart / 4 - i / 4)
                                                      , scolor
                                                      , fntID);
        i++;
      }

      engine.TrueVision.TVScreen2DText.Action_EndText();
      engine.Surfaces.RS_Target.EndRender();
    }

    public void EnableCollision(bool enable)
    {
      Mesh?.SetCollisionEnable(enable);
      FarMesh?.SetCollisionEnable(enable);
    }

    public bool Collision(Engine engine, ActorInfo actor, TV_3DVECTOR start, TV_3DVECTOR end)
    {
      TVMesh m = actor.IsFarMode ? FarMesh : Mesh;
      return m.Collision(start, end);
    }

    public bool AdvancedCollision(Engine engine, ActorInfo actor, TV_3DVECTOR start, TV_3DVECTOR end, ref TV_COLLISIONRESULT result)
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
    //public void Render(bool renderfar) { Meshes.Render(renderfar); }
    public void UpdateRenderLine() { Meshes.UpdateRenderLine(Engine, this); }
    public void EnableCollision(bool enable) { Meshes.EnableCollision(enable); }
    public bool AdvancedCollision(TV_3DVECTOR start, TV_3DVECTOR end, ref TV_COLLISIONRESULT result) { return Meshes.AdvancedCollision(Engine, this, start, end, ref result); }

    public TV_3DVECTOR MaxDimensions { get { return TypeInfo.MeshData.MaxDimensions; } }
    public TV_3DVECTOR MinDimensions { get { return TypeInfo.MeshData.MinDimensions; } }
  }
}
