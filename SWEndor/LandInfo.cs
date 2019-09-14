using MTV3D65;
using SWEndor.Primitives;
using System.IO;

namespace SWEndor
{
  public class LandInfo
  {
    public readonly Engine Engine;
    internal LandInfo(Engine engine)
    {
      Engine = engine;
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        m_land = Engine.TrueVision.TVScene.CreateLandscape("Land");
    }

    private TVLandscape m_land;
    public TVLandscape Land { get { return m_land; } }
    private int m_tex = -1;
    private string m_texpath = "";
    private string m_heightmap = "";
    private float xzScale = 250;
    private float yBottom = -1500;
    private float yScale = 20;
    private float texScale = 1;
    private bool m_enabled = true;

    public bool Enabled
    {
      get { return m_enabled; }
      set
      {
        if (m_enabled != value)
        {
          m_enabled = value;
          if (m_enabled)
            Load();
          else
            Unload();
        }
      }
    }

    public void SetTex(string path, string heightmap)
    {
      if (m_texpath != path)
      {
        if (m_tex != -1)
          Engine.TrueVision.TVTextureFactory.DeleteTexture(m_tex);
        m_tex = Engine.TrueVision.TVTextureFactory.LoadTexture(path, "Land", -1, -1);
        m_heightmap = heightmap;
      }
    }

    public void Load()
    {
      if (m_tex == -1)
        m_land.Enable(false);
      else
      {
        TV_TEXTURE tex = Engine.TrueVision.TVTextureFactory.GetTextureInfo(m_tex);
        int height = 256;// tex.Height;
        int width = 256;// tex.Width;

        if (!m_land.GenerateTerrain(m_heightmap, CONST_TV_LANDSCAPE_PRECISION.TV_PRECISION_ULTRA, 1, 1, -width * xzScale / 2, yBottom, -height * xzScale / 2, true))
          m_land.CreateEmptyTerrain(CONST_TV_LANDSCAPE_PRECISION.TV_PRECISION_AVERAGE, 1, 1, -width * xzScale / 2, yBottom, -height * xzScale / 2);

        m_land.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_NORMAL);

        m_land.SetMaterial(Engine.TrueVision.TVMaterialFactory.CreateLightMaterial(1, 1, 1, 1, 0, 1));
        m_land.SetScale(xzScale, yScale, xzScale);
        m_land.SetTextureScale(texScale, texScale);
        m_land.SetCollisionEnable(true);
        m_land.SetTexture(m_tex);
        m_land.ComputeNormals(true);

        m_land.Enable(true);

        //m_land.SaveTerrainData(Path.Combine(Globals.LandscapePath, @"land.txt"), CONST_TV_LANDSAVE.TV_LANDSAVE_HEIGHT);
      }
    }

    public void Unload()
    {
      m_land.Enable(false);
      m_land.Destroy();
      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        m_land = Engine.TrueVision.TVScene.CreateLandscape("Land");
    }

    public void LoadDefaults()
    {
      string path = Path.Combine(Globals.LandscapePath, @"texmap.png");
      string heightmap = Path.Combine(Globals.LandscapePath, @"heightmap.png");
      SetTex(path, heightmap);
      if (m_enabled)
        Load();
    }

    public void Render()
    {
      if (m_enabled)
        m_land.Render();
    }

    public float GetLandHeight(float fX, float fZ)
    {
      if (m_enabled)
        return m_land.GetHeight(fX, fZ);
      else
        return 0;
    }
  }
}
