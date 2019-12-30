using Primrose.Primitives.Extensions;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    /// <summary>
    /// Sets a string value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    public void SetString(string section, string key, string value)
    {
      if (value == null)
        return;

      if (!m_sections.ContainsKey(section))
        m_sections.Add(section, new INISection("[{0}]".F(section)));

      m_sections[section].SetValue(key, value);
    }

    /// <summary>
    /// Sets an int value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    public void SetInt(string section, string key, int value) { SetString(section, key, value.ToString()); }

    /// <summary>
    /// Sets a float value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    public void SetFloat(string section, string key, float value) { SetString(section, key, value.ToString()); }

    /// <summary>
    /// Sets a double value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    public void SetDouble(string section, string key, double value) { SetString(section, key, value.ToString()); }

    /// <summary>
    /// Sets a bool value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    public void SetBool(string section, string key, bool value) { SetString(section, key, value.ToString()); }

    /// <summary>
    /// Sets an int[] array in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="list">The array of values to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetIntArray(string section, string key, int[] list, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }

    /// <summary>
    /// Sets a float[] array in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="list">The array of values to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetFloatArray(string section, string key, float[] list, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }

    /// <summary>
    /// Sets a float2 value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetFloat2(string section, string key, float2 value, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }

    /// <summary>
    /// Sets a float3 value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetFloat3(string section, string key, float3 value, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }

    /// <summary>
    /// Sets a float4 value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetFloat4(string section, string key, float4 value, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }

    /// <summary>
    /// Sets a double[] array in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="list">The array of values to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetDoubleArray(string section, string key, double[] list, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }

    /// <summary>
    /// Sets a bool[] array in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="list">The array of values to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetBoolArray(string section, string key, bool[] list, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }

    /// <summary>
    /// Sets a string[] array in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="list">The array of values to be set</param>
    /// <param name="delimiter">The delimiter to be inserted between values</param>
    public void SetStringArray(string section, string key, string[] list, char delimiter = ',') { SetString(section, key, string.Join(delimiter.ToString(), list)); }

    /// <summary>
    /// Sets an enum value in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    public void SetEnum<T>(string section, string key, T value) where T : struct { SetString(section, key, value.ToString().Replace(", ", "|")); }

    /// <summary>
    /// Sets an enum array in the INIFile 
    /// </summary>
    /// <param name="section">The section that will contain the key-value pair</param>
    /// <param name="key">The key that will contain the value</param>
    /// <param name="value">The value to be set</param>
    public void SetEnumArray<T>(string section, string key, T[] list, char delimiter = ',') where T : struct { SetString(section, key, string.Join(delimiter.ToString(), list)); }
  }
}
