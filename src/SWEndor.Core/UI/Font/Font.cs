using MTV3D65;
using SWEndor.Core;
using Primrose.FileFormat.INI;
using Primrose.Primitives.Extensions;
using System;
using System.IO;

namespace SWEndor.UI
{
  public partial class Font
  {
    private Font(Engine engine
               , byte key
               , string fontname
               , int size = 10
               , FontMode mode = FontMode.NONE
               )
    {
      ID = engine.TrueVision.TVScreen2DText.TextureFont_Create(key.ToString()
                                                            , fontname
                                                            , size
                                                            , mode.Has(FontMode.BOLD)
                                                            , mode.Has(FontMode.UNDERLINED)
                                                            , mode.Has(FontMode.ITALIC)
                                                            , true);

      generateWidth(engine);
      //SaveToFiles(engine, fontname + "_" + size.ToString(), CONST_TV_IMAGEFORMAT.TV_IMAGE_DDS);
    }

    private Font(Engine engine
           , byte key
           , string name
           )
    {
      Load(engine, name, out ID);

      generateWidth(engine);
    }

    // we actually don't need to store all those setting variables as the created font cannot be changed
    public readonly int ID;

    public float GetWidth(string text)
    {
      float w = 0;
      for (int i = 0; i < text.Length; i++)
        w += width[text[i]];
      return w;
    }

    private float[] width;

    private void generateWidth(Engine engine)
    {
      width = new float[256];
      float dummy = 0;
      for (int b = 0; b <= byte.MaxValue; b++)
        engine.TrueVision.TVScreen2DText.TextureFont_GetTextSize(((char)b).ToString(), ID, ref width[b], ref dummy);
    }

    public void SaveToFiles(Engine engine, string name, CONST_TV_IMAGEFORMAT texFormat)
    {
      string ext = texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_BMP ? "bmp"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_DDS ? "dds"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_DXT1 ? "dds"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_DXT3 ? "dds"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_DXT5 ? "dds"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_HDR ? "hdr"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_JPG ? "jpg"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_PNG ? "png"
              : texFormat == CONST_TV_IMAGEFORMAT.TV_IMAGE_TVT ? "tvt" : "raw";

      string texpath = Path.Combine(Globals.FontPath, "{0}.{1}".F(name, ext));
      engine.TrueVision.TVTextureFactory.SaveTexture(engine.TrueVision.TVScreen2DText.TextureFont_GetTexture(ID), texpath, texFormat);
      string datapath = Path.Combine(Globals.FontPath, "{0}.txt".F(name));
      if (!File.Exists(datapath))
        File.Create(datapath).Close();

      INIFile f = new INIFile(datapath);
      TV_TEXTUREFONT_CHAR[] p = new TV_TEXTUREFONT_CHAR[255];
      int num = 0;
      engine.TrueVision.TVScreen2DText.TextureFont_GetFontData(ID, ref num, p);

      for (int i = 0; i < num; i++)
      {
        TV_TEXTUREFONT_CHAR c = p[i];
        f.SetValue(INIFile.PreHeaderSectionName, c.iAsciiChar.ToString(), new float[] { c.fX1, c.fX2, c.fY1, c.fY2 });
      }
      f.WriteToFile(datapath);
    }

    public void Load(Engine engine, string name, out int iFnt)
    {
      string[] exts = new string[] { "dds", "bmp", "hdr", "png", "jpg", "tvt", "raw" };
      string texpath = "";
      foreach (string ext in exts)
      {
        texpath = Path.Combine(Globals.FontPath, "{0}.{1}".F(name, ext));
        if (File.Exists(texpath))
          break;
      }

      int iTex = engine.TrueVision.TVTextureFactory.LoadTexture(texpath);

      TV_TEXTUREFONT_CHAR[] p = new TV_TEXTUREFONT_CHAR[255];
      string datapath = Path.Combine(Globals.FontPath, "{0}.txt".F(name));
      int iNum = 0;
      if (File.Exists(datapath))
      {
        INIFile f = new INIFile(datapath);
        foreach (INIFile.INISection.INILine ln in f.GetSection(INIFile.PreHeaderSectionName).Lines)
        {
          int chr = 0;
          if (int.TryParse(ln.Key, out chr))
          {
            float[] fs = f.GetValue<float[]>(INIFile.PreHeaderSectionName, ln.Key, null);
            if (fs != null && fs.Length >= 4)
              p[iNum++] = new TV_TEXTUREFONT_CHAR()
              {
                iAsciiChar = chr,
                fX1 = fs[0],
                fX2 = fs[1],
                fY1 = fs[2],
                fY2 = fs[3]
              };
          }
        }
      }

      iFnt = engine.TrueVision.TVScreen2DText.TextureFont_CreateCustom(name
                                                      , iTex
                                                      , iNum
                                                      , p);
    }
  }

  /// <summary>
  /// Bitwise Font properties 
  /// </summary>
  [Flags]
  public enum FontMode : byte
  {
    /// <summary>The font is of the default format</summary>
    NONE = 0,

    /// <summary>The font is bolded</summary>
    BOLD = 0x1,

    /// <summary>The font is underlined</summary>
    UNDERLINED = 0x2,

    /// <summary>The font is italicized</summary>
    ITALIC = 0x4,
  }

  /// <summary>
  /// Provides extension methods for FontMode enum
  /// </summary>
  public static class FontModeExt
  {
    /// <summary>Returns whether a font mask contains a subset</summary>
    /// <param name="mask">The mask to compare</param>
    /// <param name="subset">The subset to compare</param>
    /// <returns>Returns true if the mask contains all bits of the subset, false if otherwise</returns>
    public static bool Has(this FontMode mask, FontMode subset) { return (mask & subset) == subset; }
  }
}
