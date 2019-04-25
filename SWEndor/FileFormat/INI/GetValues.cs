using MTV3D65;
using System;
using System.Text;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    public string GetStringValue(string section, string key, string defaultValue = "")
    {
      if (Sections.ContainsKey(section))
      {
        INILine line = Sections[section].GetLine(key);
        if (line != null)
          return line.Value;
      }
      return defaultValue;
    }

    public int GetIntValue(string section, string key, int defaultValue = 0)
    {
      int ret = defaultValue;
      string s = GetStringValue(section, key, defaultValue.ToString());
      if (s != null && int.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    public uint GetUIntValue(string section, string key, uint defaultValue = 0)
    {
      uint ret = defaultValue;
      string s = GetStringValue(section, key, defaultValue.ToString());
      if (s != null && uint.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    public float GetFloatValue(string section, string key, float defaultValue = 0)
    {
      float ret = defaultValue;
      string s = GetStringValue(section, key, defaultValue.ToString());
      if (s != null && float.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    public double GetDoubleValue(string section, string key, double defaultValue = 0)
    {
      double ret = defaultValue;
      string s = GetStringValue(section, key, defaultValue.ToString());
      if (s != null && double.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    public TV_2DVECTOR GetTV_2DVECTOR(string section, string key, TV_2DVECTOR defaultValue = default(TV_2DVECTOR))
    {
      float[] list = GetFloatList(section, key, new float[0]);
      if (list.Length >= 2)
        return new TV_2DVECTOR(list[0], list[1]);
      return defaultValue;
    }

    public TV_3DVECTOR GetTV_3DVECTOR(string section, string key, TV_3DVECTOR defaultValue = default(TV_3DVECTOR))
    {
      float[] list = GetFloatList(section, key, new float[0]);
      if (list.Length >= 3)
        return new TV_3DVECTOR(list[0], list[1], list[2]);
      return defaultValue;
    }

    public TV_COLOR GetTV_COLOR(string section, string key, TV_COLOR defaultValue = default(TV_COLOR))
    {
      float[] list = GetFloatList(section, key, new float[0]);
      if (list.Length >= 4)
        return new TV_COLOR(list[0], list[1], list[2], list[3]);
      return defaultValue;
    }


    public int[] GetIntList(string section, string key, int[] defaultList, char delimiter = ',')
    {
      string str = GetStringValue(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      int[] ret = new int[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        int.TryParse(tokens[i], out ret[i]);
      return ret;
    }

    public uint[] GetUIntList(string section, string key, uint[] defaultList, char delimiter = ',')
    {
      string str = GetStringValue(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      uint[] ret = new uint[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        uint.TryParse(tokens[i], out ret[i]);
      return ret;
    }


    public float[] GetFloatList(string section, string key, float[] defaultList, char delimiter = ',')
    {
      string str = GetStringValue(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      float[] ret = new float[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        float.TryParse(tokens[i], out ret[i]);

      return ret;
    }

    public double[] GetDoubleList(string section, string key, double[] defaultList, char delimiter = ',')
    {
      string str = GetStringValue(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      double[] ret = new double[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        double.TryParse(tokens[i], out ret[i]);

      return ret;
    }

    public bool GetBoolValue(string section, string key, bool defaultValue = false)
    {
      string s = GetStringValue(section, key, "").ToLower().Trim();
      if (s.Equals("0") || s.Trim().Equals("false") || s.Equals("no"))
        return false;
      else if (s.Equals("1") || s.Equals("true") || s.Equals("yes"))
        return true;
      return defaultValue;
    }

    public bool[] GetBoolList(string section, string key, bool[] defaultList, char delimiter = ',')
    {
      string str = GetStringValue(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      bool[] ret = new bool[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
      {
        string s = GetStringValue(section, key, "").ToLower().Trim();
        if (s.Equals("0") || s.Trim().Equals("false") || s.Equals("no"))
          ret[i] = false;
        else if (s.Equals("1") || s.Equals("true") || s.Equals("yes"))
          ret[i] = true;
      }
      return ret;
    }

    public string[] GetStringList(string section, string key, string[] defaultList, char delimiter = ',')
    {
      string str = GetStringValue(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      return tokens;
    }

    public StringBuilder[] GetStringBuilderList(string section, string key, StringBuilder[] defaultList, char delimiter = ',')
    {
      string[] tokens = GetStringList(section, key, null, delimiter);
      if (tokens == null)
        return defaultList;

      StringBuilder[] ret = new StringBuilder[tokens.Length];
      for (int i = 0; i < tokens.Length; i++)
        ret[i] = new StringBuilder(tokens[i]);
      return ret;
    }

    private string[] ConvertToStringArray(Array t)
    {
      string[] ret = new string[t.Length];
      for (int i = 0; i < t.Length; i++)
        ret[i] = t.GetValue(i).ToString();
      return ret;
    }
  }
}
