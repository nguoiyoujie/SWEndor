//using MTV3D65;
using Primrose.Primitives;
using Primrose.Primitives.ValueTypes;
using System;
using System.Text;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    /// <summary>
    /// The default delimiter referenced for use in the INIFile class
    /// </summary>
    public static readonly char[] DefaultDelimiter = new char[] { ',' };

    /// <summary>
    /// Gets a string value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public string GetString(string section, string key, string defaultValue = "")
    {
      return GetString(section, key, defaultValue, section);
    }

    private string GetString(string section, string key, string defaultValue, string firstsection)
    {
      if (m_sections.ContainsKey(section))
      {
        if (m_sections[section].HasKey(key))
          return m_sections[section].GetLine(key).Value;

        string val = defaultValue;
        foreach (string inherit in m_sections[section].Inherits)
        {
          if (inherit != firstsection
            && val == defaultValue
            && m_sections.ContainsKey(inherit))
          {
            val = GetString(inherit, key, defaultValue, firstsection);
          }
        }
        return val;
      }
      return defaultValue;
    }

    /// <summary>
    /// Gets an int value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public int GetInt(string section, string key, int defaultValue = 0)
    {
      int ret = defaultValue;
      string s = GetString(section, key, defaultValue.ToString());
      if (s != null && int.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    /// <summary>
    /// Gets a uint value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public uint GetUInt(string section, string key, uint defaultValue = 0)
    {
      uint ret = defaultValue;
      string s = GetString(section, key, defaultValue.ToString());
      if (s != null && uint.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    /// <summary>
    /// Gets a float value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public float GetFloat(string section, string key, float defaultValue = 0)
    {
      float ret = defaultValue;
      string s = GetString(section, key, defaultValue.ToString());
      if (s != null && float.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    /// <summary>
    /// Gets a double value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public double GetDouble(string section, string key, double defaultValue = 0)
    {
      double ret = defaultValue;
      string s = GetString(section, key, defaultValue.ToString());
      if (s != null && double.TryParse(s, out ret))
        return ret;
      return defaultValue;
    }

    /// <summary>
    /// Gets a float2 value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public float2 GetFloat2(string section, string key, float2 defaultValue = default(float2))
    {
      float[] list = GetFloatArray(section, key, new float[0]);
      if (list.Length >= 2)
        return new float2(list[0], list[1]);
      else if (list.Length == 1)
        return new float2(list[0], defaultValue[1]);

      return defaultValue;
    }

    /// <summary>
    /// Gets a float3 value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public float3 GetFloat3(string section, string key, float3 defaultValue = default(float3))
    {
      float[] list = GetFloatArray(section, key, new float[0]);
      if (list.Length >= 3)
        return new float3(list[0], list[1], list[2]);
      else if (list.Length == 2)
        return new float3(list[0], list[1], defaultValue[2]);
      else if (list.Length == 1)
        return new float3(list[0], defaultValue[1], defaultValue[2]);

      return defaultValue;
    }

    /// <summary>
    /// Gets a float4 value from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public float4 GetFloat4(string section, string key, float4 defaultValue = default(float4))
    {
      float[] list = GetFloatArray(section, key, new float[0]);
      if (list.Length >= 4)
        return new float4(list[0], list[1], list[2], list[3]);
      else if (list.Length == 3)
        return new float4(list[0], list[1], list[2], defaultValue[3]);
      else if (list.Length == 2)
        return new float4(list[0], list[1], defaultValue[2], defaultValue[3]);
      else if (list.Length == 1)
        return new float4(list[0], defaultValue[1], defaultValue[2], defaultValue[3]);

      return defaultValue;
    }

    /// <summary>
    /// Gets an int[] array from the INIFile 
    /// </summary>
    /// <param name="section">The section containing the key-value pair</param>
    /// <param name="key">The key containing the value</param>
    /// <param name="defaultList">The default array</param>
    /// <returns>The value belong to the section and key in the INIFile. If any key does not exist, returns defaultValue</returns>
    public int[] GetIntArray(string section, string key, int[] defaultList)
    {
      return GetIntArray(section, key, defaultList, DefaultDelimiter);
    }

    private int[] GetIntArray(string section, string key, int[] defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      int[] ret = new int[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        int.TryParse(tokens[i], out ret[i]);
      return ret;
    }

    public uint[] GetUIntArray(string section, string key, uint[] defaultList)
    {
      return GetUIntArray(section, key, defaultList, DefaultDelimiter);
    }

    public uint[] GetUIntArray(string section, string key, uint[] defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      uint[] ret = new uint[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        uint.TryParse(tokens[i], out ret[i]);
      return ret;
    }

    public float[] GetFloatArray(string section, string key, float[] defaultList)
    {
      return GetFloatArray(section, key, defaultList, DefaultDelimiter);
    }

    public float[] GetFloatArray(string section, string key, float[] defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      float[] ret = new float[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        float.TryParse(tokens[i], out ret[i]);

      return ret;
    }

    public double[] GetDoubleArray(string section, string key, double[] defaultList)
    {
      return GetDoubleArray(section, key, defaultList, DefaultDelimiter);
    }

    public double[] GetDoubleArray(string section, string key, double[] defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      double[] ret = new double[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
        double.TryParse(tokens[i], out ret[i]);

      return ret;
    }

    public bool GetBool(string section, string key, bool defaultValue = false)
    {
      string s = GetString(section, key, "").ToLower().Trim();
      if (s.Equals("0") || s.Trim().Equals("false") || s.Equals("no"))
        return false;
      else if (s.Equals("1") || s.Equals("true") || s.Equals("yes"))
        return true;
      return defaultValue;
    }

    public bool[] GetBoolArray(string section, string key, bool[] defaultList)
    {
      return GetBoolArray(section, key, defaultList, DefaultDelimiter);
    }

    public bool[] GetBoolArray(string section, string key, bool[] defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      bool[] ret = new bool[tokens.Length];

      for (int i = 0; i < tokens.Length; i++)
      {
        string s = GetString(section, key, "").ToLower().Trim();
        if (s.Equals("0") || s.Trim().Equals("false") || s.Equals("no"))
          ret[i] = false;
        else if (s.Equals("1") || s.Equals("true") || s.Equals("yes"))
          ret[i] = true;
      }
      return ret;
    }

    public string[] GetStringArray(string section, string key, string[] defaultList)
    {
      return GetStringArray(section, key, defaultList, DefaultDelimiter);
    }

    public string[] GetStringArray(string section, string key, string[] defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);
      return tokens;
    }

    public StringBuilder[] GetStringBuilderArray(string section, string key, StringBuilder[] defaultList)
    {
      return GetStringBuilderArray(section, key, defaultList, DefaultDelimiter);
    }

    public StringBuilder[] GetStringBuilderArray(string section, string key, StringBuilder[] defaultList, char[] delimiter)
    {
      string[] tokens = GetStringArray(section, key, null, delimiter);
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

    private string[] ConvertToStringArray(float2 v)
    {
      string[] ret = new string[2];
      ret[0] = v.x.ToString();
      ret[1] = v.y.ToString();
      return ret;
    }

    private string[] ConvertToStringArray(float3 v)
    {
      string[] ret = new string[3];
      ret[0] = v.x.ToString();
      ret[1] = v.y.ToString();
      ret[2] = v.z.ToString();
      return ret;
    }
    
    private string[] ConvertToStringArray(float4 v)
    {
      string[] ret = new string[4];
      ret[0] = v.x.ToString();
      ret[1] = v.y.ToString();
      ret[2] = v.z.ToString();
      ret[3] = v.w.ToString();
      return ret;
    }

    public object GetEnum(Type t, string section, string key, object defaultValue)
    {
      string s = GetString(section, key, defaultValue.GetEnumName()).Replace("|", ","); ;
      try { return Enum.Parse(t, s); }
      catch { return defaultValue; }
    }

    public T GetEnum<T>(string section, string key, T defaultValue)
    {
      string s = GetString(section, key, defaultValue.GetEnumName()).Replace("|", ","); ;
      try { return (T)Enum.Parse(typeof(T), s); }
      catch { return defaultValue; }
    }

    public object GetEnumArray(Type t, string section, string key, object defaultList)
    {
      return GetEnumArray(t, section, key, defaultList, DefaultDelimiter);
    }

    public object GetEnumArray(Type t, string section, string key, object defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);

      Type et = t.GetElementType();
      Array array = Array.CreateInstance(et, tokens.Length);

      for (int i = 0; i < tokens.Length; i++)
      {
        try { array.SetValue(Enum.Parse(et, tokens[i]), i); }
        catch { }
      }
      return array;
    }

    public T GetEnumArray<T>(string section, string key, T defaultList)
    {
      return GetEnumArray(section, key, defaultList, DefaultDelimiter);
    }

    public T GetEnumArray<T>(string section, string key, T defaultList, char[] delimiter)
    {
      string str = GetString(section, key, "");
      if (str == "")
        return defaultList;

      string[] tokens = str.Split(delimiter);

      Type et = typeof(T).GetElementType();
      Array array = Array.CreateInstance(et, tokens.Length);

      for (int i = 0; i < tokens.Length; i++)
      {
        try { array.SetValue(Enum.Parse(et, tokens[i]), i); }
        catch { }
      }
      return (T)(object)array;
    }
  }
}
