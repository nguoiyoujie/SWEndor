using MTV3D65;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.Primitives.Extensions
{
  public static class INIFileExts
  {
    public static TV_2DVECTOR GetTV_2DVECTOR(this INIFile f, string section, string key, TV_2DVECTOR defaultValue = default(TV_2DVECTOR))
    {
      float[] list = f.GetValue(section, key, null, new float[0]);
      if (list.Length >= 2)
        return new TV_2DVECTOR(list[0], list[1]);
      return defaultValue;
    }

    public static TV_3DVECTOR GetTV_3DVECTOR(this INIFile f, string section, string key, TV_3DVECTOR defaultValue = default(TV_3DVECTOR))
    {
      float[] list = f.GetValue(section, key, null, new float[0]);
      if (list.Length >= 2)
        return new TV_3DVECTOR(list[0], list[1], list[2]);
      return defaultValue;
    }

    private static string[] ConvertToStringArray(TV_2DVECTOR v)
    {
      string[] ret = new string[2];
      ret[0] = v.x.ToString();
      ret[1] = v.y.ToString();
      return ret;
    }

    private static string[] ConvertToStringArray(TV_3DVECTOR v)
    {
      string[] ret = new string[3];
      ret[0] = v.x.ToString();
      ret[1] = v.y.ToString();
      ret[2] = v.z.ToString();
      return ret;
    }

    public static void SetTV_2DVECTOR(this INIFile f, string section, string key, TV_2DVECTOR value, char delimiter = ',') { f.SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }
    public static void SetTV_3DVECTOR(this INIFile f, string section, string key, TV_3DVECTOR value, char delimiter = ',') { f.SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }
  }
}
