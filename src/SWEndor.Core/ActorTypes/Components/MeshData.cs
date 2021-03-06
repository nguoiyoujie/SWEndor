﻿using MTV3D65;
using SWEndor.Core;
using Primitives.FileFormat.INI;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Components
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
    public readonly MeshMode Mode;

    [INIValue]
    public readonly float Scale;

    [INIValue]
    public readonly string SourceMeshPath;

    [INIValue]
    public readonly string SourceFarMeshPath;

    [INIValue]
    public readonly string Shader;

    [INIValue]
    public readonly CONST_TV_BLENDINGMODE BlendMode;

    [INIValue]
    public readonly string[] Data;


    // Derived values
    public readonly TVMesh SourceMesh;
    public readonly TVMesh SourceFarMesh;
    public readonly TV_3DVECTOR MaxDimensions;
    public readonly TV_3DVECTOR MinDimensions;


    public readonly static MeshData Default = new MeshData(Globals.Engine.TrueVision.TVScene.CreateMeshBuilder(), 1, MeshMode.NONE, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null, null);

    public MeshData(Engine engine, string id, string srcMesh) : this(engine, id, srcMesh, null, 1, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float scale) : this(engine, id, srcMesh, null, scale, CONST_TV_BLENDINGMODE.TV_BLEND_NO, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float scale, CONST_TV_BLENDINGMODE blendmode) : this(engine, id, srcMesh, null, scale, blendmode, null) { }

    public MeshData(Engine engine, string id, string srcMesh, float scale, CONST_TV_BLENDINGMODE blendmode, string shader) : this(engine, id, srcMesh, null, scale, blendmode, shader) { }

    public MeshData(Engine engine, string id, string srcMesh, float scale, string shader) : this(engine, id, srcMesh, null, scale, CONST_TV_BLENDINGMODE.TV_BLEND_NO, shader) { }

    public MeshData(Engine engine, string id, string srcMesh, string srcFarMesh, float scale, CONST_TV_BLENDINGMODE blendmode, string shader)
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
      SourceMesh = engine.TrueVision.TVGlobals.GetMesh(id);
      if (SourceMesh == null)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          SourceMesh = engine.TrueVision.TVScene.CreateMeshBuilder(id);

          if (srcMesh != null)
            SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcMesh), true);
          SourceMesh.Enable(false);
          SourceMesh.SetCollisionEnable(false);
          //SourceMesh.CompactMesh();
          SourceMesh.WeldVertices(0.001f, 0.001f);
          SourceMesh.ComputeBoundings();
          SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);
          SourceMesh.SetBlendingMode(blendmode); // CONST_TV_BLENDINGMODE.TV_BLEND_ALPHA);
        }
      }

      SourceFarMesh = engine.TrueVision.TVGlobals.GetMesh(farname);
      if (SourceFarMesh == null)
      {
        if (srcFarMesh != null)
        {
          using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          {
            SourceFarMesh = engine.TrueVision.TVScene.CreateMeshBuilder(farname);

            SourceFarMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcFarMesh), true);
            SourceFarMesh.Enable(false);
            SourceFarMesh.SetCollisionEnable(false);
            //SourceFarMesh.CompactMesh();
            SourceFarMesh.WeldVertices(0.01f, 0.01f);
            SourceFarMesh.ComputeBoundings();
            SourceMesh.SetBlendingMode(blendmode);
          }
        }
        else
          SourceFarMesh = SourceMesh; //.Duplicate();
      }
    }

    public MeshData(TVMesh mesh, float scale, MeshMode mode, CONST_TV_BLENDINGMODE blendmode, string data, string shader)
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

            float size = 1;
            if (!float.TryParse(Data[0], out size))
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

            float size = 1;
            if (!float.TryParse(Data[0], out size))
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

            float size = 1;
            if (!float.TryParse(Data[0], out size))
              break;

            int columns = 1;
            if (!int.TryParse(Data[2], out columns))
              break;

            int rows = 1;
            if (!int.TryParse(Data[3], out rows))
              break;

            string texname = Data[1];
            this = MeshDataDecorator.CreateBillboardAtlasAnimation(engine, id, size, texname, BlendMode, columns, rows, Shader);
          }
          break;
      }
    }

/*
    public void LoadFromINI(Engine engine, INIFile f, string sectionname, string id)
    {
      MeshMode mode = f.GetEnum(sectionname, "Mode", Mode);
      float scale = f.GetFloat(sectionname, "Scale", Scale);
      string shader = f.GetString(sectionname, "Shader", Shader);
      CONST_TV_BLENDINGMODE blendmode = f.GetEnum(sectionname, "BlendMode", BlendMode);

      switch (mode)
      {
        case MeshMode.NONE:
          this = Default;
          break;

        case MeshMode.NORMAL:
          {
            string srcMesh = f.GetString(sectionname, "SourceMeshPath", SourceMeshPath);
            string srcFarMesh = f.GetString(sectionname, "SourceFarMeshPath", SourceFarMeshPath);
            this = new MeshData(engine, id, srcMesh, srcFarMesh, scale, blendmode, shader);
          }
          break;

        case MeshMode.HORIZON:
          {
            string[] data = f.GetStringArray(sectionname, "Data", null);
            float size = 1;
            if (data != null 
              && data.Length >= 2
              && float.TryParse(data[0], out size))
              this = MeshDataDecorator.CreateHorizon(engine, id, size, data[1], blendmode, shader);
          }
          break;

        case MeshMode.TEX_MOD:
          {
            string[] data = f.GetStringArray(sectionname, "Data", null);
            if (data != null
              && data.Length >= 2)
              this = MeshDataDecorator.CreateTexturedModel(engine, id, data[0], data[1], blendmode, shader);
          }
          break;

        case MeshMode.ALPHATEX_WALL:
          {
            string[] data = f.GetStringArray(sectionname, "Data", null);
            float size = 1;
            if (data != null 
              && data.Length >= 3
              && float.TryParse(data[0], out size))
              this = MeshDataDecorator.CreateAlphaTexturedWall(engine, id, size, data[1], data[2], blendmode, shader);
          }
          break;

        case MeshMode.BILLBOARD_ANIM:
          {
            string[] data = f.GetStringArray(sectionname, "Data", null);
            float size = 1;
            int columns = 1;
            int rows = 1;
            if (data != null 
              && data.Length >= 4
              && float.TryParse(data[0], out size)
              && int.TryParse(data[2], out columns)
              && int.TryParse(data[3], out rows)
              )
              this = MeshDataDecorator.CreateBillboardAtlasAnimation(engine, id, size, data[1], blendmode, columns, rows, shader);
          }
          break;
      }
    }
    
    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetEnum(sectionname, "Mode", Mode);
      f.SetFloat(sectionname, "Scale", Scale);
      f.SetString(sectionname, "Shader", Shader);
      f.SetEnum(sectionname, "BlendMode", BlendMode);

      switch (Mode)
      {
        case MeshMode.NONE:
          break;

        case MeshMode.NORMAL:
          {
            f.SetString(sectionname, "SourceMeshPath", SourceMeshPath);
            f.SetString(sectionname, "SourceFarMeshPath", SourceFarMeshPath);
          }
          break;

        case MeshMode.HORIZON:
        case MeshMode.TEX_MOD:
        case MeshMode.ALPHATEX_WALL:
        case MeshMode.BILLBOARD_ANIM:
          {
            f.SetString(sectionname, "Data", SourceMeshPath);
          }
          break;
      }
    }
    */
  }
  

  internal static class MeshDataDecorator
  {
    //private static TrueVision tv = Globals.Engine.TrueVision;

    public static MeshData CreateHorizon(Engine engine, string id, float size, string texname, CONST_TV_BLENDINGMODE blendmode, string shader = null)
    {
      TVMesh m = engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        m = engine.TrueVision.TVScene.CreateMeshBuilder(id);

        string texpath = Path.Combine(Globals.ImagePath, texname);
        int tex = LoadAlphaTexture(engine, texname, texpath);

        m.AddFloor(tex, -size, -size, size, size);
        m.SetTexture(tex);
        m.SetCullMode(CONST_TV_CULLING.TV_DOUBLESIDED);
        //m.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
      }
      return new MeshData(m, 1, MeshMode.HORIZON, blendmode, "{0},{1}".F(size, texname), shader);
    }

    public static MeshData CreateTexturedModel(Engine engine, string id, string modelpath, string texname, CONST_TV_BLENDINGMODE blendmode, string shader = null)
    {
      TVMesh m = engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        m = engine.TrueVision.TVScene.CreateMeshBuilder(id);
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int itex = LoadTexture(engine, texname, texpath);

        m.LoadXFile(Path.Combine(Globals.ModelPath, modelpath), true);
        m.SetTexture(itex);
      }
      return new MeshData(m, 1, MeshMode.TEX_MOD, blendmode, "{0},{1}".F(modelpath, texname), shader);
    }

    public static MeshData CreateAlphaTexturedWall(Engine engine, string id, float size, string texname, string alphatexname, CONST_TV_BLENDINGMODE blendmode, string shader = null)
    {
      TVMesh m = engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          m = engine.TrueVision.TVScene.CreateMeshBuilder(id);

        // 1 texture
        int tex = 0;

        string texpath = Path.Combine(Globals.ImagePath, texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname);
        if (engine.TrueVision.TVGlobals.GetTex(texname) == 0)
        {
          int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
          tex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }

        m.AddWall(tex, -size / 2, 0, size / 2, 0, size, -size / 2);
        m.SetTexture(tex);
      }
      return new MeshData(m, 1, MeshMode.ALPHATEX_WALL, blendmode, "{0},{1},{2}".F(size, texname, alphatexname), shader);
    }

    public static MeshData CreateAlphaTexturedFlickerWall(Engine engine, string id, float size, string texname, string texdname, string alphatexname, CONST_TV_BLENDINGMODE blendmode, int frames, int[] flickerframes, ref int[] texanimframes)
    {
      TVMesh m = engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          m = engine.TrueVision.TVScene.CreateMeshBuilder(id);

        string texpath = Path.Combine(Globals.ImagePath, texname);
        string texdpath = Path.Combine(Globals.ImagePath, texdname);
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname);

        int dstartex = 0;
        int dstardtex = 0;
        if (engine.TrueVision.TVGlobals.GetTex(texname) == 0)
        {
          int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath); // note we are loading alpha map as texture
          dstartex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }

        if (engine.TrueVision.TVGlobals.GetTex(texdname) == 0)
        {
          int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texdpath);
          int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
          dstardtex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texdname);
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
      return new MeshData(m, 1, MeshMode.ALPHATEX_FLICKERWALL, blendmode, null, null);
    }

    public static MeshData CreateBillboardAtlasAnimation(Engine engine, string id, float size, string texname, CONST_TV_BLENDINGMODE blendmode, int columns, int rows, string shader = null)
    {
      TVMesh m = engine.TrueVision.TVGlobals.GetMesh(id);
      if (m == null)
      {
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int i = LoadAlphaTexture(engine, texname, texpath);
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          m = engine.TrueVision.TVScene.CreateBillboard(i, 0, 0, 0, size, size, id, true);
        m.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        m.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);
        m.SetTextureModEnable(true);
        m.SetTextureModTranslationScale(1f / columns, 1f / rows);
      }
      return new MeshData(m, 1, MeshMode.BILLBOARD_ANIM, blendmode, "{0},{1},{2},{3}".F(size, texname, columns, rows), shader);
    }

    private static int LoadAlphaTexture(Engine engine, string id, string texpath, string alphatexpath = null)
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        int tex = engine.TrueVision.TVGlobals.GetTex(id);
        if (tex == 0)
        {
          int texS = engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath ?? texpath); //LoadAlphaTexture
          tex = engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, id);
        }
        return tex;
      }
    }

    private static int LoadTexture(Engine engine, string id, string texpath)
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        int tex = engine.TrueVision.TVGlobals.GetTex(id);
        if (tex == 0)
          tex = engine.TrueVision.TVTextureFactory.LoadTexture(texpath, id);
        return tex;
      }
    }

    /*
    private static void LoadAlphaTextureFromFolder(Engine engine, ref int[] texanimframes, string mainPath, string subPath)
    {
      List<int> frames = new List<int>();
      string folderPath = Path.Combine(mainPath, subPath);
      foreach (string texpath in Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly))
      {
        string texname = Path.Combine(subPath, Path.GetFileName(texpath));
        frames.Add(LoadAlphaTexture(engine, texname, texpath));
      }
      texanimframes = frames.ToArray();
    }

    private static void LoadTextureFromFolder(Engine engine, ref int[] texanimframes, string mainPath, string subPath)
    {
      List<int> frames = new List<int>();
      string folderPath = Path.Combine(mainPath, subPath);
      foreach (string texpath in Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly))
      {
        string texname = Path.Combine(subPath, Path.GetFileName(texpath));
        frames.Add(LoadTexture(engine, texname, texpath));
      }
      texanimframes = frames.ToArray();
    }
    */
  }
}

