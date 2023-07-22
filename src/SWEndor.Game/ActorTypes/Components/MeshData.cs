using MTV3D65;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System.IO;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.ActorTypes.Components
{
  internal enum MeshMode : byte
  {
    NONE, 
    NORMAL,
    HORIZON,
    TEX_MOD,
    ALPHATEX_WALL,
    ALPHATEX_FLICKERWALL,
    BILLBOARD_ANIM
  }


  internal struct MeshData
  {
    [INIValue]
    public MeshMode Mode;

    [INIValue]
    public float3 Scale;

    [INIValue]
    public string SourceMeshPath;

    [INIValue]
    public string SourceFarMeshPath;

    [INIValue]
    public string Shader;

    [INIValue]
    public CONST_TV_BLENDINGMODE BlendMode;

    [INIValue]
    public string[] Data;


    // Derived values
    public TVMesh SourceMesh;
    public TVMesh SourceFarMesh;
    public TV_3DVECTOR MaxDimensions;
    public TV_3DVECTOR MinDimensions;


    public static MeshData Default = new MeshData(Globals.Engine.TrueVision.TVScene.CreateMeshBuilder(), float3.One, MeshMode.NONE, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null, null);

    public MeshData(Engine engine, string id, string srcMesh) : this(engine, id, srcMesh, null, float3.One, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale) : this(engine, id, srcMesh, null, scale, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale, CONST_TV_BLENDINGMODE blendmode) : this(engine, id, srcMesh, null, scale, blendmode, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale, CONST_TV_BLENDINGMODE blendmode, string shader) : this(engine, id, srcMesh, null, scale, blendmode, shader) { }

    public MeshData(Engine engine, string id, string srcMesh, float3 scale, string shader) : this(engine, id, srcMesh, null, scale, CONST_TV_BLENDINGMODE.TV_BLEND_NO, shader) { }

    public MeshData(Engine engine, string id, string srcMesh, string srcFarMesh, float3 scale, CONST_TV_BLENDINGMODE blendmode, string shader)
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
      Data = null;

      // create SourceMesh and SourceFarMesh
      SourceMesh = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (SourceMesh == null)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE)) // allow meshes to be loaded concurrently
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
            //SourceMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_BUMPMAPPING_TANGENTSPACE, 0, 1);
            //SourceMesh.SetShadowCast(true, true);
            SourceMesh.SetBlendingMode(blendmode);
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
        else
          SourceFarMesh = SourceMesh; //.Duplicate();
      }
    }

    public MeshData(TVMesh mesh, float3 scale, MeshMode mode, CONST_TV_BLENDINGMODE blendmode, string data, string shader)
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

      mesh.SetBlendingMode(blendmode);

      SourceMesh = mesh;
      SourceFarMesh = mesh;
      SourceFarMesh.Enable(false);
      SourceMesh.SetCollisionEnable(false);
      SourceMesh.WeldVertices();
      SourceMesh.ComputeBoundings();
      SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);
    }

    public void Load(Engine engine, string id)
    {
      switch (Mode)
      {
        case MeshMode.NONE:
          this = Default;
          break;

        case MeshMode.NORMAL:
          this = new MeshData(engine, id, SourceMeshPath, SourceFarMeshPath, Scale, BlendMode, Shader);
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
            this = MeshDataDecorator.CreateHorizon(engine, id, size, texname, BlendMode, Shader);
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

            this = MeshDataDecorator.CreateTexturedModel(engine, id, modelpath, texname, BlendMode, Shader);
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

            this = MeshDataDecorator.CreateAlphaTexturedWall(engine, id, size, texname, alphatexname, BlendMode, Shader);
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
            this = MeshDataDecorator.CreateBillboardAtlasAnimation(engine, id, size, texname, BlendMode, columns, rows, Shader);
          }
          break;
      }
    }
  }
  

  internal static class MeshDataDecorator
  {
    public static MeshData CreateHorizon(Engine engine, string id, float size, string texname, CONST_TV_BLENDINGMODE blendmode, string shader = null)
    {
      TVMesh m = engine.MeshRegistry.Get(id); //engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        m = engine.TrueVision.TVScene.CreateMeshBuilder(id);
        engine.MeshRegistry.Put(id, m);
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int tex = LoadAlphaTexture(engine, texname, texpath);
        m.AddFloor(tex, -size, -size, size, size);
        m.SetTexture(tex);
        m.SetCullMode(CONST_TV_CULLING.TV_DOUBLESIDED);
      }
      return new MeshData(m, float3.One, MeshMode.HORIZON, blendmode, "{0},{1}".F(size, texname), shader);
    }

    public static MeshData CreateTexturedModel(Engine engine, string id, string modelpath, string texname, CONST_TV_BLENDINGMODE blendmode, string shader = null)
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
      return new MeshData(m, float3.One, MeshMode.TEX_MOD, blendmode, "{0},{1}".F(modelpath, texname), shader);
    }

    public static MeshData CreateAlphaTexturedWall(Engine engine, string id, float size, string texname, string alphatexname, CONST_TV_BLENDINGMODE blendmode, string shader = null)
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
          using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          {
            int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
            int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
            tex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
          }
          engine.TextureRegistry.Put(texname, tex);
        }

        m.AddWall(tex, -size / 2, 0, size / 2, 0, size, -size / 2);
        m.SetTexture(tex);
      }
      return new MeshData(m, float3.One, MeshMode.ALPHATEX_WALL, blendmode, "{0},{1},{2}".F(size, texname, alphatexname), shader);
    }

    public static MeshData CreateAlphaTexturedFlickerWall(Engine engine, string id, float size, string texname, string texdname, string alphatexname, CONST_TV_BLENDINGMODE blendmode, int frames, int[] flickerframes, ref int[] texanimframes)
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
          using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          {
            int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
            int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath); // note we are loading alpha map as texture
            dstartex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
          }
          engine.TextureRegistry.Put(texname, dstartex);
        }

        int dstardtex = engine.TextureRegistry.Get(texdname); //engine.TrueVision.TVGlobals.GetTex(id);
        if (dstardtex == 0)
        {
          string texdpath = Path.Combine(Globals.ImagePath, texdname);
          using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          {
            int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texdpath);
            int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
            dstardtex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texdname);
          }
          engine.TextureRegistry.Put(texdname, dstardtex);
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
      return new MeshData(m, float3.One, MeshMode.ALPHATEX_FLICKERWALL, blendmode, null, null);
    }

    public static MeshData CreateBillboardAtlasAnimation(Engine engine, string id, float size, string texname, CONST_TV_BLENDINGMODE blendmode, int columns, int rows, string shader = null)
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
      }
      return new MeshData(m, float3.One, MeshMode.BILLBOARD_ANIM, blendmode, string.Join(",", size, texname, columns, rows), shader);
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
          tex = engine.TrueVision.TVTextureFactory.LoadTexture(texpath, id);
          engine.TextureRegistry.Put(id, tex);
        }
        return tex;
      }
    }
  }
}

