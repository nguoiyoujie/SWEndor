using MTV3D65;
using Primrose.Primitives;

namespace SWEndor.FileFormat.INI
{
  public partial class INIFile
  {
    public void SetStringValue(string section, string key, string value)
    {
      if (value == null)
        return;

      if (!m_sections.ContainsKey(section))
        m_sections.Add(section, new INISection("[" + section + "]"));

      m_sections[section].SetValue(key, value);
    }

    public void SetIntValue(string section, string key, int value) { SetStringValue(section, key, value.ToString()); }
    public void SetFloatValue(string section, string key, float value) { SetStringValue(section, key, value.ToString()); }
    public void SetDoubleValue(string section, string key, double value) { SetStringValue(section, key, value.ToString()); }
    public void SetBoolValue(string section, string key, bool value) { SetStringValue(section, key, value.ToString()); }
    public void SetIntList(string section, string key, int[] list, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }
    public void SetFloatList(string section, string key, float[] list, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }
    public void SetTV_2DVECTOR(string section, string key, TV_2DVECTOR value, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }
    public void SetTV_3DVECTOR(string section, string key, TV_3DVECTOR value, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }
    public void SetTV_COLOR(string section, string key, TV_COLOR value, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(value))); }
    public void SetDoubleList(string section, string key, double[] list, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }
    public void SetBoolList(string section, string key, bool[] list, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), ConvertToStringArray(list))); }
    public void SetStringList(string section, string key, string[] list, char delimiter = ',') { SetStringValue(section, key, string.Join(delimiter.ToString(), list)); }
    public void SetEnumValue<T>(string section, string key, T value) where T : struct { SetStringValue(section, key, value.GetEnumName()); }
  }
}
