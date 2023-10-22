using MTV3D65;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System.IO;
using Primrose.Primitives.ValueTypes;
using Primrose.Primitives.Factories;

namespace SWEndor.Game.ActorTypes.Components
{
  internal enum MeshMode : byte
  {
    /// <summary>Empty mesh</summary>
    NONE,

    /// <summary>Normal mesh loaded from a source .X file</summary>
    NORMAL,

    /// <summary>Horizontal plane mesh</summary>
    HORIZON,

    /// <summary>Normal mesh, with a custom texture applied</summary>
    TEX_MOD,

    /// <summary>Vertical texture wall</summary>
    ALPHATEX_WALL,

    /// <summary>Flickering texture wall</summary>
    ALPHATEX_FLICKERWALL,

    /// <summary>Billboard animation. Used for explosions</summary>
    BILLBOARD_ANIM
  }


  internal struct MeshData
  {
    /// <summary>Describes the type of mesh</summary>
    [INIValue]
    public MeshMode Mode;

    /// <summary>Scales the mesh from its original dimensions</summary>
    [INIValue]
    public float3 Scale;

    /// <summary>The path of the mesh</summary>
    [INIValue]
    public string SourceMeshPath;

    /// <summary>The path of the far (less detailed) mesh, if any</summary>
    [INIValue]
    public string SourceFarMeshPath;

    /// <summary>The shader name</summary>
    [INIValue]
    public string Shader;

    /// <summary>The blend mode to use</summary>
    [INIValue]
    public CONST_TV_BLENDINGMODE BlendMode;

    /// <summary>The order in which the mesh is rendered. Order 0 is first priority, following by increaing numbers. Values below 0 are not rendered.</summary>
    [INIValue]
    public int RenderOrder;

    // TO-DO: Revise custom data structure
    /// <summary>Custom data</summary>
    [INIValue]
    public string[] Data;


    // Derived values
    // Source meshes are static meshes that will be used to generate instance meshes. Generally they are not to be modified.
    // Instances will use duplicates of the source mesh. The object pool will act as the generator source.
    public TVMesh SourceMesh;
    public TVMesh SourceFarMesh;
    private ObjectPool<TVMesh, MeshData> _meshPool;
    private ObjectPool<TVMesh, MeshData> _farmeshPool;
    public TV_3DVECTOR MaxDimensions;
    public TV_3DVECTOR MinDimensions;


    public static MeshData Default;

    public static void Init(Engine engine)
    {
      Default = MeshDataDecorator.CreateDefault(engine);
    }

    public MeshData(Engine engine, string id, string srcMesh) : this(engine, id, srcMesh, null, float3.One, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale) : this(engine, id, srcMesh, null, scale, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale, CONST_TV_BLENDINGMODE blendmode) : this(engine, id, srcMesh, null, scale, blendmode, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale, CONST_TV_BLENDINGMODE blendmode, string shader) : this(engine, id, srcMesh, null, scale, blendmode, shader) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale, string shader) : this(engine, id, srcMesh, null, scale, CONST_TV_BLENDINGMODE.TV_BLEND_NO, shader) { }

    public MeshData(Engine engine, string id, string srcMesh, string srcFarMesh, float3 scale, CONST_TV_BLENDINGMODE blendmode, string shader) : this(engine, id, srcMesh, srcFarMesh, scale, blendmode, shader, 0) { }

    public MeshData(Engine engine, string id, string srcMesh, string srcFarMesh, float3 scale, CONST_TV_BLENDINGMODE blendmode, string shader, int renderOrder)
    {
      string farname = id + "_far";
      SourceMeshPath = srcMesh;
      SourceFarMeshPath = srcFarMesh;
      MinDimensions = new TV_3DVECTOR();
      MaxDimensions = new TV_3DVECTOR();
      Scale = scale;
      Mode = MeshMode.NORMAL;
      Shader = shader;
      BlendMode = blendmode;
      RenderOrder = renderOrder;
      Data = null;

      // create SourceMesh and SourceFarMesh
      SourceMesh = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (SourceMesh == null)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE)) // allow meshes to be loaded concurrently
        {
          if (engine.Game.IsRunning) // Handle load interrupts in case game is closed during loading stage
          {
            SourceMesh = engine.TrueVision.TVScene.CreateMeshBuilder(id);
            engine.MeshRegistry.Put(id, SourceMesh);
            if (srcMesh != null)
            {
              SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcMesh), true);
              SourceMesh.Enable(false);
              SourceMesh.SetCollisionEnable(false);
              SourceMesh.WeldVertices(0.001f, 0.001f);
              SourceMesh.ComputeBoundings();
              SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);
              // test code
              //SourceMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_BUMPMAPPING_TANGENTSPACE, 0, 1);
              //SourceMesh.SetShadowCast(true, true);
              SourceMesh.SetBlendingMode(blendmode);
            }
          }
        }
      }

      SourceFarMesh = engine.MeshRegistry.Get(farname); //engine.TrueVision.TVGlobals.GetMesh(farname);
      if (SourceFarMesh == null)
      {
        if (srcFarMesh != null)
        {
          using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE)) // allow meshes to be loaded concurrently
          {
            if (engine.Game.IsRunning) // Handle load interrupts in case game is closed during loading stage
            {
              SourceFarMesh = engine.TrueVision.TVScene.CreateMeshBuilder(farname);
              engine.MeshRegistry.Put(farname, SourceFarMesh);
              SourceFarMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcFarMesh), true);
              SourceFarMesh.Enable(false);
              SourceFarMesh.SetCollisionEnable(false);
              SourceFarMesh.WeldVertices(0.01f, 0.01f);
              SourceFarMesh.ComputeBoundings();
              SourceFarMesh.SetBlendingMode(blendmode);
            }
          }
        }
        else
          SourceFarMesh = SourceMesh;
      }

      _meshPool = new ObjectPool<TVMesh, MeshData>((m) => { return m.SourceMesh?.Duplicate(); });
      _farmeshPool = new ObjectPool<TVMesh, MeshData>((m) => { return (m.SourceFarMesh ?? m.SourceMesh)?.Duplicate(); });
    }

    public MeshData(TVMesh mesh, float3 scale, MeshMode mode, CONST_TV_BLENDINGMODE blendmode, string data, string shader, int renderOrder)
    {
      SourceMeshPath = data;
      SourceFarMeshPath = null;
      MinDimensions = new TV_3DVECTOR();
      MaxDimensions = new TV_3DVECTOR();
      Scale = scale;
      Mode = mode;
      Shader = shader;
      BlendMode = blendmode;
      Data = null;
      RenderOrder = renderOrder;

      mesh.SetBlendingMode(blendmode);

      SourceMesh = mesh;
      SourceFarMesh = mesh;
      SourceFarMesh.Enable(false);
      SourceMesh.SetCollisionEnable(false);
      SourceMesh.WeldVertices();
      SourceMesh.ComputeBoundings();
      SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);

      _meshPool = new ObjectPool<TVMesh, MeshData>((m) => { return m.SourceMesh?.Duplicate(); });
      _farmeshPool = new ObjectPool<TVMesh, MeshData>((m) => { return (m.SourceFarMesh ?? m.SourceMesh)?.Duplicate(); });
    }

    public TVMesh GetNewMesh()
    {
      return _meshPool.GetNew(this);
    }

    public TVMesh GetNewFarMesh()
    {
      return _farmeshPool.GetNew(this);
    }

    public void ReturnMesh(TVMesh mesh)
    {
      _meshPool.Return(mesh);
    }

    public void ReturnFarMesh(TVMesh mesh)
    {
      _farmeshPool.Return(mesh);
    }

    public void Load(Engine engine, string id)
    {
      if (!engine.Game.IsRunning) // Handle load interrupts in case game is closed during loading stage
      {
        return;
      }
      switch (Mode)
      {
        case MeshMode.NONE:
          this = Default;
          break;

        case MeshMode.NORMAL:
          this = new MeshData(engine, id, SourceMeshPath, SourceFarMeshPath, Scale, BlendMode, Shader, RenderOrder);
          break;

        case MeshMode.HORIZON:
          {
            if (Data == null)
              break;

            if (Data.Length < 2)
              break;

            if (!float.TryParse(Data[0], out float size))
              break;

            string texname = Data[1];
            this = MeshDataDecorator.CreateHorizon(engine, id, size, texname, BlendMode, Shader, RenderOrder);
          }
          break;

        case MeshMode.TEX_MOD:
          {
            if (Data == null)
              break;

            if (Data.Length < 2)
              break;

            string modelpath = Data[0];
            string texname = Data[1];

            this = MeshDataDecorator.CreateTexturedModel(engine, id, modelpath, texname, BlendMode, Shader, RenderOrder);
          }
          break;

        case MeshMode.ALPHATEX_WALL:
          {
            if (Data == null)
              break;

            if (Data.Length < 3)
              break;

            if (!float.TryParse(Data[0], out float size))
              break;

            string texname = Data[1];
            string alphatexname = Data[2];

            this = MeshDataDecorator.CreateAlphaTexturedWall(engine, id, size, texname, alphatexname, BlendMode, Shader, RenderOrder);
          }
          break;

        case MeshMode.BILLBOARD_ANIM:
          {
            if (Data == null)
              break;

            if (Data.Length < 4)
              break;

            if (!float.TryParse(Data[0], out float size))
              break;

            if (!int.TryParse(Data[2], out int columns))
              break;

            if (!int.TryParse(Data[3], out int rows))
              break;

            string texname = Data[1];
            this = MeshDataDecorator.CreateBillboardAtlasAnimation(engine, id, size, texname, BlendMode, columns, rows, Shader, RenderOrder);
          }
          break;
      }
    }
  }
  

  internal static class MeshDataDecorator
  {
    public static MeshData CreateDefault(Engine engine)
    {
      return new MeshData(engine.TrueVision.TVScene.CreateMeshBuilder(), float3.One, MeshMode.NONE, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null, null, 0);
    }

    public static MeshData CreateHorizon(Engine engine, string id, float size, string texname, CONST_TV_BLENDINGMODE blendmode, string shader = null, int renderOrder = 0)
    {
      TVMesh m = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        m = engine.TrueVision.TVScene.CreateMeshBuilder(id);
        engine.MeshRegistry.Put(id, m);
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int tex = LoadTexture(engine, texname, texpath);
        m.AddFloor(tex, -size, -size, size, size);
        m.SetTexture(tex);
        m.SetAlphaTest(true);
        m.SetCullMode(CONST_TV_CULLING.TV_DOUBLESIDED);
      }
      return new MeshData(m, float3.One, MeshMode.HORIZON, blendmode, "{0},{1}".F(size, texname), shader, renderOrder);
    }

    public static MeshData CreateTexturedModel(Engine engine, string id, string modelpath, string texname, CONST_TV_BLENDINGMODE blendmode, string shader = null, int renderOrder = 0)
    {
      TVMesh m = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        m = engine.TrueVision.TVScene.CreateMeshBuilder(id);
        engine.MeshRegistry.Put(id, m);
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int itex = LoadTexture(engine, texname, texpath);
        m.LoadXFile(Path.Combine(Globals.ModelPath, modelpath), true);
        m.SetTexture(itex);
      }
      return new MeshData(m, float3.One, MeshMode.TEX_MOD, blendmode, "{0},{1}".F(modelpath, texname), shader, renderOrder);
    }

    public static MeshData CreateAlphaTexturedWall(Engine engine, string id, float size, string texname, string alphatexname, CONST_TV_BLENDINGMODE blendmode, string shader = null, int renderOrder = 0)
    {
      TVMesh m = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          m = engine.TrueVision.TVScene.CreateMeshBuilder(id);
          engine.MeshRegistry.Put(id, m);
        }
        // 1 texture
        int tex = engine.TextureRegistry.Get(texname);
        if (tex == 0)
        {
          string texpath = Path.Combine(Globals.ImagePath, texname);
          string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname);
          tex = LoadAlphaTexture(engine, texname, texpath, alphatexpath);
        }

        m.AddWall(tex, -size / 2, 0, size / 2, 0, size, -size / 2);
        m.SetTexture(tex);
      }
      return new MeshData(m, float3.One, MeshMode.ALPHATEX_WALL, blendmode, "{0},{1},{2}".F(size, texname, alphatexname), shader, renderOrder);
    }

    public static MeshData CreateAlphaTexturedFlickerWall(Engine engine, string id, float size, string texname, string texdname, string alphatexname, CONST_TV_BLENDINGMODE blendmode, int frames, int[] flickerframes, ref int[] texanimframes, int renderOrder = 0)
    {
      TVMesh m = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          m = engine.TrueVision.TVScene.CreateMeshBuilder(id);
          engine.MeshRegistry.Put(id, m);
        }
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname);

        int dstartex = engine.TextureRegistry.Get(texname); //engine.TrueVision.TVGlobals.GetTex(id);
        if (dstartex == 0)
        {
          string texpath = Path.Combine(Globals.ImagePath, texname);
          dstartex = LoadAlphaTexture(engine, texname, texpath, alphatexpath);
        }

        int dstardtex = engine.TextureRegistry.Get(texdname); //engine.TrueVision.TVGlobals.GetTex(id);
        if (dstardtex == 0)
        {
          string texdpath = Path.Combine(Globals.ImagePath, texdname);
          dstardtex = LoadAlphaTexture(engine, texdname, texdpath, alphatexpath);
        }

        texanimframes = new int[frames];
        for (int i = 0; i < frames; i++)
          texanimframes[i] = dstartex;

        foreach (int f in flickerframes)
          if (f >= 0 && f < texanimframes.Length)
            texanimframes[f] = dstardtex;
        
        m.AddWall(texanimframes[0], -size / 2, 0, size / 2, 0, size, -size / 2);
        m.SetTexture(texanimframes[0]);
      }
      return new MeshData(m, float3.One, MeshMode.ALPHATEX_FLICKERWALL, blendmode, null, null, renderOrder);
    }

    public static MeshData CreateBillboardAtlasAnimation(Engine engine, string id, float size, string texname, CONST_TV_BLENDINGMODE blendmode, int columns, int rows, string shader = null, int renderOrder = 0)
    {
      TVMesh m = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int i = LoadTexture(engine, texname, texpath); // LoadAlphaTexture
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          m = engine.TrueVision.TVScene.CreateBillboard(i, 0, 0, 0, size, size, id, true);
          engine.MeshRegistry.Put(id, m);
        }
        m.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);
        m.SetTextureModEnable(true);
        m.SetTextureModTranslationScale(1f / columns, 1f / rows);
        m.SetAlphaTest(true);
      }
      return new MeshData(m, float3.One, MeshMode.BILLBOARD_ANIM, blendmode, string.Join(",", size, texname, columns, rows), shader, renderOrder);
    }

    private static int LoadAlphaTexture(Engine engine, string id, string texpath, string alphatexpath = null)
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        int tex = engine.TextureRegistry.Get(id); //engine.TrueVision.TVGlobals.GetTex(id);
        if (tex == 0)
        {
          int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath ?? texpath); //LoadAlphaTexture
          tex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, id);
          engine.TextureRegistry.Put(id, tex);
        }
        return tex;
      }
    }

    private static int LoadTexture(Engine engine, string id, string texpath)
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        int tex = engine.TextureRegistry.Get(id); //engine.TrueVision.TVGlobals.GetTex(id);
        if (tex == 0)
        {
          tex = engine.TrueVision.TVTextureFactory.LoadTexture(texpath, id, -1, -1, CONST_TV_COLORKEY.TV_COLORKEY_USE_ALPHA_CHANNEL, true);
          engine.TextureRegistry.Put(id, tex);
        }
        return tex;
      }
    }
  }
}

