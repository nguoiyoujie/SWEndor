using MTV3D65;
using System.Collections.Generic;
using System.IO;

namespace SWEndor
{
  public class FlareInfo
  {
    public FlareInfo(TVTextureFactory texturefactory, string texpath)
    {
      TexID = texturefactory.LoadTexture(texpath, Path.GetFileNameWithoutExtension(texpath), -1, -1, CONST_TV_COLORKEY.TV_COLORKEY_NO, true);
    }

    //~FlareInfo()
    //{
      //if (Globals.Engine.TrueVision.TVTextureFactory != null)
      //  Globals.Engine.TrueVision.TVTextureFactory.DeleteTexture(TexID);
    //}

    public int TexID = -1;
    public float Size = 10;
    public float LinePosition = 40;
    public TV_COLOR NormalColor = new TV_COLOR(1, 1, 1, 0.5f);
    public TV_COLOR SpecularColor = new TV_COLOR(1, 1, 1, 0.5f);
  }

  public class AtmosphereInfo
  {
    public readonly Engine Engine;
    internal AtmosphereInfo(Engine engine)
    {
      Engine = engine;
      m_atmosphere = new TVAtmosphere();
    }

    private TVAtmosphere m_atmosphere;
    private int m_tex = -1;
    private string m_texpath = "";
    private float m_radius = 60000;

    private int m_Sun = -1;
    private string m_sunpath = "";
    private TV_3DVECTOR m_SunPosition = new TV_3DVECTOR();
    private FlareInfo[] m_flares = new FlareInfo[0];

    private bool m_enabled = true;
    private bool m_showSun = true;
    private bool m_showFlare = true;

    public bool Enabled
    {
      get { return m_enabled; }
      set
      {
        if (m_enabled != value)
        {
          m_enabled = value;
          Load();
        }
      }
    }

    public bool ShowSun
    {
      get { return m_showSun; }
      set
      {
        if (m_showSun != value)
        {
          m_showSun = value;
          Load();
        }
      }
    }

    public bool ShowFlare
    {
      get { return m_showFlare; }
      set
      {
        if (m_showFlare != value)
        {
          m_showFlare = value;
          Load();
        }
      }
    }

    public void Load()
    {
      if (m_enabled)
      {
        LoadTex();
        if (m_showSun)
        {
          LoadSun();
          if (m_showFlare)
            LoadFlares();
          else
            m_atmosphere.LensFlare_Enable(false);
        }
        else
        {
          m_atmosphere.Sun_Enable(false);
          m_atmosphere.LensFlare_Enable(false);
        }
      }
      else
      {
        m_atmosphere.SkySphere_Enable(false);
        m_atmosphere.Sun_Enable(false);
        m_atmosphere.LensFlare_Enable(false);
      }
    }

    public void SetTex_Sphere(string path, float radius)
    {
      if (m_texpath != path)
      {
        if (m_tex != -1)
          Engine.TrueVision.TVTextureFactory.DeleteTexture(m_tex);
        m_tex = Engine.TrueVision.TVTextureFactory.LoadTexture(path);
        m_texpath = path;
        m_radius = radius;
      }
    }
    
    public void LoadTex()
    {
      if (m_tex == -1)
        m_atmosphere.SkySphere_Enable(false);
      else
      {
        m_atmosphere.SkySphere_SetRadius(m_radius);
        m_atmosphere.SkySphere_SetTexture(m_tex);
        m_atmosphere.SkySphere_Enable(true);
      }
    }

    public void SetTex_Sun(string path)
    {
      if (m_sunpath != path)
      {
        if (m_Sun != -1)
          Engine.TrueVision.TVTextureFactory.DeleteTexture(m_Sun);
        m_Sun = Engine.TrueVision.TVTextureFactory.LoadTexture(path);
        m_sunpath = path;
      }
    }

    public void SetPos_Sun(TV_3DVECTOR pos)
    {
      m_SunPosition = pos;
    }

    public void LoadSun()
    {
      if (m_Sun == -1)
        m_atmosphere.Sun_Enable(false);
      else
      {
        m_atmosphere.Sun_SetTexture(m_Sun);
        m_atmosphere.Sun_SetBillboardSize(1);
        m_atmosphere.Sun_SetPosition(m_SunPosition.x, m_SunPosition.y, m_SunPosition.z);
        m_atmosphere.Sun_Enable(true);
      }
    }

    public void UnloadSun()
    {
      m_Sun = -1;
      m_atmosphere.Sun_Enable(false);
    }

    public void SetFlares(FlareInfo[] flares)
    {
      m_flares = flares;
    }

    public void LoadFlares()
    {
      if (m_flares == null || m_flares.Length == 0)
        m_atmosphere.LensFlare_Enable(false);
      else
      {
        m_atmosphere.LensFlare_SetLensNumber(m_flares.Length);
        m_atmosphere.LensFlare_Enable(true);
        for (int i = 0; i < m_flares.Length; i++)
          m_atmosphere.LensFlare_SetLensParams(i, m_flares[i].TexID, m_flares[i].Size, m_flares[i].LinePosition, m_flares[i].NormalColor.GetIntColor(), m_flares[i].SpecularColor.GetIntColor());
      }
    }

    public void UnloadFlares()
    {
      m_flares = new FlareInfo[0];
      m_atmosphere.LensFlare_Enable(false);
    }

    public void LoadDefaults(bool addSun, bool addFlares)
    {
      string path = Path.Combine(Globals.AtmospherePath, @"stars_big.bmp");
      SetTex_Sphere(path, 60000);
      if (m_enabled)
        LoadTex();

      if (addSun)
      {
        string pathSun = Path.Combine(Globals.AtmospherePath, @"sun.jpg");
        SetTex_Sun(pathSun);
        SetPos_Sun(new TV_3DVECTOR(1000, 250, 0));
        if (m_enabled)
          LoadSun();

        if (addFlares)
        {
          List<FlareInfo> fset = new List<FlareInfo>(); 
          for (int i = 1; i <= 4; i++)
          {
            FlareInfo finfo = new FlareInfo(Engine.TrueVision.TVTextureFactory, Path.Combine(Globals.AtmospherePath, @"flare" + i + ".jpg"));
            switch (i)
            {
              case 1:
                finfo.Size = 10;
                finfo.LinePosition = 80;
                break;
              case 2:
                finfo.Size = 2;
                finfo.LinePosition = 38;
                break;
              case 3:
                finfo.Size = 3.6f;
                finfo.LinePosition = 25;
                finfo.SpecularColor = new TV_COLOR(0.7f, 1, 1, 0.5f);
                break;
              case 4:
                finfo.Size = 2;
                finfo.LinePosition = 6;
                finfo.NormalColor = new TV_COLOR(1, 0.1f, 0, 0.5f);
                finfo.SpecularColor = new TV_COLOR(0.5f, 1, 1, 0.5f);
                break;
            }
            fset.Add(finfo);
          }
          SetFlares(fset.ToArray());
          if (m_enabled)
            LoadFlares();
        }
        else
        {
          m_atmosphere.LensFlare_Enable(false);
        }
      }
      else
      {
        m_atmosphere.Sun_Enable(false);
      }
    }

    public void Render()
    {
      if (m_enabled)
        m_atmosphere.Atmosphere_Render();
    }
  }
}
