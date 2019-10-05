using MTV3D65;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Components
{
  public enum MeshMode : byte
  {
    NONE, 
    NORMAL,
    HORIZON,
    TexturedModel,
    AlphaTexturedWall,
    AlphaTexturedFlickerWall,
    BillboardAtlasAnimation
  }

  public struct MeshData
  {
    private static TrueVision tv = Globals.Engine.TrueVision;

    public readonly string Name;
    public readonly string SourceMeshPath;
    public readonly string SourceFarMeshPath;
    public readonly TVMesh SourceMesh;
    public readonly TVMesh SourceFarMesh;
    public readonly TV_3DVECTOR MaxDimensions;
    public readonly TV_3DVECTOR MinDimensions;
    public readonly float Scale;
    public readonly MeshMode Mode;
    public readonly string Shader;

    public readonly static MeshData Default = new MeshData("", tv.TVScene.CreateMeshBuilder(), 1, MeshMode.NONE, null, null);

    public MeshData(string name, string srcMesh) : this(name, srcMesh, null, 1, null) { }

    public MeshData(string name, string srcMesh, float scale) : this(name, srcMesh, null, scale, null) { }

    public MeshData(string name, string srcMesh, float scale, string shader) : this(name, srcMesh, null, scale, shader) { }

    public MeshData(string name, string srcMesh, string srcFarMesh, float scale, string shader)
    {
      Name = name;
      string farname = name + "_far";
      SourceMeshPath = srcMesh;
      SourceFarMeshPath = srcFarMesh;
      MinDimensions = new TV_3DVECTOR();
      MaxDimensions = new TV_3DVECTOR();
      Scale = scale;
      Mode = MeshMode.NORMAL;
      Shader = shader;

      // create SourceMesh and SourceFarMesh
      SourceMesh = tv.TVGlobals.GetMesh(name);
      if (SourceMesh == null)
      {
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          SourceMesh = tv.TVScene.CreateMeshBuilder(name);

          if (srcMesh != null)
            SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcMesh), true);
          SourceMesh.Enable(false);
          SourceMesh.SetCollisionEnable(false);
          //SourceMesh.CompactMesh();
          SourceMesh.WeldVertices(0.001f, 0.001f);
          SourceMesh.ComputeBoundings();
          SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);
        }
      }

      SourceFarMesh = tv.TVGlobals.GetMesh(farname);
      if (SourceFarMesh == null)
      {
        if (srcFarMesh != null)
        {
          using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          {
            SourceFarMesh = tv.TVScene.CreateMeshBuilder(farname);

            SourceFarMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcFarMesh), true);
            SourceFarMesh.Enable(false);
            SourceFarMesh.SetCollisionEnable(false);
            //SourceFarMesh.CompactMesh();
            SourceFarMesh.WeldVertices(0.01f, 0.01f);
            SourceFarMesh.ComputeBoundings();
          }
        }
        else
          SourceFarMesh = SourceMesh; //.Duplicate();
      }
    }

    public MeshData(string name, TVMesh mesh, float scale, MeshMode mode, string data, string shader)
    {
      Name = name;
      SourceMeshPath = data;
      SourceFarMeshPath = null;
      MinDimensions = new TV_3DVECTOR();
      MaxDimensions = new TV_3DVECTOR();
      Scale = scale;
      Mode = mode;
      Shader = shader;

      SourceMesh = mesh;
      SourceFarMesh = mesh;
      SourceFarMesh.Enable(false);
      SourceMesh.SetCollisionEnable(false);
      SourceMesh.WeldVertices();
      SourceMesh.ComputeBoundings();
      SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      MeshMode mode = f.GetEnumValue(sectionname, "Mode", Mode);
      string name = f.GetStringValue(sectionname, "Name", Name);
      float scale = f.GetFloatValue(sectionname, "Scale", Scale);
      string shader = f.GetStringValue(sectionname, "Shader", Shader);

      switch (mode)
      {
        case MeshMode.NONE:
          this = Default;
          break;

        case MeshMode.NORMAL:
          {
            string srcMesh = f.GetStringValue(sectionname, "SourceMeshPath", SourceMeshPath);
            string srcFarMesh = f.GetStringValue(sectionname, "SourceFarMeshPath", SourceFarMeshPath);
            this = new MeshData(name, srcMesh, srcFarMesh, scale, shader);
          }
          break;

        case MeshMode.HORIZON:
          {
            string data = f.GetStringValue(sectionname, "Data", SourceMeshPath);
            if (data != null)
              this = MeshDataDecorator.CreateHorizon(name, scale, data, shader);
          }
          break;

        case MeshMode.AlphaTexturedWall:
          {
            string[] data = f.GetStringList(sectionname, "Data", null);
            if (data != null && data.Length >= 2)
              this = MeshDataDecorator.CreateAlphaTexturedWall(name, scale, data[0], data[1], shader);
          }
          break;

        case MeshMode.BillboardAtlasAnimation:
          {
            string[] data = f.GetStringList(sectionname, "Data", null);
            int columns = 1;
            int rows = 1;
            if (data != null 
              && data.Length >= 3
              && int.TryParse(data[1], out columns)
              && int.TryParse(data[2], out rows)
              )
              this = MeshDataDecorator.CreateBillboardAtlasAnimation(name, scale, data[0], columns, rows, shader);
          }
          break;
      }
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      f.SetEnumValue(sectionname, "Mode", Mode);
      f.SetStringValue(sectionname, "Name", Name);
      f.SetFloatValue(sectionname, "Scale", Scale);
      f.SetStringValue(sectionname, "Shader", Shader);

      switch (Mode)
      {
        case MeshMode.NONE:
          break;

        case MeshMode.NORMAL:
          {
            f.SetStringValue(sectionname, "SourceMeshPath", SourceMeshPath);
            f.SetStringValue(sectionname, "SourceFarMeshPath", SourceFarMeshPath);
          }
          break;

        case MeshMode.HORIZON:
          {
            f.SetStringValue(sectionname, "Data", SourceMeshPath);
          }
          break;

        case MeshMode.AlphaTexturedWall:
          {
            f.SetStringValue(sectionname, "Data", SourceMeshPath);
          }
          break;

        case MeshMode.BillboardAtlasAnimation:
          {
            f.SetStringValue(sectionname, "Data", SourceMeshPath);
          }
          break;
      }
    }
  }

  public static class MeshDataDecorator
  {
    private static TrueVision tv = Globals.Engine.TrueVision;

    public static MeshData CreateHorizon(string name, float size, string texname, string shader = null)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
        m = tv.TVScene.CreateMeshBuilder(name);

        string texpath = Path.Combine(Globals.ImagePath, texname);
        int tex = LoadAlphaTexture(texname, texpath);

        m.AddFloor(tex, -size, -size, size, size);
        m.SetTexture(tex);
        m.SetCullMode(CONST_TV_CULLING.TV_DOUBLESIDED);
        m.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
      }
      return new MeshData(name, m, 1, MeshMode.HORIZON, texname, shader);
    }

    public static MeshData CreateTexturedModel(string name, string modelpath, string texname, string shader = null)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
        m = tv.TVScene.CreateMeshBuilder(name);
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int itex = LoadTexture(texname, texpath);

        m.LoadXFile(Path.Combine(Globals.ModelPath, modelpath), true);
        m.SetTexture(itex);
      }
      return new MeshData(name, m, 1, MeshMode.TexturedModel, "{0},{1}".F(modelpath, texname), shader);
    }

    public static MeshData CreateAlphaTexturedWall(string name, float size, string texname, string alphatexname, string shader = null)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          m = tv.TVScene.CreateMeshBuilder(name);

        // 1 texture
        int tex = 0;

        string texpath = Path.Combine(Globals.ImagePath, texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname);
        if (tv.TVGlobals.GetTex(texname) == 0)
        {
          int texS = tv.TVTextureFactory.LoadTexture(texpath);
          int texA = tv.TVTextureFactory.LoadTexture(alphatexpath);
          tex = tv.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }

        m.AddWall(tex, -size / 2, 0, size / 2, 0, size, -size / 2);
        m.SetTexture(tex);
      }
      return new MeshData(name, m, 1, MeshMode.AlphaTexturedWall, "{0},{1}".F(texname, alphatexname), shader);
    }

    public static MeshData CreateAlphaTexturedFlickerWall(string name, float size, string texname, string texdname, string alphatexname, int frames, int[] flickerframes, ref int[] texanimframes)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          m = tv.TVScene.CreateMeshBuilder(name);

        string texpath = Path.Combine(Globals.ImagePath, texname);
        string texdpath = Path.Combine(Globals.ImagePath, texdname);
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname);

        int dstartex = 0;
        int dstardtex = 0;
        if (tv.TVGlobals.GetTex(texname) == 0)
        {
          int texS = tv.TVTextureFactory.LoadTexture(texpath);
          int texA = tv.TVTextureFactory.LoadTexture(alphatexpath); // note we are loading alpha map as texture
          dstartex = tv.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }

        if (tv.TVGlobals.GetTex(texdname) == 0)
        {
          int texS = tv.TVTextureFactory.LoadTexture(texdpath);
          int texA = tv.TVTextureFactory.LoadTexture(alphatexpath);
          dstardtex = tv.TVTextureFactory.AddAlphaChannel(texS, texA, texdname);
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
      return new MeshData(name, m, 1, MeshMode.AlphaTexturedFlickerWall, null, null);
    }

    public static MeshData CreateBillboardAtlasAnimation(string name, float size, string texname, int columns, int rows, string shader = null)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
        string texpath = Path.Combine(Globals.ImagePath, texname);
        int i = LoadAlphaTexture(texname, texpath);
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          m = tv.TVScene.CreateBillboard(i, 0, 0, 0, size, size, name, true);
        m.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        m.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);
        m.SetTextureModEnable(true);
        m.SetTextureModTranslationScale(1f / columns, 1f / rows);
      }
      return new MeshData(name, m, 1, MeshMode.BillboardAtlasAnimation, "{0},{1},{2}".F(texname, columns, rows), shader);
    }

    private static int LoadAlphaTexture(string name, string texpath, string alphatexpath = null)
    {
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        int tex = tv.TVGlobals.GetTex(name);
        if (tex == 0)
        {
          int texS = tv.TVTextureFactory.LoadTexture(texpath);
          int texA = tv.TVTextureFactory.LoadTexture(alphatexpath ?? texpath); //LoadAlphaTexture
          tex = tv.TVTextureFactory.AddAlphaChannel(texS, texA, name);
        }
        return tex;
      }
    }

    private static int LoadTexture(string name, string texpath)
    {
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        int tex = tv.TVGlobals.GetTex(name);
        if (tex == 0)
          tex = tv.TVTextureFactory.LoadTexture(texpath, name);
        return tex;
      }
    }

    private static void LoadAlphaTextureFromFolder(ref int[] texanimframes, string mainPath, string subPath)
    {
      List<int> frames = new List<int>();
      string folderPath = Path.Combine(mainPath, subPath);
      foreach (string texpath in Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly))
      {
        string texname = Path.Combine(subPath, Path.GetFileName(texpath));
        frames.Add(LoadAlphaTexture(texname, texpath));
      }
      texanimframes = frames.ToArray();
    }

    private static void LoadTextureFromFolder(ref int[] texanimframes, string mainPath, string subPath)
    {
      List<int> frames = new List<int>();
      string folderPath = Path.Combine(mainPath, subPath);
      foreach (string texpath in Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly))
      {
        string texname = Path.Combine(subPath, Path.GetFileName(texpath));
        frames.Add(LoadTexture(texname, texpath));
      }
      texanimframes = frames.ToArray();
    }
  }
}

