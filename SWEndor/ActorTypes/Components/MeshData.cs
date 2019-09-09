using MTV3D65;
using SWEndor.Primitives;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Components
{
  public struct MeshData
  {
    private static TrueVision tv = Globals.Engine.TrueVision;

    public readonly string SourceMeshPath;
    public readonly string SourceFarMeshPath;
    public readonly TVMesh SourceMesh;
    public readonly TVMesh SourceFarMesh;
    public readonly TV_3DVECTOR MaxDimensions;
    public readonly TV_3DVECTOR MinDimensions;
    //public TV_3DVECTOR Size { get { return MaxDimensions - MinDimensions; } }

    public readonly static MeshData Default = new MeshData("", tv.TVScene.CreateMeshBuilder());

    public MeshData(string name, string srcMesh) : this(name, srcMesh, null) { }

    public MeshData(string name, string srcMesh, string srcFarMesh)
    {
      SourceMeshPath = srcMesh;
      SourceFarMeshPath = srcFarMesh;
      MinDimensions = new TV_3DVECTOR();
      MaxDimensions = new TV_3DVECTOR();

      // create SourceMesh and SourceFarMesh
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_RENDER))
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_COLLISION))
      {
        SourceMesh = tv.TVGlobals.GetMesh(name);
        if (SourceMesh == null)
        {
          SourceMesh = tv.TVScene.CreateMeshBuilder(name);
          //SourceMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_BUMPMAPPING_TANGENTSPACE, 8);

          if (srcMesh != null)
            SourceMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcMesh), true);
          SourceMesh.Enable(false);
          SourceMesh.SetCollisionEnable(false);
          SourceMesh.WeldVertices();
          SourceMesh.ComputeBoundings();
          SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);
        }

        SourceFarMesh = tv.TVGlobals.GetMesh(name + "_far");
        if (SourceFarMesh == null)
        {
          if (srcFarMesh != null)
          {
            SourceFarMesh = tv.TVScene.CreateMeshBuilder(name + "_far");
            SourceFarMesh.LoadXFile(Path.Combine(Globals.ModelPath, srcFarMesh), true);
            SourceFarMesh.Enable(false);
            SourceMesh.SetCollisionEnable(false);
            SourceFarMesh.WeldVertices();
            SourceFarMesh.ComputeBoundings();
          }
          else
            SourceFarMesh = SourceMesh; //.Duplicate();
        }
      }
    }

    public MeshData(string name, TVMesh mesh)
    {
      SourceMeshPath = null;
      SourceFarMeshPath = null;
      MinDimensions = new TV_3DVECTOR();
      MaxDimensions = new TV_3DVECTOR();

      SourceMesh = mesh;
      SourceFarMesh = mesh;
      SourceFarMesh.Enable(false);
      SourceMesh.SetCollisionEnable(false);
      SourceFarMesh.WeldVertices();
      SourceFarMesh.ComputeBoundings();
      SourceMesh.GetBoundingBox(ref MinDimensions, ref MaxDimensions);
    }

  }

  public static class MeshDataDecorator
  {
    private static TrueVision tv = Globals.Engine.TrueVision;

    public static MeshData CreateHorizon(string name, float size, string texname)
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
      return new MeshData(name, m);
    }

    public static MeshData CreateTexturedModel(string name, string modelpath, string texname)
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
      return new MeshData(name, m);
    }

    public static MeshData CreateAlphaTexturedWall(string name, float size, string texname, string alphatexname)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
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
      return new MeshData(name, m);
    }


    public static MeshData CreateAlphaTexturedFlickerWall(string name, float size, string texname, string texdname, string alphatexname, int frames, int[] flickerframes, ref int[] texanimframes)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
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
      return new MeshData(name, m);
    }

    public static MeshData CreateBillboardAnimation(string name, float size, string texfolder, ref int[] texanimframes)
    {
      TVMesh m = tv.TVGlobals.GetMesh(name);
      if (m == null)
      {
        LoadAlphaTextureFromFolder(ref texanimframes, Globals.ImagePath, texfolder);
        m = tv.TVScene.CreateBillboard(texanimframes[0], 0, 0, 0, size, size, name, true);
        m.SetBlendingMode(CONST_TV_BLENDINGMODE.TV_BLEND_ADD);
        m.SetBillboardType(CONST_TV_BILLBOARDTYPE.TV_BILLBOARD_FREEROTATION);
      }
      return new MeshData(name, m);
    }

    public static int LoadAlphaTexture(string name, string texpath, string alphatexpath = null)
    {
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_RENDER))
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

    public static int LoadTexture(string name, string texpath)
    {
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_RENDER))
      {
        int tex = tv.TVGlobals.GetTex(name);
        if (tex == 0)
          tex = tv.TVTextureFactory.LoadTexture(texpath, name);
        return tex;
      }
    }

    public static void LoadAlphaTextureFromFolder(ref int[] texanimframes, string mainPath, string subPath)
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

    public static void LoadTextureFromFolder(ref int[] texanimframes, string mainPath, string subPath)
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

